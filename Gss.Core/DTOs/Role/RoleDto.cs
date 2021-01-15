using System.ComponentModel.DataAnnotations;

namespace Gss.Core.DTOs.Role
{
  public class RoleDto
  {
    [Required]
    [MinLength(3, ErrorMessage = "Role length can't be less than 3.")]
    public string Name { get; set; }
  }
}
