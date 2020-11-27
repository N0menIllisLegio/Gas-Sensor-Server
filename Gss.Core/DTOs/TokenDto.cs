using System;

namespace Gss.Core.DTOs
{
  public class TokenDto
  {
    public string AccessToken { get; set; }
    public DateTime AccessTokenExpiration { get; set; }
    public string RefreshToken { get; set; }
  }
}
