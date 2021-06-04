using System;
using System.ComponentModel.DataAnnotations;

namespace Gss.Core.DTOs.Microcontroller
{
  public class SetSensorsCriticalValueDto
  {
    [Required]
    public Guid MicrocontrollerID { get; set; }

    [Required]
    public Guid SensorID { get; set; }

    public int? CriticalValue { get; set; }
  }
}
