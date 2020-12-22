using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gss.Core.Entities
{
  public class SensorType
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
