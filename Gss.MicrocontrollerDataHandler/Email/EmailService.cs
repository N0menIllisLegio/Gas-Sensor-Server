using Gss.Core.Entities;
using Gss.Core.Resources;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Gss.MicrocontrollerDataHandler.Email;

internal class EmailService : IEmailService
{
  private readonly ILogger<EmailService> _logger;
  private readonly EmailOptions _emailOptions;

  public EmailService(IOptions<EmailOptions> emailOptions, ILogger<EmailService> logger)
  {
    _logger = logger;
    _emailOptions = emailOptions.Value;
  }

  public async Task SendCriticalValueEmailAsync(string email, int receivedCriticalValue, int setCriticalValue,
    Microcontroller microcontroller, Sensor sensor, CancellationToken cancellationToken = default)
  {
    string html = Messages.CriticalValueNotificationEmailTemplate
      .Replace("{sensorName}", sensor.Name)
      .Replace("{receivedSensorValue}", receivedCriticalValue.ToString())
      .Replace("{microcontrollerName}", microcontroller.Name)
      .Replace("{microcontrollerLatitude}", microcontroller.Latitude.ToString())
      .Replace("{microcontrollerLongitude}", microcontroller.Longitude.ToString())
      .Replace("{sensorType}", sensor.Type.Name)
      .Replace("{sensorCriticalValue}", setCriticalValue.ToString())
      .Replace("{microcontrollerPageUrl}", $"{_emailOptions.SiteUrl}/microcontrollers/{microcontroller.Id}")
      .Replace("{contactInfo}", Messages.ContactInfoString)
      .Replace("{siteUrl}", _emailOptions.SiteUrl);

    var emailMessage = new MimeMessage();
    var builder = new BodyBuilder
    {
      HtmlBody = html
    };

    emailMessage.From.Add(new MailboxAddress("Gas sensors Administration", _emailOptions.FromEmail));
    emailMessage.To.Add(MailboxAddress.Parse(email));
    emailMessage.Subject = $"Sensor {sensor.Name} reached critical threshold - {receivedCriticalValue}!";
    emailMessage.Body = builder.ToMessageBody();

    using var client = new SmtpClient();

    try
    {
      await client.ConnectAsync(_emailOptions.SmtpServer, _emailOptions.SmtpPort, _emailOptions.SmtpUseSsl,
        cancellationToken);

      if (!string.IsNullOrEmpty(_emailOptions.Username) && !string.IsNullOrEmpty(_emailOptions.Password))
      {
        await client.AuthenticateAsync(_emailOptions.Username, _emailOptions.Password, cancellationToken);
      }

      await client.SendAsync(emailMessage, cancellationToken);
      await client.DisconnectAsync(true, cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, "Failed to send email");

      throw;
    }
  }
}