using Microsoft.IdentityModel.Tokens;

namespace Gss.Core.Helpers
{
  public static class Settings
  {
    public static string JwtIssuer { get; set; }
    public static string JwtAudience { get; set; }
    public static SymmetricSecurityKey JwtKey { get; set; }

    public static int JwtAccessTokenLifetimeMinutes { get; set; }
    public static int JwtRefreshTokenLifetimeDays { get; set; }
  }
}
