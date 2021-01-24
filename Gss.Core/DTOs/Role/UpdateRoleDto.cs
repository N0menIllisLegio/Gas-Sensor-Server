using System.ComponentModel.DataAnnotations;

namespace Gss.Core.DTOs.Role
{
  public class UpdateRoleDto
  {
    [Required]
    public string Name { get; set; }
  }
}
