using System;

namespace Gss.Core.DTOs
{
  public record UserInfoDto
  {
    public string Username { get; init; }
    public string Email { get; init; }

    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string AvatarPath { get; init; }
    public string Gender { get; init; }
    public DateTime? Birthday { get; init; }
  }
}
