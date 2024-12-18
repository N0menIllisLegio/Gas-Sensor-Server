using Gss.Core.Interfaces.Repositories;
using Gss.MicrocontrollerDataHandler.Email;
using Gss.Queue;
using Gss.Queue.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Gss.MicrocontrollerDataHandler.Consumers;

internal sealed class CriticalValueReachedConsumer: IConsumer<CriticalValueReached>
{
    private readonly IMicrocontrollersRepository _microcontrollersRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<CriticalValueReached> _logger;

    public CriticalValueReachedConsumer(IMicrocontrollersRepository microcontrollersRepository,
        IEmailService emailService, ILogger<CriticalValueReached> logger)
    {
        _microcontrollersRepository = microcontrollersRepository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CriticalValueReached> context)
    {
        var microcontroller =
            await _microcontrollersRepository.FindMicrocontrollerByMicrocontrollerSensorIdAsync(
                context.Message.MicrocontrollerSensorId, context.CancellationToken);

        if (microcontroller == null)
        {
            _logger.LogError("Failed to send Critical Value notification for MicrocontrollerSensorId == {Id}, " +
                             "because entity wasn't found", context.Message.MicrocontrollerSensorId);

            return;
        }

        var microcontrollerSensor =
            microcontroller.MicrocontrollerSensors.First(x => x.Id == context.Message.MicrocontrollerSensorId);

        // TODO: get email by OwnerId from Keycloak
        if (microcontrollerSensor.CriticalValue.HasValue)
            await _emailService.SendCriticalValueEmailAsync("test@test.com", context.Message.Value,
                microcontrollerSensor.CriticalValue ?? -1, microcontroller, microcontrollerSensor.Sensor,
                context.CancellationToken);
    }
}

internal class CriticalValueReachedConsumerDefinition : ConsumerDefinition<CriticalValueReachedConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<CriticalValueReachedConsumer> consumerConfigurator, IRegistrationContext context)
    {
        endpointConfigurator.UseMessageRetry(x => x.Interval(5, 1000));

        if (endpointConfigurator is IRabbitMqReceiveEndpointConfigurator rmq)
        {
            rmq.Bind<CriticalValueReached>(x =>
            {
                x.RoutingKey = RoutingKeys.CriticalValueReachedKey;
            });
        }
    }
}