namespace Gss.Core.DTOs.SensorData;

public class NotifySensorResponseDto
{
  public required string SensorType { get; set; }
  public required string? SensorTypeIcon { get; set; }
  public required string? SensorTypeUnits { get; set; }
  public required string SensorName { get; set; }
  public decimal SensorValue { get; set; }
  public Guid MicrocontrollerSensorId { get; set; }
}