using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;

namespace Gss.Core.Entities
{
  public class SensorType : IEntity
  {
    [ExpressionsBuilder]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    [ExpressionsBuilder]
    public string Name { get; set; }
    public string Icon { get; set; }

    [MaxLength(20)]
    [ExpressionsBuilder]
    public string Units { get; set; } // like C, F,Hz, Db ?

    public virtual IList<Sensor> Sensors { get; set; }
  }
}
