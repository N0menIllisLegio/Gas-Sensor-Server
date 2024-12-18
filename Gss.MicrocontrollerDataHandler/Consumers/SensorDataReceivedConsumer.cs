using Gss.Core.Entities;
using Gss.Infrastructure;
using Gss.Queue;
using Gss.Queue.Events;
using MassTransit;
using RabbitMQ.Client;

namespace Gss.MicrocontrollerDataHandler.Consumers;

internal sealed class SensorDataReceivedConsumer: IConsumer<Batch<SensorDataReceived>>
{
    private readonly AppDbContext _appDbContext;
    private readonly ILogger<SensorDataReceivedConsumer> _logger;

    public SensorDataReceivedConsumer(AppDbContext appDbContext, ILogger<SensorDataReceivedConsumer> logger)
    {
        _appDbContext = appDbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<Batch<SensorDataReceived>> context)
    {
        _logger.LogTrace("Consumed sensor data batch: {MessageId}. Length = {Length}",
            context.MessageId, context.Message.Length);

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

        await _appDbContext.SensorsData.BulkInsertAsync(dataForInsertion, options =>
        {
            options.AutoMapOutputDirection = false;
            options.InsertIfNotExists = true;
        }, context.CancellationToken);

        _logger.LogTrace("Processed sensor data batch: {MessageId}", context.MessageId);
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
        {
            rmq.Bind<SensorDataReceived>(x =>
            {
                x.RoutingKey = RoutingKeys.SensorDataReceivedKey;
                x.ExchangeType = ExchangeType.Direct;
            });
        }
    }
}
