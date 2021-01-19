using System;

namespace Gss.Core.DTOs.Sensor
{
  public class SensorDto
  {
    public Guid ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    // TODO Add SensorTypeDto
  }
}
