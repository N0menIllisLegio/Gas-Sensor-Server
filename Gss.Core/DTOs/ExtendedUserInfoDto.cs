using System;
using Gss.Core.Entities;

namespace Gss.Core.DTOs
{
  public record ExtendedUserInfoDto : UserInfoDto
  {
    public ExtendedUserInfoDto(User user)
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
