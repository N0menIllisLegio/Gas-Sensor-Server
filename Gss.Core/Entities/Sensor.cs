using Gss.Core.Helpers;
using Gss.Core.Interfaces;

namespace Gss.Core.Entities;

public class Sensor : IEntity
{
  [ExpressionsBuilder]
  public Guid Id { get; set; }

  [ExpressionsBuilder]
  public required string Name { get; set; }

  [ExpressionsBuilder]
  public string? Description { get; set; }

  [ExpressionsBuilder]
  public Guid TypeID { get; set; }

  [ExpressionsBuilder]
  public virtual SensorType Type { get; set; }

  public virtual IList<MicrocontrollerSensors> SensorMicrocontrollers { get; set; }
}