using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;

namespace Gss.Core.Helpers
{
  public static class Settings
  {
    public static int MaximumItemsPerPage { get; set; } = 10;
    public static int MinimumItemsPerPage { get; set; } = 2;

    public static class Email
    {
      public static string Address { get; set; }
      public static string Password { get; set; }
      public static string SmtpServer { get; set; }
      public static int SmtpPort { get; set; }
      public static bool SmtpUseSsl { get; set; }
    }

    public static class JWT
    {
      public static string Issuer { get; set; }
      public static string Audience { get; set; }
      public static SymmetricSecurityKey Key { get; set; }

      public static int AccessTokenLifetimeMinutes { get; set; }
      public static int RefreshTokenLifetimeDays { get; set; }
    }

    public static class AzureImages
    {
      public static string AccountName { get; set; }
      public static string AccountKey { get; set; }
      public static string ImagesContainer { get; set; }
      public static string ThumbnailsContainer { get; set; }
      public static List<string> SupportedExtensions { get; set; }
    }
  }
}
