using Gss.Core.DTOs;
using Gss.Core.DTOs.SensorType;
using Gss.Core.Entities;

namespace Gss.Core.Interfaces.Repositories;

public interface ISensorsTypesRepository : IRepositoryBase<SensorType>
{
    Task<PagedResultDto<SensorType>> GetPagedResultAsync(PagedInfoDto pagedInfoDto, CancellationToken cancellationToken = default);
    Task<int> UpdateSensorTypeAsync(Guid sensorTypeId, UpdateSensorTypeDto updateSensorTypeDto, CancellationToken cancellationToken = default);
}