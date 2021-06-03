using System;
using System.ComponentModel.DataAnnotations.Schema;
using Gss.Core.Interfaces;

namespace Gss.Core.Entities
{
  public class SensorData : IEntity
  {
    [NotMapped]
    public Guid Id { get; set; }

    public Guid MicrocontrollerID { get; set; }
    public virtual Microcontroller Microcontroller { get; set; }

    public Guid SensorID { get; set; }
    public virtual Sensor Sensor { get; set; }

    public DateTimeOffset ValueReadTime { get; set; }

    public int SensorValue { get; set; }
    public DateTimeOffset ValueReceivedTime { get; set; }
  }
}
