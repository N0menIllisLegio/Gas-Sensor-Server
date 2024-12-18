using Gss.Core.DTOs.Sensor;

namespace Gss.Core.DTOs.Microcontroller;

public class MicrocontrollerSensorDto : SensorDto
{
    public Guid MicrocontrollerSensorId { get; set; }
    public int? CriticalValue { get; set; }
}