using System;
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

namespace Gss.Core.Services
{
  public class SocketConnectionService
  {
    private const char _commandSeparator = '|';
    private const string _okResponse = "Server_OK";
    private const string _attentionResponse = "Server_AT";
    private const string _sensorValueResponse = "Server_SV";

    private readonly IServiceScopeFactory _serviceScopeFactory;

    public SocketConnectionService(IServiceScopeFactory serviceScopeFactory)
    {
      _serviceScopeFactory = serviceScopeFactory;
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
        Console.WriteLine(exception);
      }
      finally
      {
        socket?.Close();
      }
    }

    private async void HandleConnection(Socket socket)
    {
      try
      {
        Microcontroller connectedMicrocontroller = null;

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

          Console.WriteLine(receivedMessageBuilder.ToString()); // TODO temp

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
                // Error - Disconnect
                Console.WriteLine("Invalid status");
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
                Console.WriteLine("Auth failed");
                return;
              }

              response = _okResponse;

              break;

            case "STM_DATA":
              Guid sensorID;
              DateTime sensorValueReadedDateTime;
              int sensorValue;

              if (connectedMicrocontroller is null
                || splittedArguments.Length != 3
                || !Guid.TryParse(splittedArguments[0], out sensorID)
                || !DateTime.TryParse(splittedArguments[1], CultureInfo.CreateSpecificCulture("en-US"),
                  DateTimeStyles.None, out sensorValueReadedDateTime)
                || !Int32.TryParse(splittedArguments[2], out sensorValue))
              {
                // Error - Disconnect
                Console.WriteLine("Invalid status");
                return;
              }

              //_unitOfWork.Data.write
              Console.WriteLine($"{sensorID} | {sensorValueReadedDateTime} | {sensorValue}");

              response = _okResponse;

              break;

            case "STM_RQ":

              if (connectedMicrocontroller is null)
              {
                // Error - Disconnect
                Console.WriteLine("Unknown command");
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
              Console.WriteLine("Unknown command");
              throw new NotSupportedException();
          }

          Console.WriteLine(response); // TODO temp

          await socket.SendAsync(Encoding.ASCII.GetBytes(response), SocketFlags.None);
        }
      }
      catch (Exception exception)
      {
        Console.WriteLine(exception);
      }
      finally
      {
        socket?.Dispose();
      }
    }
  }
}
