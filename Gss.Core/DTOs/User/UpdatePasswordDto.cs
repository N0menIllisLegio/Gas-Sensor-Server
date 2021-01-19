using System;
using System.ComponentModel.DataAnnotations;

namespace Gss.Core.DTOs.User
{
  public class UpdatePasswordDto
  {
    [Required]
    public Guid UserID { get; set; }

    [Required]
    [StringLength(20, MinimumLength = 4)]
    public string NewPassword { get; set; }
  }
}
