using Gss.Core.DTOs;
using Gss.Core.DTOs.Sensor;
using Gss.Core.Entities;
using Gss.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gss.Infrastructure.Repositories;

public class SensorsRepository : RepositoryBase<Sensor>, ISensorsRepository
{
    public SensorsRepository(AppDbContext appDbContext)
        : base(appDbContext)
    {
    }

    public async Task<PagedResultDto<Sensor>> GetPagedResultAsync(PagedInfoDto pagedInfoDto,
        CancellationToken cancellationToken = default)
    {
        var pagedResult = await GetPagedResultAsync(
            pagedInfoDto,
            search => search.Name.Contains(pagedInfoDto.SearchString) ||
                      (search.Description != null && search.Description.Contains(pagedInfoDto.SearchString)),
            include => include.Include(sensor => sensor.Type), cancellationToken);

        return pagedResult;
    }

    public Task<Sensor?> FindSensorAsync(Guid sensorId, CancellationToken cancellationToken = default)
    {
        return DbSet.Include(sensor => sensor.Type)
            .FirstOrDefaultAsync(x => x.Id == sensorId, cancellationToken);
    }

    public async Task<int> UpdateSensorAsync(Guid sensorId, UpdateSensorDto updateSensorDto,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(x => x.Id == sensorId)
            .ExecuteUpdateAsync(x => x
                    .SetProperty(p => p.Name, updateSensorDto.Name)
                    .SetProperty(p => p.Description, updateSensorDto.Description)
                    .SetProperty(p => p.TypeId, updateSensorDto.TypeId),
                cancellationToken);
    }
}