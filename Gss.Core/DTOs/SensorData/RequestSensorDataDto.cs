using System;
using System.Collections.Generic;
using Gss.Core.Enums;

namespace Gss.Core.DTOs.SensorData
{
  public class RequestSensorDataDto
  {
    public Guid MicrocontrollerID { get; set; }
    public Guid SensorID { get; set; }
    public SensorDataPeriod Period { get; set; }
    public List<DateTimeOffset> WatchingDates { get; set; }
  }
}
