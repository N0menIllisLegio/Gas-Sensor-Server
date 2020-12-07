using System.ComponentModel.DataAnnotations;

namespace Gss.Core.DTOs
{
  public class ChangePasswordDto
  {
    [Required]
    [StringLength(20, MinimumLength = 4)]
    public string Password { get; set; }

    [Required]
    [Compare("Password")]
    [StringLength(20, MinimumLength = 4)]
    public string ConfirmPassword { get; set; }
  }
}
