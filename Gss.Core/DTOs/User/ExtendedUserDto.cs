using System;

namespace Gss.Core.DTOs.User
{
  public class ExtendedUserDto: UserDto
  {
    public bool EmailConfirmed { get; set; }
    public bool LockoutEnabled { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
    public int AccessFailedCount { get; set; }
    public DateTime CreationDate { get; set; }
  }
}
