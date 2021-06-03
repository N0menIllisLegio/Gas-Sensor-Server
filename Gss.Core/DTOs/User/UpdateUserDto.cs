using System.ComponentModel.DataAnnotations;

namespace Gss.Core.DTOs.User
{
  public class UpdateUserDto : UpdateUserInfoDto
  {
    [Required]
    [EmailAddress]
    public string Email { get; set; }
  }
}
