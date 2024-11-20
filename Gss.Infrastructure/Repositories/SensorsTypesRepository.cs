using Gss.Core.DTOs;
using Gss.Core.DTOs.SensorType;
using Gss.Core.Entities;
using Gss.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gss.Infrastructure.Repositories;

public class SensorsTypesRepository : RepositoryBase<SensorType>, ISensorsTypesRepository
{
  public SensorsTypesRepository(AppDbContext appDbContext)
    : base(appDbContext)
  {
  }

  public async Task<PagedResultDto<SensorType>> GetPagedResultAsync(PagedInfoDto pagedInfoDto, CancellationToken cancellationToken = default)
  {
    var pagedResultDto = await GetPagedResultAsync(
      pagedInfoDto,
      type => type.Name.Contains(pagedInfoDto.SearchString) ||
              type.Units != null && type.Units.Contains(pagedInfoDto.SearchString),
      cancellationToken: cancellationToken);

    return pagedResultDto;
  }

  public async Task<int> UpdateSensorTypeAsync(
    Guid sensorTypeId, UpdateSensorTypeDto updateSensorTypeDto, CancellationToken cancellationToken = default)
  {
    return await DbSet.Where(x => x.Id == sensorTypeId)
      .ExecuteUpdateAsync(x => x
        .SetProperty(p => p.Name, updateSensorTypeDto.Name)
        .SetProperty(p => p.Units, updateSensorTypeDto.Units)
        .SetProperty(p => p.Icon, updateSensorTypeDto.Icon), cancellationToken: cancellationToken);
  }
}