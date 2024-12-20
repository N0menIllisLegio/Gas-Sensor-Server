using System.Diagnostics;
using Gss.Core.Interfaces.Repositories;
using Gss.MicrocontrollerDataHandler.Email;
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
    private readonly IMicrocontrollersRepository _microcontrollersRepository;

    public CriticalValueReachedConsumer(IMicrocontrollersRepository microcontrollersRepository,
        IEmailService emailService, ILogger<CriticalValueReached> logger)
    {
        _microcontrollersRepository = microcontrollersRepository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CriticalValueReached> context)
    {
        using Activity? activity = ActivitySource.StartActivity();
        activity?.SetTag("MicrocontrollerSensorId", context.Message.MicrocontrollerSensorId);

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

        var microcontrollerSensor =
            microcontroller.MicrocontrollerSensors.First(x => x.Id == context.Message.MicrocontrollerSensorId);

        // TODO: get email by OwnerId from Keycloak
        if (microcontrollerSensor.CriticalValue.HasValue && microcontrollerSensor.CriticalValue < context.Message.Value)
        {
            await _emailService.SendCriticalValueEmailAsync("test@test.com", context.Message.Value,
                microcontrollerSensor.CriticalValue ?? -1, microcontroller, microcontrollerSensor.Sensor,
                context.CancellationToken);
        }
        else
        {
            _logger.LogWarning("Critical value was changed before email was sent. " +
                               "Value = {ReportedValue}, Current Critical Value = {CriticalValue}",
                context.Message.Value, microcontrollerSensor.CriticalValue);

            activity?.SetStatus(ActivityStatusCode.Error);
        }
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