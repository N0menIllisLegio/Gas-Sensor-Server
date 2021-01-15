﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Gss.Core.Interfaces;

namespace Gss.Core.Entities
{
  public class SensorType : IEntity
  {
    public Guid ID { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; }
    public string Icon { get; set; }

    [MaxLength(20)]
    public string Units { get; set; } // like C, F,Hz, Db ?

    public virtual IList<Sensor> Sensors { get; set; }
  }
}
