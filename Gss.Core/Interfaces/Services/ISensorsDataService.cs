using Gss.Core.DTOs.SensorData;

namespace Gss.Core.Interfaces.Services;

public interface ISensorsDataService
{
  Task<List<SensorDataDto>> GetSensorData(string requestedByEmail, RequestSensorDataDto requestSensorDataDto);
}