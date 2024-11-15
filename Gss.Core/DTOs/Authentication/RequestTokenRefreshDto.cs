namespace Gss.Core.DTOs.Authentication
{
  public class RequestTokenRefreshDto
  {
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
  }
}
