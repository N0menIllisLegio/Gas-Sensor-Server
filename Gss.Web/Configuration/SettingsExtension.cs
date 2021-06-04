using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Gss.Core.Helpers;
using Gss.Core.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Gss.Web.Configuration
{
  internal static class SettingsExtension
  {
    public static void ConfigureSettings(this IConfiguration configuration)
    {
      var jwtSection = configuration.GetSection("Authentication:JWT");

      Settings.JWT.Issuer = jwtSection["Issuer"];
      Settings.JWT.Audience = jwtSection["Audience"];
      Settings.JWT.Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]));
      Settings.JWT.AccessTokenLifetimeMinutes = Int32.Parse(jwtSection["AccessTokenLifetimeMinutes"]);
      Settings.JWT.RefreshTokenLifetimeDays = Int32.Parse(jwtSection["RefreshTokenLifetimeDays"]);

      var emailSection = configuration.GetSection("Email");
      string emailRegex = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*"
        + @"@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";

      Settings.Email.Address = emailSection["Address"];
      Settings.Email.Password = emailSection["Password"];
      Settings.Email.SmtpServer = emailSection["SmtpServer"];

      if (Int32.TryParse(emailSection["SmtpPort"], out int port)
        && Regex.IsMatch(Settings.Email.Address, emailRegex, RegexOptions.IgnoreCase))
      {
        Settings.Email.SmtpPort = port;
        Settings.Email.SmtpUseSsl = emailSection["SmtpUseSsl"].ToUpper() == "TRUE";
      }
      else
      {
        throw new ApplicationException(Messages.InvalidSettingsErrorString);
      }

      var azureImagesSection = configuration.GetSection("AzureImages");

      Settings.AzureImages.AccountName = azureImagesSection["AccountName"];
      Settings.AzureImages.AccountKey = azureImagesSection["AccountKey"];
      Settings.AzureImages.ImagesContainer = azureImagesSection["ImagesContainer"];
      Settings.AzureImages.ThumbnailsContainer = azureImagesSection["ThumbnailsContainer"];
      Settings.AzureImages.SupportedExtensions = azureImagesSection.GetSection("SupportedExtensions").Get<List<string>>();

      var socketConnectionOptions = configuration.GetSection("MicrocontrollersConnectionsOptions:Socket");

      Settings.Socket.ReceiveTimeout = Int32.Parse(socketConnectionOptions["ReceiveTimeout"]);
      Settings.Socket.SendTimeout = Int32.Parse(socketConnectionOptions["SendTimeout"]);
      Settings.Socket.ListenQueue = Int32.Parse(socketConnectionOptions["ListenQueue"]);
      Settings.Socket.IPAddress = socketConnectionOptions["IPAddress"];
      Settings.Socket.Port = Int32.Parse(socketConnectionOptions["Port"]);
    }
  }
}
