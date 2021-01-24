using System;
using Gss.Core.Interfaces;

namespace Gss.Core.Entities
{
  public class MicrocontrollerSensors : IEntity
  {
    public Guid ID { get; set; }

    public Guid MicrocontrollerID { get; set; }
    public virtual Microcontroller Microcontroller { get; set; }

    public Guid SensorID { get; set; }
    public virtual Sensor Sensor { get; set; }
  }
}
