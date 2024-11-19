using Gss.Core.DTOs.Sensor;
using Gss.Core.Entities;
using Gss.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gss.Infrastructure.Repositories;

public class SensorsRepository : RepositoryBase<Sensor>, ISensorsRepository
{
  public SensorsRepository(AppDbContext appDbContext)
    : base(appDbContext)
  { }

  public Task<Sensor?> FindSensorAsync(Guid sensorId, CancellationToken cancellationToken = default)
  {
    return DbSet.Include(sensor => sensor.Type)
      .FirstOrDefaultAsync(x => x.Id == sensorId, cancellationToken: cancellationToken);
  }

  public async Task<int> UpdateSensorAsync(Guid sensorId, UpdateSensorDto updateSensorDto,
    CancellationToken cancellationToken = default)
  {
    return await DbSet.Where(x => x.Id == sensorId)
      .ExecuteUpdateAsync(x => x
        .SetProperty(p => p.Name, updateSensorDto.Name)
        .SetProperty(p => p.Description, updateSensorDto.Description)
        .SetProperty(p => p.TypeId, updateSensorDto.TypeId),
        cancellationToken: cancellationToken);
  }
}