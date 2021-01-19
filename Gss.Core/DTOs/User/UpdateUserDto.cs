using System;
using System.ComponentModel.DataAnnotations;

namespace Gss.Core.DTOs.User
{
  public class UpdateUserDto : UpdateUserInfoDto
  {
    [Required]
    public Guid ID { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }
  }
}
