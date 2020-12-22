﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gss.Core.Entities
{
  public class Sensor
  {
    public Guid ID { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; }

    [Required]
    public virtual SensorType Type { get; set; }

    public virtual IList<MicrocontrollerSensor> SensorMicrocontrollers { get; set; }
  }
}
