using Gss.Core.Interfaces;

namespace Gss.Core.Entities;

public sealed class Microcontroller : IEntity
{
  public Guid Id { get; set; }
  public required string Name { get; set; }
  public required string IpAddress { get; set; }
  public DateTimeOffset? LastResponseTime { get; set; }
  public bool Public { get; set; }
  public double? Latitude { get; set; }
  public double? Longitude { get; set; }
  public required string PasswordHash { get; set; }
  public Guid? RequestedSensorId { get; set; }

  // ---- RELATIONSHIPS

  public Guid? OwnerId { get; set; }
  public IList<MicrocontrollerSensors> MicrocontrollerSensors { get; set; } = null!;
}