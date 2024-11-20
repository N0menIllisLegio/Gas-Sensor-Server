using System.Net;
using System.Net.Sockets;
using Gss.Core.Entities;
using Gss.MicrocontrollerListener.Data;
using Gss.MicrocontrollerListener.Email;
using Gss.MicrocontrollerListener.Notifications;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace Gss.MicrocontrollerListener.MicrocontrollerHandlers;

internal class SocketConnectionService
{
  private const int ReceivedSensorsDataMaxSize = 5;

  private readonly ILogger<SocketConnectionService> _logger;
  private readonly IHubContext<NotificationsHub> _notificationHub;
  private readonly IServiceScopeFactory _serviceScopeFactory;
  private readonly MicrocontrollersConnectionsOptions _microcontrollersConnectionsOptions;

  private readonly object _locker = new ();
  private readonly List<SensorData> _receivedSensorsData = new ();

  private int _runInsertReceivedSensorsData;

  public SocketConnectionService(IServiceScopeFactory serviceScopeFactory,
    IOptions<MicrocontrollersConnectionsOptions> microcontrollersConnectionsOptions,
    ILogger<SocketConnectionService> logger, IHubContext<NotificationsHub> notificationHub)
  {
    _serviceScopeFactory = serviceScopeFactory;
    _microcontrollersConnectionsOptions = microcontrollersConnectionsOptions.Value;
    _logger = logger;
    _notificationHub = notificationHub;
  }

  public async void RunAsync()
  {
    await Task.Run(Run);
  }

  private void Run()
  {
    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    socket.Bind(new IPEndPoint(
      IPAddress.Parse(_microcontrollersConnectionsOptions.IpAddress), _microcontrollersConnectionsOptions.Port));

    socket.Listen(_microcontrollersConnectionsOptions.ListenQueue);

    try
    {
      while (true)
      {
        var acceptedSocket = socket.Accept();

        acceptedSocket.ReceiveTimeout = _microcontrollersConnectionsOptions.ReceiveTimeout;
        acceptedSocket.SendTimeout = _microcontrollersConnectionsOptions.SendTimeout;

        HandleConnection(acceptedSocket);
      }
    }
    catch (Exception exception)
    {
      _logger.LogCritical(exception,
        "SocketConnectionService stopped working with error: {ErrorMessage}", exception.Message);
    }
    finally
    {
      socket.Close();
    }
  }

  private async void HandleConnection(Socket socket)
  {
    using var connectionManager =
      new MicrocontrollerConnectionManager(socket, _microcontrollersConnectionsOptions, _logger);

    Core.Entities.Microcontroller? connectedMicrocontroller = null;

    try
    {
      var request = await connectionManager.ReceiveRequestAsync();

      if (request is null)
        return;

      var authRequest = request.ParseAuthRequest();

      if (authRequest is null)
      {
        _logger.LogWarning("Failed to read auth request. {Endpoint}", socket.RemoteEndPoint);
        return;
      }

      using (var scope = _serviceScopeFactory.CreateScope())
      {
        var listenerRepository = scope.ServiceProvider.GetRequiredService<IListenerRepository>();

        connectedMicrocontroller = await listenerRepository.GetMicrocontrollerAsync(authRequest.MicrocontrollerId);

        if (connectedMicrocontroller is not null)
        {
          // TODO: HMAC. API-Key or hash
          if (connectedMicrocontroller.Key == authRequest.Password)
          {
            await listenerRepository.UpdateLastResponseTimeAsync(connectedMicrocontroller.Id);
          }
        }
      }

      if (connectedMicrocontroller is null)
      {
        _logger.LogWarning("Failed to authenticate. {Endpoint} --- {MicrocontrollerId}",
          socket.RemoteEndPoint, authRequest.MicrocontrollerId);

        return;
      }

      await connectionManager.SendOkAsync();

      request = await connectionManager.ReceiveRequestAsync();

      if (request is null)
        return;

      switch (request.Command)
      {
        case MicrocontrollerRequest.DataCommand:
          var dataRequest = request.ParseDataRequest();

          if (dataRequest is null)
          {
            _logger.LogWarning("Failed to parse data. {Endpoint} --- {MicrocontrollerId} --- {Request}",
              socket.RemoteEndPoint, connectedMicrocontroller.Id, dataRequest);

            return;
          }

          await connectionManager.SendOkAsync();

          var microcontrollerSensor = connectedMicrocontroller.MicrocontrollerSensors
            .FirstOrDefault(ms => ms.Id == dataRequest.MicrocontrollerSensorId);

          if (microcontrollerSensor is null)
          {
            _logger.LogWarning("Such sensor({Id}) doesn't connected to microcontroller. {Endpoint} --- {Microcontroller}",
              dataRequest.MicrocontrollerSensorId, socket.RemoteEndPoint, connectedMicrocontroller.Id);

            return;
          }

          if (microcontrollerSensor.CriticalValue <= dataRequest.SensorValue)
          {
            // TODO: message queue
            using var scope = _serviceScopeFactory.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            await emailService.SendCriticalValueEmailAsync(dataRequest.SensorValue,
              microcontrollerSensor.CriticalValue.Value, connectedMicrocontroller, microcontrollerSensor.Sensor);
          }

          var sensorData = new SensorData
          {
            MicrocontrollerSensorId = microcontrollerSensor.Id,
            ReadTime = DateTime.SpecifyKind(dataRequest.SensorValueReadTime, DateTimeKind.Utc),
            Value = dataRequest.SensorValue,
            ReceivedTime = DateTime.UtcNow
          };

          bool runBulkInsert;

          lock (_locker)
          {
            _receivedSensorsData.Add(sensorData);

            runBulkInsert = _receivedSensorsData.Count > ReceivedSensorsDataMaxSize;
          }

          if (runBulkInsert)
          {
            _ = Task.Run(InsertReceivedSensorsDataAsync);
          }

          break;

        case MicrocontrollerRequest.RequestSensorValueCommand:
          if (connectedMicrocontroller.RequestedMicrocontrollerSensorId.HasValue)
          {
            await connectionManager.SendRequestSensorValueAsync(connectedMicrocontroller
              .RequestedMicrocontrollerSensorId.Value);

            request = await connectionManager.ReceiveRequestAsync();

            if (request is null)
              return;

            dataRequest = request.ParseDataRequest();

            if (dataRequest is null)
            {
              _logger.LogWarning("Failed to parse data. {Endpoint} --- {MicrocontrollerId} --- {Request}",
                socket.RemoteEndPoint, connectedMicrocontroller.Id, dataRequest);

              return;
            }

            await connectionManager.SendOkAsync();

            var newDataEntry = new SensorData
            {
              MicrocontrollerSensorId = connectedMicrocontroller.RequestedMicrocontrollerSensorId.Value,
              ReadTime = DateTime.SpecifyKind(dataRequest.SensorValueReadTime, DateTimeKind.Utc),
              Value = dataRequest.SensorValue,
              ReceivedTime = DateTime.UtcNow
            };

            var requestedMicrocontrollerSensor = connectedMicrocontroller.MicrocontrollerSensors.First(
              x => x.Id == connectedMicrocontroller.RequestedMicrocontrollerSensorId);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
              var listenerRepository = scope.ServiceProvider.GetRequiredService<IListenerRepository>();
              await listenerRepository.ResetMicrocontrollerRequestSensorValueAsync(connectedMicrocontroller.Id);
            }

            lock (_locker)
            {
              _receivedSensorsData.Add(newDataEntry);

              runBulkInsert = _receivedSensorsData.Count > ReceivedSensorsDataMaxSize;
            }

            await _notificationHub.Clients.User(connectedMicrocontroller.OwnerId!.Value.ToString())
              .SendAsync("Notification", new NotifySensorResponseDto
              {
                MicrocontrollerSensorId = requestedMicrocontrollerSensor.Id,
                SensorName = requestedMicrocontrollerSensor.Sensor.Name,
                SensorType = requestedMicrocontrollerSensor.Sensor.Type.Name,
                SensorValue = dataRequest.SensorValue,
                SensorTypeIcon = requestedMicrocontrollerSensor.Sensor.Type.Icon,
                SensorTypeUnits = requestedMicrocontrollerSensor.Sensor.Type.Units
              });

            if (runBulkInsert)
            {
              _ = Task.Run(InsertReceivedSensorsDataAsync);
            }
          }
          else
            await connectionManager.SendAttentionAsync();
          break;

        case MicrocontrollerRequest.DateSyncCommand:
          await connectionManager.SendDateTimeAsync();
          break;

        default:
          _logger.LogCritical("Missing implementation for {Command}. {Endpoint} --- {Request}",
            request.Command, socket.RemoteEndPoint, request);

          break;
      }
    }
    catch (OperationCanceledException)
    {
      _logger.LogWarning("Microcontroller {Id} was disconnected after {Timeout}ms timeout --- {Endpoint}",
        connectedMicrocontroller?.Id, _microcontrollersConnectionsOptions.ReceiveTimeout, socket.RemoteEndPoint);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, "Failed to handle microcontroller connection. {Endpoint} --- {MicrocontrollerId}",
        socket.RemoteEndPoint, connectedMicrocontroller?.Id);
    }
    finally
    {
      socket.Dispose();
    }
  }

  private async Task InsertReceivedSensorsDataAsync()
  {
    if (Interlocked.CompareExchange(ref _runInsertReceivedSensorsData, 0, 1) == 0)
    {
      List<SensorData> receivedData;

      lock (_locker)
      {
        receivedData = _receivedSensorsData.ToList();
        _receivedSensorsData.Clear();
      }

      using var scope = _serviceScopeFactory.CreateScope();
      var repository = scope.ServiceProvider.GetRequiredService<IListenerRepository>();

      try
      {
        await repository.BulkInsertIfNotExistsAsync(receivedData);
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, exception.Message);
      }
      finally
      {
        Interlocked.Exchange(ref _runInsertReceivedSensorsData, 1);
      }
    }
  }
}