using Gss.Core.Entities;
using Gss.Core.Interfaces;

namespace Gss.Infrastructure.Repositories
{
  public class SensorsRepository : RepositoryBase<Sensor>, ISensorsRepository
  {
    public SensorsRepository(AppDbContext appDbContext)
      : base(appDbContext)
    { }
  }
}
