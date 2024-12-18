using Gss.Core.Entities;

namespace Gss.MicrocontrollerDataHandler.Email;

internal interface IEmailService
{
  Task SendCriticalValueEmailAsync(string email, int receivedCriticalValue, int setCriticalValue,
      Microcontroller microcontroller, Sensor sensor, CancellationToken cancellationToken = default);
}