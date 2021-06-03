using System.ComponentModel.DataAnnotations;

namespace Gss.Core.DTOs.SensorType
{
  public class UpdateSensorTypeDto
  {
    [Required]
    [MaxLength(200)]
    public string Name { get; set; }
    public string Icon { get; set; }

    [MaxLength(20)]
    public string Units { get; set; }
  }
}
