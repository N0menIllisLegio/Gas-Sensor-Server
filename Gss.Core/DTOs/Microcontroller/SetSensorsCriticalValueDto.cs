namespace Gss.Core.DTOs.Microcontroller;

public class SetSensorsCriticalValueDto
{
    public Guid MicrocontrollerSensorId { get; set; }
    public int? CriticalValue { get; set; }
}