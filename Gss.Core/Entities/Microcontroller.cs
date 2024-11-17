using Gss.Core.Helpers;
using Gss.Core.Interfaces;

namespace Gss.Core.Entities;

public class Microcontroller : IEntity
{
  [ExpressionsBuilder]
  public Guid Id { get; set; }

  [ExpressionsBuilder]
  public required string Name { get; set; }
  public required string IPAddress { get; set; }
  public DateTimeOffset? LastResponseTime { get; set; }

  [ExpressionsBuilder]
  public bool Public { get; set; }
  public double? Latitude { get; set; }
  public double? Longitude { get; set; }
  public required string PasswordHash { get; set; }
  public Guid? RequestedSensorID { get; set; }

  // ---- RELATIONSHIPS

  public Guid? OwnerId { get; set; }
  public virtual IList<MicrocontrollerSensors> MicrocontrollerSensors { get; set; }
}