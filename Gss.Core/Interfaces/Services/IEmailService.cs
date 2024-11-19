using Gss.Core.Entities;

namespace Gss.Core.Interfaces.Services;

public interface IEmailService
{
  Task<bool> SendCriticalValueEmailAsync(int receivedCriticalValue, int setCriticalValue,
      Microcontroller microcontroller, Sensor sensor, CancellationToken cancellationToken = default);
}