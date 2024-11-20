using Gss.Core.DTOs.SensorData;
using Gss.Core.Utils;

namespace Gss.Core.Interfaces.Repositories;

public interface ISensorsDataRepository
{
  Task<List<SensorDataDto>> GetSensorDataByPeriodAsync(Guid microcontrollerSensorId, DateTimeOffset watchingDate,
      SensorDataPeriod period, CancellationToken cancellationToken = default);
}