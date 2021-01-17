using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.Entities;

namespace Gss.Core.Interfaces
{
  public interface IMicrocontrollersRepository : IRepositoryBase<Microcontroller>
  {
    Task<PagedResultDto<Microcontroller>> GetPagedResultAsync(PagedInfoDto pagedInfoDto,
      Expression<Func<Microcontroller, bool>> additionalFilterCriteria = null, bool disableTracking = true);
  }
}
