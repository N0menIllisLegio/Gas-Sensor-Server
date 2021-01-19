using System;
using System.ComponentModel.DataAnnotations;

namespace Gss.Core.DTOs.Sensor
{
  public class CreateSensorDto
  {
    [Required]
    [MaxLength(200)]
    public string Name { get; set; }

    [MaxLength(1800)]
    public string Description { get; set; }

    [Required]
    public Guid TypeID { get; set; }
  }
}
