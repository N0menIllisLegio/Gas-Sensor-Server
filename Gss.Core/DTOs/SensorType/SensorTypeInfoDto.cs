using System;

namespace Gss.Core.DTOs.SensorType
{
  public class SensorTypeInfoDto
  {
    public SensorTypeInfoDto(Entities.SensorType sensorType)
    {
      ID = sensorType.ID;
      Name = sensorType.Name;
      Icon = sensorType.Icon;
      Units = sensorType.Units;
    }

    public Guid ID { get; init; }
    public string Name { get; init; }
    public string Icon { get; init; }
    public string Units { get; init; }
  }
}
