using System.Collections.Generic;
using System.Threading.Tasks;
using Gss.Core.Entities;
using Gss.Core.Interfaces.Repositories;

namespace Gss.Infrastructure.Repositories
{
  public class SensorsDataRepository: RepositoryBase<SensorData>, ISensorsDataRepository
  {
    public SensorsDataRepository(AppDbContext appDbContext)
      : base(appDbContext)
    { }

    public async Task BulkInsert(List<SensorData> sensorData)
    {
      bool previousAutoDetectChangesEnabledValue = Context.ChangeTracker.AutoDetectChangesEnabled;
      Context.ChangeTracker.AutoDetectChangesEnabled = false;

      DbSet.AddRange(sensorData);
      await Context.SaveChangesAsync();

      Context.ChangeTracker.AutoDetectChangesEnabled = previousAutoDetectChangesEnabledValue;
    }
  }
}
