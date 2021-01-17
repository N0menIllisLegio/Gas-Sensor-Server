using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.Entities;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gss.Infrastructure.Repositories
{
  public class MicrocontrollersRepository : RepositoryBase<Microcontroller>, IMicrocontrollersRepository
  {
    public MicrocontrollersRepository(AppDbContext appDbContext)
      : base(appDbContext)
    { }

    public async Task<PagedResultDto<Microcontroller>> GetPagedResultAsync(PagedInfoDto pagedInfoDto,
      Expression<Func<Microcontroller, bool>> additionalFilterCriteria = null, bool disableTracking = true)
    {
      var query = DbSet.SearchBy(pagedInfoDto.SearchString, microcontroller => new { microcontroller.Name, microcontroller.LastResponseTime },
        pagedInfoDto.Filters, additionalFilterCriteria);

      if (disableTracking)
      {
        query = query.AsNoTracking();
      }

      query = query.OrderBy(pagedInfoDto.SortOptions);

      var pagedQuery = query.Skip((pagedInfoDto.PageNumber - 1) * pagedInfoDto.PageSize).Take(pagedInfoDto.PageSize);

      return new PagedResultDto<Microcontroller>
      {
        Items = await pagedQuery.ToListAsync(),
        TotalItemsCount = await query.CountAsync(),
        PagedInfo = pagedInfoDto
      };
    }
  }
}
