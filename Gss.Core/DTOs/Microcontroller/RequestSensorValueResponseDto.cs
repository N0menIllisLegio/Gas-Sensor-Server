using System;

namespace Gss.Core.DTOs.Microcontroller
{
  public class RequestSensorValueResponseDto
  {
    public Guid? PreviousRequestedSensorID { get; set; }
    public Guid CurrentRequestedSensorID { get; set; }
  }
}
