using Gss.Queue.Events;
using MassTransit;
using RabbitMQ.Client;

namespace Gss.Queue;

public static class ConfigurationExtensions
{
    public static void ConfigureMessageTopology(this IRabbitMqBusFactoryConfigurator configurator)
    {
        configurator.Send<SensorDataReceived>(x =>
        {
            x.UseRoutingKeyFormatter(_ => RoutingKeys.SensorDataReceivedKey);
        });

        configurator.Publish<SensorDataReceived>(x => { x.ExchangeType = ExchangeType.Direct; });

        configurator.Send<CriticalValueReached>(x =>
        {
            x.UseRoutingKeyFormatter(_ => RoutingKeys.CriticalValueReachedKey);
        });

        configurator.Publish<CriticalValueReached>(x => { x.ExchangeType = ExchangeType.Direct; });
    }
}