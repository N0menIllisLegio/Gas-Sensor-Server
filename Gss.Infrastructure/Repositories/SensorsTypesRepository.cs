using Gss.Core.Entities;
using Gss.Core.Interfaces;

namespace Gss.Infrastructure.Repositories
{
  public class SensorsTypesRepository : RepositoryBase<SensorType>, ISensorsTypesRepository
  {
    public SensorsTypesRepository(AppDbContext appDbContext)
      : base(appDbContext)
    { }
  }
}
