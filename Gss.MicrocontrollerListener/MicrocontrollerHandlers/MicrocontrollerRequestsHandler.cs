using Gss.Core.Entities;
using Gss.MicrocontrollerListener.Data;
using Gss.MicrocontrollerListener.Email;
using Gss.MicrocontrollerListener.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace Gss.MicrocontrollerListener.MicrocontrollerHandlers;

internal sealed class MicrocontrollerRequestsHandler : IMicrocontrollerRequestsHandler
{
    private readonly IListenerRepository _listenerRepository;
    private readonly ILogger<MicrocontrollerListener> _logger;
    private readonly IEmailService _emailService;
    private readonly IHubContext<NotificationsHub> _notificationHub;

    public MicrocontrollerRequestsHandler(IListenerRepository listenerRepository, ILogger<MicrocontrollerListener> logger,
        IEmailService emailService, IHubContext<NotificationsHub> notificationHub)
    {
        _listenerRepository = listenerRepository;
        _logger = logger;
        _emailService = emailService;
        _notificationHub = notificationHub;
    }

    public async Task<Microcontroller?> HandleRequestAsync(AuthRequest authRequest,
        CancellationToken cancellationToken = default)
    {
        var connectedMicrocontroller =
            await _listenerRepository.GetMicrocontrollerAsync(authRequest.MicrocontrollerId, cancellationToken);

        if (connectedMicrocontroller is not null)
        {
            // TODO: HMAC. API-Key or hash
            if (connectedMicrocontroller.Key == authRequest.Password)
            {
                await _listenerRepository.UpdateLastResponseTimeAsync(connectedMicrocontroller.Id, cancellationToken);

                return connectedMicrocontroller;
            }

            _logger.LogWarning("Failed to authenticate. {MicrocontrollerId}", authRequest.MicrocontrollerId);
        }
        else
        {
            _logger.LogWarning("Microcontroller not found. {MicrocontrollerId}", authRequest.MicrocontrollerId);
        }

        return null;
    }

    public SensorData? HandleRequest(DataRequest dataRequest, Microcontroller connectedMicrocontroller,
        CancellationToken cancellationToken = default)
    {
        var microcontrollerSensor = connectedMicrocontroller.MicrocontrollerSensors
            .FirstOrDefault(ms => ms.Id == dataRequest.MicrocontrollerSensorId);

        if (microcontrollerSensor is null)
        {
            _logger.LogWarning("Sensor ({Id}) doesn't connected to microcontroller ({MicrocontrollerId})",
                dataRequest.MicrocontrollerSensorId, connectedMicrocontroller.Id);

            return null;
        }

        if (microcontrollerSensor.CriticalValue <= dataRequest.SensorValue)
        {
            // TODO: message queue
            _ = _emailService.SendCriticalValueEmailAsync(dataRequest.SensorValue,
                microcontrollerSensor.CriticalValue.Value, connectedMicrocontroller, microcontrollerSensor.Sensor,
                default);
        }

        return new SensorData
        {
            MicrocontrollerSensorId = microcontrollerSensor.Id,
            ReadTime = DateTime.SpecifyKind(dataRequest.SensorValueReadTime, DateTimeKind.Utc),
            Value = dataRequest.SensorValue,
            ReceivedTime = DateTime.UtcNow
        };
    }

    public async Task<SensorData> HandleRequestGetSensorDataAsync(DataRequest dataRequest,
        Microcontroller connectedMicrocontroller, CancellationToken cancellationToken = default)
    {
        var newDataEntry = new SensorData
        {
            MicrocontrollerSensorId = connectedMicrocontroller.RequestedMicrocontrollerSensorId!.Value,
            ReadTime = DateTime.SpecifyKind(dataRequest.SensorValueReadTime, DateTimeKind.Utc),
            Value = dataRequest.SensorValue,
            ReceivedTime = DateTime.UtcNow
        };

        var requestedMicrocontrollerSensor = connectedMicrocontroller.MicrocontrollerSensors.First(
            x => x.Id == connectedMicrocontroller.RequestedMicrocontrollerSensorId);

        await _listenerRepository.ResetMicrocontrollerRequestSensorValueAsync(
            connectedMicrocontroller.Id, cancellationToken);

        await _notificationHub.Clients.User(connectedMicrocontroller.OwnerId!.Value.ToString())
            .SendAsync("Notification", new NotifySensorResponseDto
            {
                MicrocontrollerSensorId = requestedMicrocontrollerSensor.Id,
                SensorName = requestedMicrocontrollerSensor.Sensor.Name,
                SensorType = requestedMicrocontrollerSensor.Sensor.Type.Name,
                SensorValue = dataRequest.SensorValue,
                SensorTypeIcon = requestedMicrocontrollerSensor.Sensor.Type.Icon,
                SensorTypeUnits = requestedMicrocontrollerSensor.Sensor.Type.Units
            }, cancellationToken: cancellationToken);

        return newDataEntry;
    }
}
