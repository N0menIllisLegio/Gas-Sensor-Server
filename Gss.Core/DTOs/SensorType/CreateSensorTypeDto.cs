using System.ComponentModel.DataAnnotations;

namespace Gss.Core.DTOs.SensorType
{
  public class CreateSensorTypeDto
  {
    [Required]
    [MaxLength(200)]
    public string Name { get; set; }
    public string Icon { get; set; }

    [MaxLength(20)]
    public string Units { get; set; } // like C, F,Hz, Db ?
  }
}
