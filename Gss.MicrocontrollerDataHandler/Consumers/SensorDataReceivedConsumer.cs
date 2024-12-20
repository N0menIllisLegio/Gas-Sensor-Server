using System.Diagnostics;
using Gss.Core.Entities;
using Gss.Infrastructure;
using Gss.Queue;
using Gss.Queue.Events;
using MassTransit;
using RabbitMQ.Client;

namespace Gss.MicrocontrollerDataHandler.Consumers;

internal sealed class SensorDataReceivedConsumer : IConsumer<Batch<SensorDataReceived>>
{
    public static readonly ActivitySource ActivitySource =
        new("Gss.MicrocontrollerDataHandler.Consumers.SensorDataReceivedConsumer", "1.0.0");

    private readonly AppDbContext _appDbContext;

    public SensorDataReceivedConsumer(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task Consume(ConsumeContext<Batch<SensorDataReceived>> context)
    {
        using Activity? activity = ActivitySource.StartActivity();
        activity?.SetTag("BatchSize", context.Message.Length);

        var dataForInsertion = new List<SensorData>();

        foreach (var group in context.Message.GroupBy(data => new
                 {
                     data.Message.MicrocontrollerSensorId,
                     data.Message.ReadTime
                 }))
        {
            var insertingValue = group.First().Message;

            dataForInsertion.Add(new SensorData
            {
                MicrocontrollerSensorId = insertingValue.MicrocontrollerSensorId,
                ReadTime = insertingValue.ReadTime,
                Value = insertingValue.Value,
                ReceivedTime = insertingValue.ReceivedTime
            });
        }

        activity?.SetTag("GroupedBatchSize", dataForInsertion.Count);

        await _appDbContext.SensorsData.BulkInsertAsync(dataForInsertion, options =>
        {
            options.AutoMapOutputDirection = false;
            options.InsertIfNotExists = true;
        }, context.CancellationToken);

        activity?.SetStatus(ActivityStatusCode.Ok);
    }
}

internal class SensorDataReceivedConsumerDefinition : ConsumerDefinition<SensorDataReceivedConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<SensorDataReceivedConsumer> consumerConfigurator, IRegistrationContext context)
    {
        endpointConfigurator.UseMessageRetry(x => x.Interval(5, 1000));
        endpointConfigurator.PrefetchCount = 50;
        endpointConfigurator.Batch<SensorDataReceived>(x =>
        {
            x.ConcurrencyLimit = 1;
            x.MessageLimit = 50;
            x.TimeLimit = TimeSpan.FromSeconds(5);
        });

        if (endpointConfigurator is IRabbitMqReceiveEndpointConfigurator rmq)
            rmq.Bind<SensorDataReceived>(x =>
            {
                x.RoutingKey = RoutingKeys.SensorDataReceivedKey;
                x.ExchangeType = ExchangeType.Direct;
            });
    }
}