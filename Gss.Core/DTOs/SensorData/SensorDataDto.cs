using System;

namespace Gss.Core.DTOs.SensorData
{
  public class SensorDataDto
  {
    public DateTimeOffset WatchingDate { get; set; }
    public DateTimeOffset ValueReadTime { get; set; }
    public decimal AverageSensorValue { get; set; }
  }
}
