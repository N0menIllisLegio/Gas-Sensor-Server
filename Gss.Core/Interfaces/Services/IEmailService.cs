using Gss.Core.Entities;

namespace Gss.Core.Interfaces.Services;

public interface IEmailService
{
  Task<bool> SendCriticalValueEmailAsync(string email, int receivedCriticalValue, int setCriticalValue,
      Microcontroller microcontroller, Sensor sensor, SensorType sensorType, CancellationToken cancellationToken = default);
}