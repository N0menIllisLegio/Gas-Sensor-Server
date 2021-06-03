using System;
using System.ComponentModel.DataAnnotations;

namespace Gss.Core.DTOs.Authentication
{
  public class ChangePasswordDto
  {
    [Required]
    public Guid UserID { get; set; }

    [Required]
    public string Token { get; set; }

    [Required]
    [StringLength(20, MinimumLength = 4)]
    public string Password { get; set; }

    [Required]
    [Compare("Password")]
    [StringLength(20, MinimumLength = 4)]
    public string ConfirmPassword { get; set; }
  }
}
