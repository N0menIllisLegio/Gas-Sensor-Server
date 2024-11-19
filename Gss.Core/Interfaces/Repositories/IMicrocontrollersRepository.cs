using System.Linq.Expressions;
using Gss.Core.DTOs.Microcontroller;
using Gss.Core.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace Gss.Core.Interfaces.Repositories;

public interface IMicrocontrollersRepository: IRepositoryBase<Microcontroller>
{
  Task<Microcontroller?> FirstOrDefaultAsync(Expression<Func<Microcontroller, bool>> match,
    Func<IQueryable<Microcontroller>, IIncludableQueryable<Microcontroller, object>>? include = null,
    CancellationToken cancellationToken = default);

  Task<List<MapMicrocontrollerDto>> GetVisibleMicrocontrollersAsync(double southWestLatitude, double southWestLongitude, double northEastLatitude, double northEastLongitude, CancellationToken cancellationToken = default);
  Task<int> SetSensorValueThresholdAsync(ICurrentUser currentUser, Guid microcontrollerSensorId, int? criticalValue, CancellationToken cancellationToken = default);
}