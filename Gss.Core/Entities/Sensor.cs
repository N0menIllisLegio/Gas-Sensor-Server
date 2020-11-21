using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gss.Core.Entities
{
  public class Sensor
  {
    public Guid ID { get; set; }

    [MaxLength(200)]
    public string Name { get; set; }

    public IList<MicrocontrollerSensor> SensorMicrocontrollers { get; set; }
  }
}
