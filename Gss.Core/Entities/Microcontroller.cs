using Gss.Core.Interfaces;
using Gss.Core.Utils;

namespace Gss.Core.Entities;

public sealed class Microcontroller : IEntity
{
  [AllowOrdering]
  public Guid Id { get; set; }

  [AllowOrdering]
  public required string Name { get; set; }

  [AllowOrdering]
  public bool Public { get; set; }

  [AllowOrdering]
  public double? Latitude { get; set; }

  [AllowOrdering]
  public double? Longitude { get; set; }

  public required string Key { get; set; }

  public Guid? OwnerId { get; set; }
  public IList<MicrocontrollerSensors> MicrocontrollerSensors { get; set; } = null!;
  public IList<Sensor> Sensors { get; set; } = null!;
}