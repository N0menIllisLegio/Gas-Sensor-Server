using System;

namespace Gss.Core.Models
{
  public class SensorDataModel
  {
    public Guid MicrocontrollerID { get; set; }
    public Guid SensorID { get; set; }
    public DateTimeOffset ValueReadTime { get; set; }
    public decimal AverageSensorValue { get; set; }
  }
}
