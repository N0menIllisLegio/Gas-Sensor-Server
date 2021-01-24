using System;
using System.ComponentModel.DataAnnotations;

namespace Gss.Core.DTOs.Sensor
{
  public class SetSensorTypeDto
  {
    [Required]
    public Guid SensorID { get; set; }

    [Required]
    public Guid SensorTypeID { get; set; }
  }
}
