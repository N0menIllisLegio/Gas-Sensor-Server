using Gss.Core.Entities;
using Gss.Core.Resources;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Gss.MicrocontrollerListener.Email;

internal class EmailService : IEmailService
{
  private readonly EmailOptions _emailOptions;

  public EmailService(IOptions<EmailOptions> emailOptions)
  {
    _emailOptions = emailOptions.Value;
  }

  public async Task<bool> SendCriticalValueEmailAsync(int receivedCriticalValue, int setCriticalValue,
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
      .Replace("{microcontrollerPageUrl}", $"{Messages.SiteURLString}/microcontroller/{microcontroller.Id}")
      .Replace("{contactInfo}", Messages.ContactInfoString)
      .Replace("{siteUrl}", Messages.SiteURLString);

    var emailMessage = new MimeMessage();
    var builder = new BodyBuilder
    {
      HtmlBody = html
    };

    emailMessage.From.Add(new MailboxAddress("Gas sensors Administration", _emailOptions.Address));
    // TODO: emailMessage.To.Add(MailboxAddress.Parse(email));
    emailMessage.Subject = $"Sensor {sensor.Name} reached critical threshold - {receivedCriticalValue}!";
    emailMessage.Body = builder.ToMessageBody();

    bool sendSuccessfully = true;
    using var client = new SmtpClient();

    try
    {
      await client.ConnectAsync(_emailOptions.SmtpServer, _emailOptions.SmtpPort, _emailOptions.SmtpUseSsl, cancellationToken);
      await client.AuthenticateAsync(_emailOptions.Address, _emailOptions.Password, cancellationToken);
      await client.SendAsync(emailMessage, cancellationToken);
      await client.DisconnectAsync(true, cancellationToken);
    }
    catch
    {
      sendSuccessfully = false;
    }

    return sendSuccessfully;
  }
}