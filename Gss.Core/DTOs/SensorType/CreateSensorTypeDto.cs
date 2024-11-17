namespace Gss.Core.DTOs.SensorType;

public class CreateSensorTypeDto
{
  public required string Name { get; set; }
  public string? Icon { get; set; }
  public string? Units { get; set; } // like C, F,Hz, Db ?
}