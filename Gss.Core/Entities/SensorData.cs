using Gss.Core.Interfaces;

namespace Gss.Core.Entities;

public sealed class SensorData : IEntity
{
  public Guid Id { get; set; }

  public Guid MicrocontrollerId { get; set; }
  public Microcontroller Microcontroller { get; set; } = null!;

  public Guid SensorId { get; set; }
  public Sensor Sensor { get; set; } = null!;

  public DateTimeOffset ValueReadTime { get; set; }

  public int SensorValue { get; set; }
  public DateTimeOffset ValueReceivedTime { get; set; }
}