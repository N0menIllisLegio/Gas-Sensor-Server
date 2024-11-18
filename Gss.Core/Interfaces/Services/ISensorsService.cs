using Gss.Core.DTOs;
using Gss.Core.DTOs.Sensor;

namespace Gss.Core.Interfaces.Services;

public interface ISensorsService
{
  Task<PagedResultDto<SensorDto>> GetAllSensorsAsync(PagedInfoDto pagedInfoDto);
  Task<SensorDto> GetSensorAsync(Guid sensorId);
  Task<SensorDto> CreateSensorAsync(CreateSensorDto createSensorDto);
  Task UpdateSensorAsync(Guid sensorId, UpdateSensorDto updateSensorDto);
  Task DeleteSensorAsync(Guid sensorId);
}