using Gss.Core.DTOs.SensorData;

namespace Gss.Core.Interfaces.Services;

public interface ISensorsDataService
{
  Task<List<SensorDataDto>> GetSensorDataAsync(RequestSensorDataDto requestSensorDataDto, CancellationToken cancellationToken = default);
}