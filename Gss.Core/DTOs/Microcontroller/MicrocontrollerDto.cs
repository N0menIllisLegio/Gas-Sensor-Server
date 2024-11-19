namespace Gss.Core.DTOs.Microcontroller;

public class MicrocontrollerDto
{
  public Guid Id { get; set; }
  public required string Name { get; set; }
  public DateTimeOffset? LastResponseTime { get; set; }
  public bool Public { get; set; }
  public double? Latitude { get; set; }
  public double? Longitude { get; set; }
  public Guid? RequestedSensorId { get; set; }
  public Guid? OwnerId { get; set; }

  public required IEnumerable<MicrocontrollerSensorDto> Sensors { get; set; }
}