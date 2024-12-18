using System.Net;
using System.Net.Sockets;
using Gss.Core.Entities;
using Gss.MicrocontrollerListener.Data;
using Gss.Queue.Events;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Gss.MicrocontrollerListener.MicrocontrollerHandlers;

internal sealed class MicrocontrollerListener: BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<MicrocontrollerListener> _logger;
    private readonly IBus _bus;
    private readonly MicrocontrollersConnectionsOptions _options;

    public MicrocontrollerListener(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<MicrocontrollerListener> logger,
        IBus bus,
        IOptions<MicrocontrollersConnectionsOptions> microcontrollersConnectionsOptions)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _bus = bus;
        _options = microcontrollersConnectionsOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(new IPEndPoint(IPAddress.Any, _options.Port));
        socket.Listen(_options.ListenQueue);

        try
        {
            while (true)
            {
                var acceptedSocket = await socket.AcceptAsync(stoppingToken);

                acceptedSocket.ReceiveTimeout = _options.ReceiveTimeout;
                acceptedSocket.SendTimeout = _options.SendTimeout;

                HandleConnection(acceptedSocket, stoppingToken);

                await Task.Delay(100, stoppingToken);
            }
        }
        catch (Exception exception)
        {
            _logger.LogCritical(exception, "{Service} stopped working with error: {ErrorMessage}",
                nameof(MicrocontrollerListener), exception.Message);
        }
        finally
        {
            socket.Close();
        }
    }

    private async void HandleConnection(Socket socket, CancellationToken cancellationToken = default)
    {
        using var connectionManager = new MicrocontrollerConnectionManager(socket, _options, _logger);

        try
        {
            var request = await connectionManager.ReceiveRequestAsync(cancellationToken);

            if (request is null)
                return;

            var authRequest = request.ParseAuthRequest();

            if (authRequest is null)
            {
                _logger.LogWarning("Failed to read auth request. {Endpoint}", socket.RemoteEndPoint);
                return;
            }

            Microcontroller? microcontroller = null;

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var listenerRepository = scope.ServiceProvider.GetRequiredService<IListenerRepository>();

                var connectedMicrocontroller =
                    await listenerRepository.GetMicrocontrollerAsync(authRequest.MicrocontrollerId, cancellationToken);

                if (connectedMicrocontroller is not null)
                {
                    // TODO: HMAC. API-Key or hash
                    if (connectedMicrocontroller.Key == authRequest.Password)
                    {
                        microcontroller = connectedMicrocontroller;
                    }

                    _logger.LogWarning("Failed to authenticate. {MicrocontrollerId}", authRequest.MicrocontrollerId);
                }
                else
                {
                    _logger.LogWarning("Microcontroller not found. {MicrocontrollerId}", authRequest.MicrocontrollerId);
                }
            }

            if (microcontroller is null)
                return;

            using var logScope = _logger.BeginScope(new Dictionary<string, string>()
            {
                { "MicrocontrollerId", microcontroller.Id.ToString() }
            });

            await connectionManager.SendOkAsync(cancellationToken);

            request = await connectionManager.ReceiveRequestAsync(cancellationToken);

            if (request is null)
                return;

            switch (request.Command)
            {
                case MicrocontrollerRequest.DataCommand:
                    var dataRequest = request.ParseDataRequest();

                    if (dataRequest is null)
                    {
                        _logger.LogWarning("Failed to parse data. {Endpoint} --- {MicrocontrollerId}",
                            socket.RemoteEndPoint, microcontroller.Id);

                        return;
                    }

                    await connectionManager.SendOkAsync(cancellationToken);

                    var microcontrollerSensor = microcontroller.MicrocontrollerSensors
                        .FirstOrDefault(ms => ms.Id == dataRequest.MicrocontrollerSensorId);

                    if (microcontrollerSensor is null)
                    {
                        _logger.LogWarning("Sensor ({Id}) doesn't connected to microcontroller ({MicrocontrollerId})",
                            dataRequest.MicrocontrollerSensorId, microcontroller.Id);

                        return;
                    }

                    if (microcontrollerSensor.CriticalValue <= dataRequest.SensorValue)
                    {
                        await _bus.Publish(new CriticalValueReached
                        {
                            MicrocontrollerSensorId = microcontrollerSensor.Id,
                            Value = dataRequest.SensorValue,
                        }, cancellationToken);
                    }

                    await _bus.Publish(new SensorDataReceived
                    {
                        MicrocontrollerSensorId = microcontrollerSensor.Id,
                        ReadTime = DateTime.SpecifyKind(dataRequest.SensorValueReadTime, DateTimeKind.Utc),
                        Value = dataRequest.SensorValue,
                        ReceivedTime = DateTime.UtcNow
                    }, cancellationToken);
                    break;

                case MicrocontrollerRequest.RequestSensorValueCommand:
                    _logger.LogDebug("OBSOLETE MC REQUEST - RequestSensorValueCommand");
                    await connectionManager.SendAttentionAsync(cancellationToken);
                    break;

                case MicrocontrollerRequest.DateSyncCommand:
                    await connectionManager.SendDateTimeAsync(cancellationToken);
                    break;

                default:
                    _logger.LogCritical("Missing implementation for {Command}. {Endpoint} --- {Request}",
                        request.Command, socket.RemoteEndPoint, request);
                    break;
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation cancelled --- {Endpoint}", socket.RemoteEndPoint);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to handle microcontroller connection. {Endpoint}",
                socket.RemoteEndPoint);
        }
    }
}
