using System.Diagnostics;
using Gss.Core.Interfaces.Repositories;
using Gss.MicrocontrollerDataHandler.Email;
using Gss.MicrocontrollerDataHandler.Keycloak;
using Gss.Queue;
using Gss.Queue.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Gss.MicrocontrollerDataHandler.Consumers;

internal sealed class CriticalValueReachedConsumer : IConsumer<CriticalValueReached>
{
    public static readonly ActivitySource ActivitySource =
        new("Gss.MicrocontrollerDataHandler.Consumers.CriticalValueReachedConsumer", "1.0.0");

    private readonly IEmailService _emailService;
    private readonly ILogger<CriticalValueReached> _logger;
    private readonly KeycloakHttpClient _keycloakHttpClient;
    private readonly IMicrocontrollersRepository _microcontrollersRepository;

    public CriticalValueReachedConsumer(IMicrocontrollersRepository microcontrollersRepository,
        IEmailService emailService, ILogger<CriticalValueReached> logger, KeycloakHttpClient keycloakHttpClient)
    {
        _microcontrollersRepository = microcontrollersRepository;
        _keycloakHttpClient = keycloakHttpClient;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CriticalValueReached> context)
    {
        using Activity? activity = ActivitySource.StartActivity();
        activity?.SetTag("MicrocontrollerSensorId", context.Message.MicrocontrollerSensorId);
        activity?.SetTag("CriticalValue", context.Message.Value);

        var microcontroller =
            await _microcontrollersRepository.FindMicrocontrollerByMicrocontrollerSensorIdAsync(
                context.Message.MicrocontrollerSensorId, context.CancellationToken);

        if (microcontroller == null)
        {
            _logger.LogError(
                "Failed to send Critical Value notification for MicrocontrollerSensorId, because entity wasn't found");

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }

        activity?.SetTag("MicrocontrollerId", microcontroller.Id);

        var microcontrollerSensor =
            microcontroller.MicrocontrollerSensors.First(x => x.Id == context.Message.MicrocontrollerSensorId);

        if (!microcontrollerSensor.CriticalValue.HasValue ||
            microcontrollerSensor.CriticalValue > context.Message.Value)
        {
            _logger.LogWarning("Critical value was changed before email was sent. Value = {ReportedValue}",
                context.Message.Value);

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }

        if (!microcontroller.OwnerId.HasValue)
        {
            _logger.LogWarning("Critical Value wasn't sent because Microcontroller is without OwnerId");

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }

        activity?.SetTag("OwnerId", microcontroller.OwnerId);

        var owner = await _keycloakHttpClient.GetUserAsync(microcontroller.OwnerId.Value, context.CancellationToken);

        if (!owner.EmailVerified)
        {
            _logger.LogWarning("Critical Value wasn't sent because Owner isn't verified email.");

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }

        if (!microcontrollerSensor.CriticalValueLastNotified.HasValue ||
            DateTime.Now - microcontrollerSensor.CriticalValueLastNotified > TimeSpan.FromHours(1))
        {
            await _emailService.SendCriticalValueEmailAsync(owner.Email, context.Message.Value,
                microcontrollerSensor.CriticalValue.Value, microcontroller, microcontrollerSensor.Sensor,
                context.CancellationToken);

            await _microcontrollersRepository.SetCriticalValueLastNotifiedAsync(microcontrollerSensor.Id,
                DateTime.UtcNow, context.CancellationToken);
        }
        else
        {
            _logger.LogInformation("Email was throttled. Previous was sent at: {LastNotificationTime}",
                microcontrollerSensor.CriticalValueLastNotified);
        }

        activity?.SetStatus(ActivityStatusCode.Ok);
    }
}

internal class CriticalValueReachedConsumerDefinition : ConsumerDefinition<CriticalValueReachedConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<CriticalValueReachedConsumer> consumerConfigurator, IRegistrationContext context)
    {
        endpointConfigurator.UseMessageRetry(x => x.Interval(5, 1000));

        if (endpointConfigurator is IRabbitMqReceiveEndpointConfigurator rmq)
            rmq.Bind<CriticalValueReached>(x => { x.RoutingKey = RoutingKeys.CriticalValueReachedKey; });
    }
}