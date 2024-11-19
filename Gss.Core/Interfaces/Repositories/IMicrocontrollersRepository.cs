using System.Linq.Expressions;
using Gss.Core.DTOs.Microcontroller;
using Gss.Core.Entities;
using Gss.Core.Utils;
using Microsoft.EntityFrameworkCore.Query;

namespace Gss.Core.Interfaces.Repositories;

public interface IMicrocontrollersRepository: IRepositoryBase<Microcontroller>
{

  Task<Microcontroller?> FirstOrDefaultAsync(Expression<Func<Microcontroller, bool>> match,
    Func<IQueryable<Microcontroller>, IIncludableQueryable<Microcontroller, object>>? include = null);

  Task<List<MapMicrocontrollerDto>> GetVisibleMicrocontrollers(double southWestLatitude, double southWestLongitude, double northEastLatitude, double northEastLongitude);
  Task<int> SetSensorValueThresholdAsync(ICurrentUser currentUser, Guid microcontrollerSensorId, int? criticalValue);
}