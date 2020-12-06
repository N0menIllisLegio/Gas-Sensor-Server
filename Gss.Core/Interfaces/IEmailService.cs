using System.Threading.Tasks;
using MimeKit;

namespace Gss.Core.Interfaces
{
  public interface IEmailService
  {
    Task<bool> SendEmailAsync(MimeMessage emailMessage);
    Task<bool> SendTextEmailAsync(string sendToAddress, string subject, string message);
    Task<bool> SendHtmlEmailAsync(string sendToAddress, string subject, string html, string textMessage = null);
  }
}
