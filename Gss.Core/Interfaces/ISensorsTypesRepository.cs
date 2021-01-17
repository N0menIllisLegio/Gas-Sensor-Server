using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.Entities;

namespace Gss.Core.Interfaces
{
  public interface ISensorsTypesRepository : IRepositoryBase<SensorType>
  {
    Task<PagedResultDto<SensorType>> GetPagedResultAsync(PagedInfoDto pagedInfoDto, bool disableTracking = true);
  }
}
