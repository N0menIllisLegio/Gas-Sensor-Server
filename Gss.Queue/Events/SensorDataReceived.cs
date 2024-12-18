namespace Gss.Queue.Events;

public record SensorDataReceived
{
    public Guid MicrocontrollerSensorId { get; init; }
    public DateTimeOffset ReadTime { get; init; }
    public int Value { get; init; }
    public DateTimeOffset ReceivedTime { get; init; }
}