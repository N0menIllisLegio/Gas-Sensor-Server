using Gss.Core.DTOs.SensorData;
using Gss.Core.Entities;
using Gss.Core.Utils;

namespace Gss.Core.Interfaces.Repositories;

public interface ISensorsDataRepository
{
  Task SingleInsertIfNotExists(SensorData sensorData, CancellationToken cancellationToken = default);
  Task BulkInsertIfNotExists(List<SensorData> sensorData, CancellationToken cancellationToken = default);
  Task<List<SensorDataDto>> GetSensorDataByPeriodAsync(Guid microcontrollerSensorId, DateTimeOffset watchingDate,
      SensorDataPeriod period, CancellationToken cancellationToken = default);
}