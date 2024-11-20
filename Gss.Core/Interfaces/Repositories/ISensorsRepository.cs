using Gss.Core.DTOs;
using Gss.Core.DTOs.Sensor;
using Gss.Core.Entities;

namespace Gss.Core.Interfaces.Repositories;

public interface ISensorsRepository : IRepositoryBase<Sensor>
{
    Task<PagedResultDto<Sensor>> GetPagedResultAsync(PagedInfoDto pagedInfoDto, CancellationToken cancellationToken = default);
    Task<Sensor?> FindSensorAsync(Guid sensorId, CancellationToken cancellationToken = default);
    Task<int> UpdateSensorAsync(Guid sensorId, UpdateSensorDto updateSensorDto, CancellationToken cancellationToken = default);
}