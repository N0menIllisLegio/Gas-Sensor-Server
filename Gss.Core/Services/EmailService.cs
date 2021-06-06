using System.Threading.Tasks;
using Gss.Core.Entities;
using Gss.Core.Helpers;
using Gss.Core.Interfaces.Services;
using Gss.Core.Resources;
using MailKit.Net.Smtp;
using MimeKit;

namespace Gss.Core.Services
{
  public class EmailService : IEmailService
  {
    private const string _senderName = "Gas sensors Administration";

    public async Task<bool> SendEmailAsync(MimeMessage emailMessage)
    {
      bool sendSuccessfully = true;
      using var client = new SmtpClient();

      try
      {
        await client.ConnectAsync(Settings.Email.SmtpServer, Settings.Email.SmtpPort, Settings.Email.SmtpUseSsl);
        await client.AuthenticateAsync(Settings.Email.Address, Settings.Email.Password);
        await client.SendAsync(emailMessage);
        await client.DisconnectAsync(true);
      }
      catch
      {
        sendSuccessfully = false;
      }

      return sendSuccessfully;
    }

    public async Task<bool> SendHtmlEmailAsync(string sendToAddress, string subject, string html,
      string textMessage = null)
    {
      var emailMessage = new MimeMessage();
      var builder = new BodyBuilder
      {
        HtmlBody = html,
        TextBody = textMessage
      };

      emailMessage.From.Add(new MailboxAddress(_senderName, Settings.Email.Address));
      emailMessage.To.Add(MailboxAddress.Parse(sendToAddress));
      emailMessage.Subject = subject;
      emailMessage.Body = builder.ToMessageBody();

      return await SendEmailAsync(emailMessage);
    }

    public async Task<bool> SendTextEmailAsync(string sendToAddress, string subject, string message)
    {
      var emailMessage = new MimeMessage();
      var builder = new BodyBuilder
      {
        TextBody = message
      };

      emailMessage.From.Add(new MailboxAddress(_senderName, Settings.Email.Address));
      emailMessage.To.Add(MailboxAddress.Parse(sendToAddress));
      emailMessage.Subject = subject;
      emailMessage.Body = builder.ToMessageBody();

      return await SendEmailAsync(emailMessage);
    }

    public async Task SendCriticalValueEmail(string email, int receivedCriticalValue, int setCriticalValue,
      Microcontroller microcontroller, Sensor sensor, SensorType sensorType)
    {
      string html = Messages.CriticalValueNotificationEmailTemplate
        .Replace("{sensorName}", sensor.Name)
        .Replace("{receivedSensorValue}", receivedCriticalValue.ToString())
        .Replace("{microcontrollerName}", microcontroller.Name)
        .Replace("{microcontrollerLatitude}", microcontroller.Latitude.ToString())
        .Replace("{microcontrollerLongitude}", microcontroller.Longitude.ToString())
        .Replace("{sensorType}", sensorType.Name)
        .Replace("{sensorCriticalValue}", setCriticalValue.ToString())
        .Replace("{microcontrollerPageUrl}", $"{Messages.SiteURLString}/microcontroller/{microcontroller.Id}")
        .Replace("{contactInfo}", Messages.ContactInfoString)
        .Replace("{siteUrl}", Messages.SiteURLString);

      await SendHtmlEmailAsync(email, $"Sensor {sensor.Name} reached critical threshold - {receivedCriticalValue}!", html);
    }
  }
}
