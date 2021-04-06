using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
    private const int _bulkInsertSize = 1;

    private readonly ILogger<SocketConnectionService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

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
      var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
      {
        ReceiveTimeout = Settings.Socket.ReceiveTimeout,
        SendTimeout = Settings.Socket.SendTimeout
      };

      socket.Bind(new IPEndPoint(IPAddress.Parse(Settings.Socket.IPAddress), Settings.Socket.Port));
      socket.Listen(Settings.Socket.ListenQueue);

      try
      {
        while (true)
        {
          var acceptedSocket = socket.Accept();
          HandleConnection(acceptedSocket);
        }
      }
      catch (Exception exception)
      {
        _logger.LogCritical(exception, exception.Message);
      }
      finally
      {
        socket?.Close();
      }
    }

    private async void HandleConnection(Socket socket)
    {
      var receivedSensorsData = new List<SensorData>();
      Microcontroller connectedMicrocontroller = null;

      try
      {
        while (socket.Connected)
        {
          var receivedMessageBuilder = new StringBuilder();
          byte[] receivedData = new byte[256];

          do
          {
            int bytesReceived = await socket.ReceiveAsync(receivedData, SocketFlags.None);
            receivedMessageBuilder.Append(Encoding.ASCII.GetString(receivedData, 0, bytesReceived));
          }
          while (socket.Available > 0);

          string[] splittedReceivedMessage = receivedMessageBuilder.ToString().Split(_commandSeparator);
          string receivedCommand = splittedReceivedMessage.First();
          string[] splittedArguments = splittedReceivedMessage.Last().Split(';');

          string response = null;

          switch (receivedCommand)
          {
            case "STM_AT":
              response = _attentionResponse;

              break;

            case "STM_AUTH":
              Guid userID, microcontrollerID;

              if (connectedMicrocontroller is not null
                || splittedArguments.Length != 3
                || !Guid.TryParse(splittedArguments[0], out userID)
                || !Guid.TryParse(splittedArguments[1], out microcontrollerID)
                || String.IsNullOrEmpty(splittedArguments[2]))
              {
                _logger.LogWarning("Failed to parse data. Endpoint: {0}\tData: {1}",
                  socket.RemoteEndPoint, receivedMessageBuilder.ToString());

                return;
              }

              using (var scope = _serviceScopeFactory.CreateScope())
              {
                var microcontrollersService = scope.ServiceProvider.GetRequiredService<IMicrocontrollersService>();

                connectedMicrocontroller = await microcontrollersService
                  .AuthenticateMicrocontrollersAsync(userID, microcontrollerID, splittedArguments[2]);
              }

              if (connectedMicrocontroller is null)
              {
                _logger.LogWarning("Failed to authenticate. Endpoint: {0}\tRequest: {1}",
                  socket.RemoteEndPoint, receivedMessageBuilder.ToString());

                return;
              }

              response = _okResponse;

              break;

            case "STM_DATA":
              Guid sensorID;
              DateTime sensorValueReadedDateTime;
              int sensorValue;

              if (connectedMicrocontroller is null)
              {
                _logger.LogWarning("Unauthorized. Endpoint: {0}\tRequest: {1}",
                  socket.RemoteEndPoint, receivedMessageBuilder.ToString());

                return;
              }

              if (splittedArguments.Length != 3
                || !Guid.TryParse(splittedArguments[0], out sensorID)
                || !DateTime.TryParse(splittedArguments[1], CultureInfo.CreateSpecificCulture("en-US"),
                  DateTimeStyles.None, out sensorValueReadedDateTime)
                || !Int32.TryParse(splittedArguments[2], out sensorValue))
              {
                _logger.LogWarning("Failed to parse data. Endpoint: {0}\tMicrocontroller: {1}\tRequest: {2}",
                  socket.RemoteEndPoint, connectedMicrocontroller.ID, receivedMessageBuilder.ToString());

                return;
              }

              if (receivedSensorsData.Count(sensorData => sensorData.SensorID == sensorID
                && sensorData.ValueReadTime == sensorValueReadedDateTime) == 0)
              {
                receivedSensorsData.Add(new SensorData
                {
                  MicrocontrollerID = connectedMicrocontroller.ID,
                  SensorID = sensorID,
                  SensorValue = sensorValue,
                  ValueReadTime = sensorValueReadedDateTime,
                  ValueReceivedTime = DateTime.Now
                });

                _logger.LogInformation("Received Data. Endpoint: {0}\tRequest: {1}",
                  socket.RemoteEndPoint, receivedMessageBuilder.ToString());

                if (receivedSensorsData.Count > _bulkInsertSize)
                {
                  using var scope = _serviceScopeFactory.CreateScope();
                  var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                  await unitOfWork.SensorsData.BulkInsert(receivedSensorsData);
                  receivedSensorsData.Clear();

                  _logger.LogInformation("Written data to DB NORMALLY.");
                }
              }

              response = _okResponse;

              break;

            case "STM_RQ":

              if (connectedMicrocontroller is null)
              {
                _logger.LogWarning("Unauthorized. Endpoint: {0}\tRequest: {1}",
                  socket.RemoteEndPoint, receivedMessageBuilder.ToString());

                return;
              }

              using (var scope = _serviceScopeFactory.CreateScope())
              {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                connectedMicrocontroller = await unitOfWork.Microcontrollers.ReloadAsync(connectedMicrocontroller);

                response = connectedMicrocontroller.RequestedSensorID is not null
                  ? $"{_sensorValueResponse}|{connectedMicrocontroller.RequestedSensorID}"
                  : _attentionResponse;
              }

              break;

            default:
              _logger.LogWarning("Unknown command. Endpoint: {0}\tRequest: {1}",
                  socket.RemoteEndPoint, receivedMessageBuilder.ToString());
              return;
          }

          await socket.SendAsync(Encoding.ASCII.GetBytes(response), SocketFlags.None);
        }
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "Endpoint: {0}\tConnected microcontroller: {1}", socket.RemoteEndPoint, connectedMicrocontroller?.ID);
      }
      finally
      {
        if (receivedSensorsData.Count > 0)
        {
          using var scope = _serviceScopeFactory.CreateScope();
          var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
          await unitOfWork.SensorsData.BulkInsert(receivedSensorsData);

          _logger.LogInformation("Written data to DB FINALLY.");
        }

        socket?.Dispose();
      }
    }
  }
}
