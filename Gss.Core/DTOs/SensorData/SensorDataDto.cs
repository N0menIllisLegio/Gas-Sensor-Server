using System;

namespace Gss.Core.DTOs.SensorData
{
  public class SensorDataDto
  {
    public DateTime WatchingDate { get; set; }
    public DateTime ValueReadTime { get; set; }
    public decimal AverageSensorValue { get; set; }
  }
}
