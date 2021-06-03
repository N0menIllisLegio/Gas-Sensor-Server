using System.ComponentModel.DataAnnotations;

namespace Gss.Core.DTOs
{
  public class EmailDto
  {
    [Required]
    [EmailAddress]
    public string Email { get; set; }
  }
}
