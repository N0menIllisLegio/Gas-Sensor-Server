using System;

namespace Gss.Core.Models
{
  public class SensorDataModel
  {
    public Guid MicrocontrollerID { get; set; }
    public Guid SensorID { get; set; }
    public DateTime ValueReadTime { get; set; }
    public decimal AverageSensorValue { get; set; }
  }
}
