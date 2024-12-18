namespace Gss.Core.Entities;

public sealed class SensorData
{
    public Guid MicrocontrollerSensorId { get; set; }
    public DateTimeOffset ReadTime { get; set; }

    public int Value { get; set; }
    public DateTimeOffset ReceivedTime { get; set; }
}