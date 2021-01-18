using System;

namespace Gss.Core.DTOs.User
{
  public class UserDto
  {
    public Guid ID { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string AvatarPath { get; set; }
    public string Gender { get; set; }
    public DateTime? Birthday { get; set; }
  }
}
