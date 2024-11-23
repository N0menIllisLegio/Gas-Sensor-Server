using System.Net;
using System.Net.Sockets;
using Gss.Core.Entities;
using Gss.MicrocontrollerListener.Data;
using Microsoft.Extensions.Options;

namespace Gss.MicrocontrollerListener.MicrocontrollerHandlers;

internal sealed class MicrocontrollerListener: BackgroundService
{
    private const int ReceivedSensorsDataMaxSize = 5;

    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<MicrocontrollerListener> _logger;
    private readonly MicrocontrollersConnectionsOptions _options;

    private readonly object _locker = new ();
    private readonly List<SensorData> _receivedSensorsData = new ();
    private int _runInsertReceivedSensorsData;

    public MicrocontrollerListener(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<MicrocontrollerListener> logger,
        IOptions<MicrocontrollersConnectionsOptions> microcontrollersConnectionsOptions)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
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

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await BulkInsertReceivedCachedDataAsync();

        await base.StopAsync(cancellationToken);
    }

    private async void HandleConnection(Socket socket, CancellationToken cancellationToken = default)
    {
        using var connectionManager = new MicrocontrollerConnectionManager(socket, _options, _logger);
        using var scope = _serviceScopeFactory.CreateScope();

        var requestsHandler = scope.ServiceProvider.GetRequiredService<IMicrocontrollerRequestsHandler>();

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

            var microcontroller = await requestsHandler.HandleRequestAsync(authRequest, cancellationToken);

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

                    var sensorData = requestsHandler.HandleRequest(dataRequest, microcontroller);

                    if (sensorData is null)
                        return;

                    AddSensorDataToConcurrentList(sensorData);
                    break;

                case MicrocontrollerRequest.RequestSensorValueCommand:
                    if (!microcontroller.RequestedMicrocontrollerSensorId.HasValue)
                    {
                        await connectionManager.SendAttentionAsync(cancellationToken);
                        return;
                    }

                    await connectionManager.SendRequestSensorValueAsync(microcontroller
                        .RequestedMicrocontrollerSensorId.Value, cancellationToken);

                    request = await connectionManager.ReceiveRequestAsync(cancellationToken);

                    if (request is null)
                        return;

                    dataRequest = request.ParseDataRequest();

                    if (dataRequest is null)
                    {
                        _logger.LogWarning("Failed to parse data. {Endpoint} --- {MicrocontrollerId} --- {Request}",
                            socket.RemoteEndPoint, microcontroller.Id, dataRequest);

                        return;
                    }

                    await connectionManager.SendOkAsync(cancellationToken);

                    var newDataEntry = await requestsHandler.HandleRequestGetSensorDataAsync(
                        dataRequest, microcontroller, cancellationToken);

                    AddSensorDataToConcurrentList(newDataEntry);
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

    private void AddSensorDataToConcurrentList(SensorData sensorData)
    {
        bool runBulkInsert;

        lock (_locker)
        {
            _receivedSensorsData.Add(sensorData);

            runBulkInsert = _receivedSensorsData.Count > ReceivedSensorsDataMaxSize;
        }

        if (runBulkInsert)
            _ = Task.Run(BulkInsertReceivedCachedDataAsync);
    }

    private async Task BulkInsertReceivedCachedDataAsync()
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
