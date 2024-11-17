using System.Text.RegularExpressions;
using Gss.Core.Helpers;
using Gss.Core.Resources;

namespace Gss.Web.Configuration;

internal static class SettingsExtension
{
  public static void ConfigureSettings(this IConfiguration configuration)
  {
      // TODO: IOptions
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
        Settings.Email.SmtpUseSsl = bool.Parse(emailSection["SmtpUseSsl"]);
      }
      else
      {
        throw new ApplicationException(Messages.InvalidSettingsErrorString);
      }

      var socketConnectionOptions = configuration.GetSection("MicrocontrollersConnectionsOptions:Socket");

      Settings.Socket.ReceiveTimeout = Int32.Parse(socketConnectionOptions["ReceiveTimeout"]);
      Settings.Socket.SendTimeout = Int32.Parse(socketConnectionOptions["SendTimeout"]);
      Settings.Socket.ListenQueue = Int32.Parse(socketConnectionOptions["ListenQueue"]);
      Settings.Socket.IPAddress = socketConnectionOptions["IPAddress"];
      Settings.Socket.Port = Int32.Parse(socketConnectionOptions["Port"]);
    }
}