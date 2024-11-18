using Gss.Core.DTOs;
using Gss.Core.DTOs.SensorType;
using Gss.Core.Entities;

namespace Gss.Core.Interfaces.Services;

public interface ISensorsTypesService
{
  Task<PagedResultDto<SensorType>> GetAllSensorsTypesAsync(PagedInfoDto pagedInfo);
  Task<SensorType> GetSensorTypeAsync(Guid sensorTypeId);
  Task<SensorType> CreateSensorTypeAsync(CreateSensorTypeDto createSensorTypeDto);
  Task UpdateSensorTypeAsync(Guid sensorTypeId, UpdateSensorTypeDto updateSensorTypeDto);
  Task DeleteSensorTypeAsync(Guid sensorTypeId);
}