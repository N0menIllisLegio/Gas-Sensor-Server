using Gss.Core.DTOs.SensorData;
using Gss.Core.Entities;
using Gss.Core.Utils;

namespace Gss.Core.Interfaces.Repositories;

public interface ISensorsDataRepository
{
  Task SingleInsertIfNotExists(SensorData sensorData);
  Task BulkInsertIfNotExists(List<SensorData> sensorData);
  Task<List<SensorDataDto>> GetSensorDataByPeriodAsync(Guid microcontrollerSensorId, DateTimeOffset watchingDate,
      SensorDataPeriod period);
}