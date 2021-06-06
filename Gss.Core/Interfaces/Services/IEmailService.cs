using System.Threading.Tasks;
using Gss.Core.Entities;
using MimeKit;

namespace Gss.Core.Interfaces.Services
{
  public interface IEmailService
  {
    Task<bool> SendEmailAsync(MimeMessage emailMessage);
    Task<bool> SendTextEmailAsync(string sendToAddress, string subject, string message);
    Task<bool> SendHtmlEmailAsync(string sendToAddress, string subject, string html, string textMessage = null);
    Task SendCriticalValueEmail(string email, int receivedCriticalValue, int setCriticalValue, Microcontroller microcontroller, Sensor sensor, SensorType sensorType);
  }
}
