using System;
using System.ComponentModel.DataAnnotations;

namespace Gss.Core.DTOs.Microcontroller
{
  public class ChangeMicrocontrollerOwnerDto
  {
    [Required]
    public Guid MicrocontrollerID { get; set; }

    [Required]
    public Guid UserID { get; set; }
  }
}
