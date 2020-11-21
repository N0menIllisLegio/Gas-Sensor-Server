using System.ComponentModel.DataAnnotations;

namespace Gss.Web.ViewModels
{
  public class RoleViewModel
  {
    [Required]
    [MinLength(3, ErrorMessage = "Role length can't be less than 3.")]
    public string Name { get; set; }
  }
}
