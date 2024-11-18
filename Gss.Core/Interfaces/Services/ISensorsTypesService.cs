using Gss.Core.DTOs;
using Gss.Core.DTOs.SensorType;

namespace Gss.Core.Interfaces.Services;

public interface ISensorsTypesService
{
  Task<PagedResultDto<SensorTypeDto>> GetAllSensorsTypesAsync(PagedInfoDto pagedInfo);
  Task<SensorTypeDto> GetSensorTypeAsync(Guid sensorTypeId);
  Task<SensorTypeDto> CreateSensorTypeAsync(CreateSensorTypeDto createSensorTypeDto);
  Task UpdateSensorTypeAsync(Guid sensorTypeId, UpdateSensorTypeDto updateSensorTypeDto);
  Task DeleteSensorTypeAsync(Guid sensorTypeId);
}