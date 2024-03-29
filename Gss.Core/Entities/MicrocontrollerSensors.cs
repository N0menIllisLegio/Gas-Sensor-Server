﻿using System;
using System.ComponentModel.DataAnnotations.Schema;
using Gss.Core.Interfaces;

namespace Gss.Core.Entities
{
  public class MicrocontrollerSensors : IEntity
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public Guid MicrocontrollerID { get; set; }
    public virtual Microcontroller Microcontroller { get; set; }

    public Guid SensorID { get; set; }
    public virtual Sensor Sensor { get; set; }

    public int? CriticalValue { get; set; }
  }
}
