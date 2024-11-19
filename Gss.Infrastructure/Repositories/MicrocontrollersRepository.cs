using System.Linq.Expressions;
using Gss.Core.DTOs.Microcontroller;
using Gss.Core.Entities;
using Gss.Core.Interfaces;
using Gss.Core.Interfaces.Repositories;
using Gss.Core.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Gss.Infrastructure.Repositories;

public class MicrocontrollersRepository : RepositoryBase<Microcontroller>, IMicrocontrollersRepository
{
  private readonly AppDbContext _appDbContext;

  public MicrocontrollersRepository(AppDbContext appDbContext)
    : base(appDbContext)
  {
    _appDbContext = appDbContext;
  }

  public async Task<List<MapMicrocontrollerDto>> GetVisibleMicrocontrollers(
    double southWestLatitude, double southWestLongitude,
    double northEastLatitude, double northEastLongitude)
  {
    return await DbSet
      .Include(mc => mc.MicrocontrollerSensors)
        .ThenInclude(micS => micS.Sensor)
          .ThenInclude(sensor => sensor.Type)
      .Where(mc => mc.Latitude.HasValue && mc.Longitude.HasValue
        && mc.Latitude > southWestLatitude && mc.Latitude < northEastLatitude
        && mc.Longitude > southWestLongitude && mc.Longitude < northEastLongitude
        && mc.Public && mc.MicrocontrollerSensors.Count > 0)
      .OrderBy(mc => mc.Name)
      .Take(100)
      .Select(mc => new MapMicrocontrollerDto
      {
        MicrocontrollerId = mc.Id,
        Latitude = mc.Latitude!.Value,
        Longitude = mc.Longitude!.Value,
        SensorTypes = mc.MicrocontrollerSensors
          .Select(micS => micS.Sensor.Type.MapToDto())
          .ToList()
      })
      .ToListAsync();
  }

  public async Task<Microcontroller?> FirstOrDefaultAsync(Expression<Func<Microcontroller, bool>> match,
    Func<IQueryable<Microcontroller>, IIncludableQueryable<Microcontroller, object>>? include = null)
  {
    return include is not null
      ? await include(DbSet).FirstOrDefaultAsync(match)
      : await DbSet.FirstOrDefaultAsync(match);
  }

  public async Task<int> SetSensorValueThresholdAsync(
    ICurrentUser currentUser, Guid microcontrollerSensorId, int? criticalValue)
  {
    return await _appDbContext.Set<MicrocontrollerSensors>()
      .Where(x => x.Id == microcontrollerSensorId && (x.Microcontroller.Public ||
                                                      x.Microcontroller.OwnerId == currentUser.Id ||
                                                      currentUser.IsAdministrator))
      .ExecuteUpdateAsync(x => x.SetProperty(p => p.CriticalValue, criticalValue));
  }
}