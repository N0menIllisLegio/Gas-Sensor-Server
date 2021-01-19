using System;
using System.ComponentModel.DataAnnotations;

namespace Gss.Core.DTOs.Role
{
  public class UpdateRoleDto
  {
    [Required]
    public Guid ID { get; set; }

    [Required]
    public string Name { get; set; }
  }
}
