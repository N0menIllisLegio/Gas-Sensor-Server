namespace Gss.Core.DTOs.Microcontroller;

public class RequestSensorValueResponseDto
{
  public Guid? PreviousRequestedSensorId { get; set; }
  public Guid CurrentRequestedSensorId { get; set; }
}