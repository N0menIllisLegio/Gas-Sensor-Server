using System;

namespace Gss.Core.DTOs.SensorData
{
  public class NotifySensorResponseDto
  {
    public string SensorType { get; set; }
    public string SensorTypeIcon { get; set; }
    public string SensorTypeUnits { get; set; }
    public string SensorName { get; set; }
    public decimal SensorValue { get; set; }
    public Guid MicrocontrollerID { get; set; }
  }
}
