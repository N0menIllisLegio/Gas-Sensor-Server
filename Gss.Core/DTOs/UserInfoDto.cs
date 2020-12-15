using System;
using Gss.Core.Entities;

namespace Gss.Core.DTOs
{
  public record UserInfoDto
  {
    public UserInfoDto(User user)
    {
      ID = user.Id;
      Email = user.Email;
      FirstName = user.FirstName;
      LastName = user.LastName;
      AvatarPath = user.AvatarPath;
      Gender = user.Gender;
      Birthday = user.Birthday;
    }

    public Guid ID { get; init; }
    public string Email { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string AvatarPath { get; init; }
    public string Gender { get; init; }
    public DateTime? Birthday { get; init; }
  }
}
