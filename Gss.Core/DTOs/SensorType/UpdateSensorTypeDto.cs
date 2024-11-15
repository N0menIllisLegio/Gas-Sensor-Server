namespace Gss.Core.DTOs.SensorType
{
  public class UpdateSensorTypeDto
  {
    public required string Name { get; set; }
    public string? Icon { get; set; }
    public string? Units { get; set; }
  }
}
