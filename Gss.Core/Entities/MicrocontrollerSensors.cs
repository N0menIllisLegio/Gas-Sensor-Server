using Gss.Core.Interfaces;

namespace Gss.Core.Entities;

public sealed class MicrocontrollerSensors : IEntity
{
    public Guid MicrocontrollerId { get; set; }
    public Microcontroller Microcontroller { get; set; } = null!;

    public Guid SensorId { get; set; }
    public Sensor Sensor { get; set; } = null!;

    public int? CriticalValue { get; set; }
    public Guid Id { get; set; }
}