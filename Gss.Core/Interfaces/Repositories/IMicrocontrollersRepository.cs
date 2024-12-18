using System.Linq.Expressions;
using Gss.Core.DTOs;
using Gss.Core.DTOs.Microcontroller;
using Gss.Core.Entities;

namespace Gss.Core.Interfaces.Repositories;

public interface IMicrocontrollersRepository: IRepositoryBase<Microcontroller>
{
  Task<int> CountAsync(Expression<Func<Microcontroller, bool>> searchCriteria, CancellationToken cancellationToken = default);

  Task<PagedResultDto<Microcontroller>> GetPagedResultAsync(PagedInfoDto pagedInfoDto,
    Expression<Func<Microcontroller, bool>> searchCriteria, CancellationToken cancellationToken = default);

  Task<List<MapMicrocontrollerDto>> GetVisibleMicrocontrollersAsync(double southWestLatitude, double southWestLongitude, double northEastLatitude, double northEastLongitude, CancellationToken cancellationToken = default);
  Task<int> SetSensorValueThresholdAsync(ICurrentUser currentUser, Guid microcontrollerSensorId, int? criticalValue, CancellationToken cancellationToken = default);

  Task<Microcontroller?> FindMicrocontrollerByMicrocontrollerSensorIdAsync(Guid microcontrollerSensorId,
    CancellationToken cancellationToken = default);

  Task ResetMicrocontrollerRequestSensorValueAsync(Guid microcontrollerId, CancellationToken cancellationToken = default);
}