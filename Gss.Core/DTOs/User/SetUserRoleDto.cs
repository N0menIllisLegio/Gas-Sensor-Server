using System;
using System.ComponentModel.DataAnnotations;

namespace Gss.Core.DTOs.User
{
  public class SetUserRoleDto
  {
    [Required]
    public Guid UserID { get; set; }

    [Required]
    public string RoleName { get; set; }
  }
}
