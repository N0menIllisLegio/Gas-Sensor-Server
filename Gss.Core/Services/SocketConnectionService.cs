using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Gss.Core.Entities;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;
using Gss.Core.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gss.Core.Services
{
  public class SocketConnectionService
  {
    private const char _commandSeparator = '|';
    private const string _okResponse = "Server_OK";
    private const string _attentionResponse = "Server_AT";
    private const string _sensorValueResponse = "Server_SV";
    private const int _receivedSensorsDatMaxSize = 5;

    private readonly ILogger<SocketConnectionService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly object _locker = new ();
    private readonly List<SensorData> _receivedSensorsData = new ();

    private int _runInsertReceivedSensorsData = 0;

    public SocketConnectionService(IServiceScopeFactory serviceScopeFactory,
      ILogger<SocketConnectionService> logger)
    {
      _serviceScopeFactory = serviceScopeFactory;
      _logger = logger;
    }

    public async void RunAsync()
    {
      await Task.Run(Run);
    }

    private void Run()
    {
      var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

      socket.Bind(new IPEndPoint(IPAddress.Parse(Settings.Socket.IPAddress), Settings.Socket.Port));
      socket.Listen(Settings.Socket.ListenQueue);

      try
      {
        while (true)
        {
          var acceptedSocket = socket.Accept();

          acceptedSocket.ReceiveTimeout = Settings.Socket.ReceiveTimeout;
          acceptedSocket.SendTimeout = Settings.Socket.SendTimeout;

          HandleConnection(acceptedSocket);
        }
      }
      catch (Exception exception)
      {
        _logger.LogCritical(exception, exception.Message);
      }
      finally
      {
        _logger.LogWarning("SocketConnectionService stopped working.");
        socket?.Close();
      }
    }

    private async void HandleConnection(Socket socket)
    {
      Microcontroller connectedMicrocontroller = null;

      try
      {
        string request = await ReceiveMicrocontrollerRequest(socket);
        var (receivedCommand, receivedArguments) = SplitRequest(request);

        if (receivedCommand != "STM_AUTH")
        {
          _logger.LogWarning("Unknown command. Endpoint: {0}\tRequest: {1}", socket.RemoteEndPoint, request);
          socket.Shutdown(SocketShutdown.Both);
          socket.Close();

          return;
        }

        if (receivedArguments.Length != 3
          || String.IsNullOrEmpty(receivedArguments[2])
          || !Guid.TryParse(receivedArguments[0], out var userID)
          || !Guid.TryParse(receivedArguments[1], out var microcontrollerID))
        {
          _logger.LogWarning("Failed to parse data. Endpoint: {0}\tRequest: {1}",
            socket.RemoteEndPoint, request);

          socket.Shutdown(SocketShutdown.Both);
          socket.Close();

          return;
        }

        string ownerEmail = null;

        using (var scope = _serviceScopeFactory.CreateScope())
        {
          var microcontrollersService = scope.ServiceProvider.GetRequiredService<IMicrocontrollersService>();

          (connectedMicrocontroller, ownerEmail) = await microcontrollersService
            .AuthenticateMicrocontrollersAsync(userID, microcontrollerID, receivedArguments[2],
              (socket.RemoteEndPoint as IPEndPoint).Address.ToString());
        }

        if (connectedMicrocontroller is null)
        {
          _logger.LogWarning("Failed to authenticate. Endpoint: {0}\tRequest: {1}",
            socket.RemoteEndPoint, request);

          socket.Shutdown(SocketShutdown.Both);
          socket.Close();

          return;
        }

        await SendMicrocontrollerResponse(socket, _okResponse);

        request = await ReceiveMicrocontrollerRequest(socket);
        (receivedCommand, receivedArguments) = SplitRequest(request);

        switch (receivedCommand)
        {
          case "STM_DATA":
            if (receivedArguments.Length != 3
              || !Guid.TryParse(receivedArguments[0], out var sensorID)
              || !DateTime.TryParse(receivedArguments[1], CultureInfo.CreateSpecificCulture("en-US"),
                DateTimeStyles.None, out var sensorValueReadedDateTime)
              || !Int32.TryParse(receivedArguments[2], out int sensorValue))
            {
              _logger.LogWarning("Failed to parse data. Endpoint: {0}\tMicrocontroller: {1}\tRequest: {2}",
                socket.RemoteEndPoint, connectedMicrocontroller.Id, request);

              socket.Shutdown(SocketShutdown.Both);
              socket.Close();

              return;
            }

            await SendMicrocontrollerResponse(socket, _okResponse);

            if (_receivedSensorsData.Count(sensorData => sensorData.MicrocontrollerID == connectedMicrocontroller.Id
              && sensorData.SensorID == sensorID
              && sensorData.ValueReadTime == sensorValueReadedDateTime) == 0)
            {
              var sensorData = new SensorData
              {
                Id = Guid.NewGuid(),
                MicrocontrollerID = connectedMicrocontroller.Id,
                SensorID = sensorID,
                SensorValue = sensorValue,
                ValueReadTime = sensorValueReadedDateTime,
                ValueReceivedTime = DateTime.UtcNow
              };

              lock (_locker)
              {
                _receivedSensorsData.Add(sensorData);
              }

              if (_receivedSensorsData.Count > _receivedSensorsDatMaxSize)
              {
                _ = Task.Run(InsertReceivedSensorsData);
              }
            }

            break;

          case "STM_RQ":

            using (var scope = _serviceScopeFactory.CreateScope())
            {
              var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

              connectedMicrocontroller = await unitOfWork.Microcontrollers.ReloadAsync(connectedMicrocontroller);
            }

            if (connectedMicrocontroller.RequestedSensorID is not null)
            {
              await SendMicrocontrollerResponse(socket, $"{_sensorValueResponse}|{connectedMicrocontroller.RequestedSensorID};");

              request = await ReceiveMicrocontrollerRequest(socket);
              (receivedCommand, receivedArguments) = SplitRequest(request);

              if (receivedArguments.Length != 3
              || !Guid.TryParse(receivedArguments[0], out sensorID)
              || !DateTime.TryParse(receivedArguments[1], CultureInfo.CreateSpecificCulture("en-US"),
                DateTimeStyles.None, out sensorValueReadedDateTime)
              || !Int32.TryParse(receivedArguments[2], out sensorValue))
              {
                _logger.LogWarning("Failed to parse data. Endpoint: {0}\tMicrocontroller: {1}\tRequest: {2}",
                  socket.RemoteEndPoint, connectedMicrocontroller.Id, request);

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();

                return;
              }

              await SendMicrocontrollerResponse(socket, _okResponse);

              using var scope = _serviceScopeFactory.CreateScope();
              var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
              unitOfWork.SensorsData.Add(new SensorData
              {
                Id = Guid.NewGuid(),
                MicrocontrollerID = connectedMicrocontroller.Id,
                SensorID = sensorID,
                SensorValue = sensorValue,
                ValueReadTime = sensorValueReadedDateTime,
                ValueReceivedTime = DateTime.UtcNow
              });

              connectedMicrocontroller.RequestedSensorID = null;
              unitOfWork.Microcontrollers.Update(connectedMicrocontroller);

              await unitOfWork.SaveAsync();
            }
            else
            {
              await SendMicrocontrollerResponse(socket, _attentionResponse);
            }

            break;

          case "STM_DT":
            var currentDateTime = DateTime.UtcNow;

            await SendMicrocontrollerResponse(socket, $"Server_DT|" +
              $"Date={currentDateTime.Day};Month={currentDateTime.Month};Year={currentDateTime.Date:yy};" +
              $"WeekDay={(int)currentDateTime.DayOfWeek};Hours={currentDateTime.Hour};" +
              $"Minutes={currentDateTime.Minute};Seconds={currentDateTime.Second};");

            break;

          default:
            _logger.LogWarning("Unknown command. Endpoint: {0}\tRequest: {1}", socket.RemoteEndPoint, request);

            break;
        }

        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
      }
      catch (OperationCanceledException)
      {
        _logger.LogWarning("Endpoint: {0}\tMicrocontroller {1} was disconnected after {2}ms timeout", socket.RemoteEndPoint,
          connectedMicrocontroller?.Id, Settings.Socket.ReceiveTimeout);
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "Endpoint: {0}\tConnected microcontroller: {1}", socket.RemoteEndPoint, connectedMicrocontroller?.Id);
      }
      finally
      {
        socket?.Dispose();
      }
    }

    private async Task<string> ReceiveMicrocontrollerRequest(Socket socket)
    {
      var receivedMessageBuilder = new StringBuilder();
      byte[] receivedData = new byte[256];

      var cancellationTokenSource = new CancellationTokenSource();
      cancellationTokenSource.CancelAfter(Settings.Socket.ReceiveTimeout);

      do
      {
        int bytesReceived = await socket.ReceiveAsync(receivedData, SocketFlags.None, cancellationTokenSource.Token);
        receivedMessageBuilder.Append(Encoding.ASCII.GetString(receivedData, 0, bytesReceived));
      }
      while (socket.Available > 0);

      _logger.LogInformation("Received Data. Endpoint: {0}\tRequest: {1}",
        socket.RemoteEndPoint, receivedMessageBuilder.ToString());

      return receivedMessageBuilder.ToString();
    }

    private (string command, string[] arguments) SplitRequest(string request)
    {
      string[] splittedReceivedMessage = request.Split(_commandSeparator);
      string receivedCommand = splittedReceivedMessage.First();
      string[] receivedArguments = splittedReceivedMessage.Last().Split(';');

      return (receivedCommand, receivedArguments);
    }

    private async Task SendMicrocontrollerResponse(Socket socket, string response)
    {
      _logger.LogInformation("Received Data. Endpoint: {0}\tResponse: {1}",
        socket.RemoteEndPoint, response);

      byte[] responseBytes = Encoding.ASCII.GetBytes(response);
      await socket.SendAsync(responseBytes, SocketFlags.None);
    }

    private async Task InsertReceivedSensorsData()
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
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        try
        {
          await unitOfWork.SensorsData.BulkInsertIfNotExists(receivedData);
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
}
