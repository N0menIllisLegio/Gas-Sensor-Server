using System;

namespace Gss.Core.DTOs.User
{
  public class ExtendedUserInfoDto: UserInfoDto
  {
    public ExtendedUserInfoDto()
    {
    }

    public ExtendedUserInfoDto(Entities.User user)
      : base(user)
    {
      EmailConfirmed = user.EmailConfirmed;
      LockoutEnabled = user.LockoutEnabled;
      LockoutEnd = user.LockoutEnd;
      AccessFailedCount = user.AccessFailedCount;
      CreationDate = user.CreationDate;
    }

    public bool EmailConfirmed { get; init; }
    public bool LockoutEnabled { get; init; }
    public DateTimeOffset? LockoutEnd { get; init; }
    public int AccessFailedCount { get; init; }
    public DateTime CreationDate { get; init; }
  }
}
