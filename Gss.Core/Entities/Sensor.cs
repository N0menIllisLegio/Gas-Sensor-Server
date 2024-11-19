using Gss.Core.Interfaces;
using Gss.Core.Utils;

namespace Gss.Core.Entities;

public sealed class Sensor : IEntity
{
  [AllowOrdering]
  public Guid Id { get; set; }

  [AllowOrdering]
  public required string Name { get; set; }
  public string? Description { get; set; }
  public Guid TypeId { get; set; }
  public SensorType Type { get; set; } = null!;

  public IList<MicrocontrollerSensors> SensorMicrocontrollers { get; set; } = null!;
  public IList<Microcontroller> Microcontrollers { get; set; } = null!;
}