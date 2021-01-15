using System;
using System.ComponentModel.DataAnnotations;

namespace Gss.Core.DTOs.User
{
  public class UpdateUserInfoDto
  {
    [Phone]
    public string PhoneNumber { get; set; }

    [Required]
    [StringLength(20, MinimumLength = 2)]
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Gender { get; set; }

    public DateTime? Birthday { get; set; }
  }
}
