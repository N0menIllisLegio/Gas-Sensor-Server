using Gss.Core.Entities;

namespace Gss.MicrocontrollerDataHandler.Email;

internal interface IEmailService
{
  Task<bool> SendCriticalValueEmailAsync(int receivedCriticalValue, int setCriticalValue,
      Microcontroller microcontroller, Sensor sensor, CancellationToken cancellationToken = default);
}