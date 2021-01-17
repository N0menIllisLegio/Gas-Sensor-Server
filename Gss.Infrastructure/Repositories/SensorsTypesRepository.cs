using System.Linq;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.Entities;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gss.Infrastructure.Repositories
{
  public class SensorsTypesRepository : RepositoryBase<SensorType>, ISensorsTypesRepository
  {
    public SensorsTypesRepository(AppDbContext appDbContext)
      : base(appDbContext)
    { }

    public async Task<PagedResultDto<SensorType>> GetPagedResultAsync(PagedInfoDto pagedInfoDto, bool disableTracking = true)
    {
      var query = DbSet.SearchBy(pagedInfoDto.SearchString, sensorType => new { sensorType.Name, sensorType.Units }, pagedInfoDto.Filters);

      if (disableTracking)
      {
        query = query.AsNoTracking();
      }

      query = query.OrderBy(pagedInfoDto.SortOptions);

      var pagedQuery = query.Skip((pagedInfoDto.PageNumber - 1) * pagedInfoDto.PageSize).Take(pagedInfoDto.PageSize);

      return new PagedResultDto<SensorType>
      {
        Items = await pagedQuery.ToListAsync(),
        TotalItemsCount = await query.CountAsync(),
        PagedInfo = pagedInfoDto
      };
    }
  }
}
