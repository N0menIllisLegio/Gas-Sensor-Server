using Gss.Core.Interfaces;

namespace Gss.Core.Entities;

public sealed class Sensor : IEntity
{
  public Guid Id { get; set; }
  public required string Name { get; set; }
  public string? Description { get; set; }
  public Guid TypeId { get; set; }
  public SensorType Type { get; set; } = null!;

  public IList<MicrocontrollerSensors> SensorMicrocontrollers { get; set; } = null!;
  public IList<Microcontroller> Microcontrollers { get; set; } = null!;
}