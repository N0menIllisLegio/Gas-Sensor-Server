using System.Linq.Expressions;
using Gss.Core.DTOs;
using Gss.Core.DTOs.Microcontroller;
using Gss.Core.Entities;
using Gss.Core.Interfaces;
using Gss.Core.Interfaces.Repositories;
using Gss.Core.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Gss.Infrastructure.Repositories;

public class MicrocontrollersRepository : RepositoryBase<Microcontroller>, IMicrocontrollersRepository
{
    private readonly AppDbContext _appDbContext;

    public MicrocontrollersRepository(AppDbContext appDbContext)
        : base(appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public override async Task<Microcontroller?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var microcontroller = await DbSet
            .Include(e => e.MicrocontrollerSensors)
            .ThenInclude(e => e.Sensor)
            .ThenInclude(e => e.Type)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return microcontroller;
    }

    public async Task<int> CountAsync(Expression<Func<Microcontroller, bool>> searchCriteria,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(searchCriteria, cancellationToken);
    }

    public async Task<PagedResultDto<Microcontroller>> GetPagedResultAsync(PagedInfoDto pagedInfoDto,
        Expression<Func<Microcontroller, bool>> searchCriteria, CancellationToken cancellationToken = default)
    {
        var pagedResultDto = await GetPagedResultAsync(
            pagedInfoDto,
            searchCriteria,
            query => query
                .Include(mc => mc.MicrocontrollerSensors)
                .ThenInclude(ms => ms.Sensor)
                .ThenInclude(s => s.Type),
            cancellationToken);

        return pagedResultDto;
    }

    public async Task<List<MapMicrocontrollerDto>> GetVisibleMicrocontrollersAsync(
        double southWestLatitude, double southWestLongitude,
        double northEastLatitude, double northEastLongitude, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(mc => mc.MicrocontrollerSensors)
            .ThenInclude(micS => micS.Sensor)
            .ThenInclude(sensor => sensor.Type)
            .Where(mc => mc.Latitude.HasValue && mc.Longitude.HasValue
                                              && mc.Latitude > southWestLatitude && mc.Latitude < northEastLatitude
                                              && mc.Longitude > southWestLongitude && mc.Longitude < northEastLongitude
                                              && mc.Public)
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
            .ToListAsync(cancellationToken);
    }

    public async Task<int> SetSensorValueThresholdAsync(
        ICurrentUser currentUser, Guid microcontrollerSensorId, int? criticalValue,
        CancellationToken cancellationToken = default)
    {
        return await _appDbContext.Set<MicrocontrollerSensors>()
            .Where(x => x.Id == microcontrollerSensorId && (x.Microcontroller.Public ||
                                                            x.Microcontroller.OwnerId == currentUser.Id ||
                                                            currentUser.IsAdministrator))
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.CriticalValue, criticalValue), cancellationToken);
    }

    public async Task<Microcontroller?> FindMicrocontrollerByMicrocontrollerSensorIdAsync(Guid microcontrollerSensorId,
        CancellationToken cancellationToken = default)
    {
        return await _appDbContext.Microcontrollers
            .Include(x => x.MicrocontrollerSensors)
            .ThenInclude(x => x.Sensor)
            .ThenInclude(x => x.Type)
            .FirstOrDefaultAsync(x => x.MicrocontrollerSensors.Count(y => y.Id == microcontrollerSensorId) == 1,
                cancellationToken);
    }
}