namespace Gss.Queue.Events;

public record CriticalValueReached
{
    public int Value { get; init; }
    public Guid MicrocontrollerSensorId { get; init; }
}