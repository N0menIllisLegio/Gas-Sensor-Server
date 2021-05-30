using System;

namespace Gss.Core.DTOs.Microcontroller
{
  public class RequestSensorValueDto
  {
    public Guid MicrocontrollerID { get; set; }
    public Guid SensorID { get; set; }
  }
}
