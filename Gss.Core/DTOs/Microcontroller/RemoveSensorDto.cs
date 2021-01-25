using System;
using System.ComponentModel.DataAnnotations;

namespace Gss.Core.DTOs.Microcontroller
{
  public class RemoveSensorDto
  {
    [Required]
    public Guid MicrocontollerID { get; set; }

    [Required]
    public Guid SensorID { get; set; }
  }
}
