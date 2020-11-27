using Microsoft.IdentityModel.Tokens;

namespace Gss.Core.Helpers
{
  public static class Settings
  {
    public static class JWT
    {
      public static string Issuer { get; set; }
      public static string Audience { get; set; }
      public static SymmetricSecurityKey Key { get; set; }

      public static int AccessTokenLifetimeMinutes { get; set; }
      public static int RefreshTokenLifetimeDays { get; set; }
    }
  }
}
