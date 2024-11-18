using Gss.Core.DTOs.SensorType;

namespace Gss.Core.DTOs.Sensor;

public class SensorDto
{
  public Guid ID { get; set; }
  public required string Name { get; set; }
  public string? Description { get; set; }
  // TODO: public required SensorTypeDto SensorType { get; set; }
}