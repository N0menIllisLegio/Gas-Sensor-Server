using Gss.Core.DTOs;
using Gss.Core.DTOs.SensorType;

namespace Gss.Core.Interfaces.Services;

public interface ISensorsTypesService
{
  Task<PagedResultDto<SensorTypeDto>> GetAllSensorsTypesAsync(PagedInfoDto pagedInfo, CancellationToken cancellationToken = default);
  Task<SensorTypeDto> GetSensorTypeAsync(Guid sensorTypeId, CancellationToken cancellationToken = default);
  Task<SensorTypeDto> CreateSensorTypeAsync(CreateSensorTypeDto createSensorTypeDto, CancellationToken cancellationToken = default);
  Task UpdateSensorTypeAsync(Guid sensorTypeId, UpdateSensorTypeDto updateSensorTypeDto, CancellationToken cancellationToken = default);
  Task DeleteSensorTypeAsync(Guid sensorTypeId, CancellationToken cancellationToken = default);
}