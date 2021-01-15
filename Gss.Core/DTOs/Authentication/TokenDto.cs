using System;

namespace Gss.Core.DTOs.Authentication
{
  public class TokenDto
  {
    public string AccessToken { get; init; }
    public DateTime AccessTokenExpiration { get; init; }
    public string RefreshToken { get; init; }
    public Guid UserID { get; init; }
  }
}
