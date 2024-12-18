using Gss.Core.DTOs.SensorData;

namespace Gss.Core.Interfaces.Services;

public interface ISensorsDataService
{
    Task<Dictionary<DateOnly, List<SensorDataDto>>> GetSensorDataAsync(RequestSensorDataDto requestSensorDataDto,
        CancellationToken cancellationToken = default);
}