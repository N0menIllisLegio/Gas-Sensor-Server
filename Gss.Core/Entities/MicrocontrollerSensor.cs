using System;

namespace Gss.Core.Entities
{
  public class MicrocontrollerSensor
  {
    public Guid ID { get; set; }

    public Guid MicrocontrollerID { get; set; }
    public virtual Microcontroller Microcontroller { get; set; }

    public Guid SensorID { get; set; }
    public virtual Sensor Sensor { get; set; }
  }
}
