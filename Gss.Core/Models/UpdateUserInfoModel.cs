using System;

namespace Gss.Core.Models
{
  public class UpdateUserInfoModel
  {
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Gender { get; set; }
    public DateTime? Birthday { get; set; }
  }
}
