namespace Gss.Core.DTOs.Sensor;

public class UpdateSensorDto
{
  public required string Name { get; set; }
  public string? Description { get; set; }
  public Guid TypeID { get; set; }
}