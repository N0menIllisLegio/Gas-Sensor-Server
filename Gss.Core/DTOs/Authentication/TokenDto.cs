namespace Gss.Core.DTOs.Authentication
{
  public class TokenDto
  {
    public required string AccessToken { get; init; }
    public DateTimeOffset AccessTokenExpiration { get; init; }
    public required string RefreshToken { get; init; }
    public Guid UserID { get; init; }
    public bool Administrator { get; init; }
  }
}
