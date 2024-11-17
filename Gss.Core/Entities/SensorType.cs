using Gss.Core.Helpers;
using Gss.Core.Interfaces;

namespace Gss.Core.Entities;

public class SensorType : IEntity
{
  [ExpressionsBuilder]
  public Guid Id { get; set; }

  [ExpressionsBuilder]
  public required string Name { get; set; }
  public string? Icon { get; set; }

  [ExpressionsBuilder]
  public string? Units { get; set; } // like C, F,Hz, Db ?

  public virtual IList<Sensor> Sensors { get; set; }
}