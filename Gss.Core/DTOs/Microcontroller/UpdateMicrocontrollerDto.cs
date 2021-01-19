using System;
using System.ComponentModel.DataAnnotations;

namespace Gss.Core.DTOs.Microcontroller
{
  public class UpdateMicrocontrollerDto : CreateMicrocontrollerDto
  {
    [Required]
    public Guid ID { get; set; }
  }
}
