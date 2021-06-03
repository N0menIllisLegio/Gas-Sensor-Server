using System.ComponentModel.DataAnnotations;

namespace Gss.Core.DTOs.Authentication
{
  public class LoginDto
  {
    [Required]
    public string Login { get; set; }

    [Required]
    public string Password { get; set; }
  }
}
