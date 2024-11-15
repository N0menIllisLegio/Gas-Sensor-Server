namespace Gss.Core.DTOs.Microcontroller
{
  public class SetSensorsCriticalValueDto
  {
    public Guid MicrocontrollerID { get; set; }
    public Guid SensorID { get; set; }

    public int? CriticalValue { get; set; }
  }
}
