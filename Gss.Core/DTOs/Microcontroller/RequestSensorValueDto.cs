namespace Gss.Core.DTOs.Microcontroller;

public class RequestSensorValueDto
{
  public Guid MicrocontrollerId { get; set; }
  public Guid MicrocontrollerSensorId { get; set; }
}