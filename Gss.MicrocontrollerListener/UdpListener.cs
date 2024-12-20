using System.Diagnostics;
using System.Globalization;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Gss.Core.Entities;
using Gss.Infrastructure;
using Gss.Queue.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gss.MicrocontrollerListener;

internal sealed class UdpListener: BackgroundService
{
    public static readonly ActivitySource ActivitySource = new("Gss.MicrocontrollerListener.UdpListener", "1.0.0");

    private const string DateTimeFormat = "yyyyMMddTHHmmssZ";

    private readonly IBus _bus;
    private readonly ILogger<UdpListener> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly MicrocontrollersConnectionsOptions _options;

    public UdpListener(IBus bus, ILogger<UdpListener> logger, IServiceScopeFactory serviceScopeFactory,
        IOptions<MicrocontrollersConnectionsOptions> microcontrollersConnectionsOptions)
    {
        _bus = bus;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _options = microcontrollersConnectionsOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var udpClient = new UdpClient(_options.Port);

        _logger.LogTrace("Started UDP client. Listening for connections on {Port}", _options.Port);

        while (true)
        {
            try
            {
                var result = await udpClient.ReceiveAsync(stoppingToken);

                _ = Task.Run(() => ProcessMicrocontrollerMessage(result, stoppingToken), stoppingToken);
            }
            catch (Exception exception)
            {
                if (stoppingToken.IsCancellationRequested)
                    return;

                _logger.LogCritical(exception, "{Service} stopped working with error: {ErrorMessage}",
                    nameof(UdpListener), exception.Message);
            }
        }
    }

    private async Task ProcessMicrocontrollerMessage(UdpReceiveResult result, CancellationToken stoppingToken)
    {
        using Activity? activity = ActivitySource.StartActivity();

        activity?.SetTag("client.address", result.RemoteEndPoint.Address);
        activity?.SetTag("client.port", result.RemoteEndPoint.Port);
        activity?.SetTag("network.peer.address", result.RemoteEndPoint.Address);
        activity?.SetTag("network.peer.port", result.RemoteEndPoint.Port);
        activity?.SetTag("network.transport", "udp");
        activity?.SetTag("server.port", _options.Port);

        var receivedMessage = Encoding.ASCII.GetString(result.Buffer);

        var messageReceivedTime = DateTime.Now;
        var splitMessage = receivedMessage.Split('|');

        if (splitMessage.Length != 5)
        {
            _logger.LogWarning("Invalid count of splits separated by |");
            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }

        if (!DateTime.TryParseExact(splitMessage.First(), DateTimeFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal, out var timestamp) ||
            messageReceivedTime - timestamp > TimeSpan.FromSeconds(_options.MessageTimespanSecondsDiff))
        {
            _logger.LogWarning("Invalid timestamp: {timestamp}. {MicrocontrollerId}", timestamp, splitMessage[2]);
            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }

        if (!Guid.TryParse(splitMessage[2], out var microcontrollerId))
        {
            _logger.LogWarning("Invalid MicrocontrollerId: {Id}", microcontrollerId);
            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }

        Microcontroller microcontroller;

        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var listenerRepository = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var connectedMicrocontroller = await listenerRepository.Microcontrollers
                .Include(x => x.MicrocontrollerSensors)
                .FirstOrDefaultAsync(x => x.Id == microcontrollerId, stoppingToken);

            if (connectedMicrocontroller is null)
            {
                _logger.LogWarning("Microcontroller not found. {MicrocontrollerId}", microcontrollerId);
                activity?.SetStatus(ActivityStatusCode.Error);

                return;
            }

            microcontroller = connectedMicrocontroller;
        }

        if (!ValidateSignature(receivedMessage, splitMessage.Last(), microcontroller.Key))
        {
            _logger.LogWarning("Invalid signature. {MicrocontrollerId}", microcontrollerId);
            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }

        foreach (var data in ReadSensorsData(splitMessage[3], microcontrollerId))
        {
            if (microcontroller.MicrocontrollerSensors
                .Any(x => x.Id == data.MicrocontrollerSensorId && x.CriticalValue <= data.Value))
            {
                // TODO: event?
                // Handle bad inserts into DB (catch and log)
                activity?.SetTag(data.MicrocontrollerSensorId.ToString(), "Critical value reached");


                await _bus.Publish(new CriticalValueReached
                {
                    MicrocontrollerSensorId = data.MicrocontrollerSensorId,
                    Value = (int)data.Value,
                }, stoppingToken);
            }

            await _bus.Publish(new SensorDataReceived
            {
                MicrocontrollerSensorId = data.MicrocontrollerSensorId,
                ReceivedTime = messageReceivedTime,
                ReadTime = data.ReadTime,
                Value = (int)data.Value,
            }, stoppingToken);
        }

        activity?.SetStatus(ActivityStatusCode.Ok);
    }

    private IEnumerable<ReadSensorsData> ReadSensorsData(string sensorsData, Guid microcontrollerId)
    {
        foreach (var sensorData in sensorsData.Split(';'))
        {
            var values = sensorData.Split(',');

            if (!Guid.TryParse(values[0], out var microcontrollerSensorId))
            {
                _logger.LogWarning("Corrupted data: {MicrocontrollerSensorId}. MicrocontrollerId: {Id}.",
                    values[0], microcontrollerId);

                yield break;
            }

            if (!DateTime.TryParseExact(values[1], DateTimeFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal, out var readTime))
            {
                _logger.LogWarning("Corrupted data: {readTime}. MicrocontrollerId: {Id}.",
                    values[1], microcontrollerId);

                yield break;
            }

            if (!double.TryParse(values[2], CultureInfo.InvariantCulture, out var value))
            {
                _logger.LogWarning("Corrupted data: {readTime}. MicrocontrollerId: {Id}.",
                    values[2], microcontrollerId);

                yield break;
            }

            yield return new (microcontrollerSensorId, readTime, value);
        }
    }

    private static bool ValidateSignature(string receivedMessage, string signature, string key)
    {
        var encoding = new ASCIIEncoding();
        var textBytes = encoding.GetBytes(receivedMessage[..(receivedMessage.Length - signature.Length - 1)]);
        var keyBytes = encoding.GetBytes(key);
        byte[] hashBytes;

        using (var hash = new HMACSHA256(keyBytes))
            hashBytes = hash.ComputeHash(textBytes);

        string hmacSignature = BitConverter.ToString(hashBytes)
            .Replace("-", "")
            .ToLower();

        return hmacSignature == signature;
    }
}