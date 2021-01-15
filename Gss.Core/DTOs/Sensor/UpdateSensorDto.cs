using System;

namespace Gss.Core.DTOs.Sensor
{
  public class UpdateSensorDto
  {
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid Type { get; set; }
  }
}
