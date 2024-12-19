namespace Gss.Core.DTOs.Microcontroller;

public class ExtendedMicrocontrollerDto: MicrocontrollerDto
{
    public DateTimeOffset? LatestResponse { get; set; }
}