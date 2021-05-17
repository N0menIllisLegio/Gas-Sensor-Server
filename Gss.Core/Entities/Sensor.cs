using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;

namespace Gss.Core.Entities
{
  public class Sensor : IEntity
  {
    [ExpressionsBuilder]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    [ExpressionsBuilder]
    public string Name { get; set; }

    [MaxLength(2000)]
    [ExpressionsBuilder]
    public string Description { get; set; }

    [ExpressionsBuilder]
    public Guid TypeID { get; set; }

    [Required]
    [ExpressionsBuilder]
    public virtual SensorType Type { get; set; }

    public virtual IList<MicrocontrollerSensors> SensorMicrocontrollers { get; set; }
  }
}
