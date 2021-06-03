using System;
using System.Collections.Generic;
using Gss.Core.DTOs.SensorType;

namespace Gss.Core.DTOs.Microcontroller
{
  public class MapMicrocontrollerDto
  {
    public Guid MicrocontrollerID { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public List<SensorTypeDto> SensorTypes { get; set; }
  }
}
