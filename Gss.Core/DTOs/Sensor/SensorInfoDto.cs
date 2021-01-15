using System;

namespace Gss.Core.DTOs.Sensor
{
  public class SensorInfoDto
  {
    public Guid ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    // TODO Add SensorTypeDto
  }
}
