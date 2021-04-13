using System.Collections.Generic;
using System.Linq;
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

    public async Task BulkInsertIfNotExists(List<SensorData> sensorData)
    {
      var dataForInsertion = new List<SensorData>();

      foreach (var group in sensorData
        .GroupBy(data => new { data.MicrocontrollerID, data.SensorID, data.ValueReadTime }))
      {
        dataForInsertion.Add(group.First());
      }

      await DbSet.BulkInsertAsync(dataForInsertion, options =>
      {
        options.AutoMapOutputDirection = false;
        options.InsertIfNotExists = true;
      });
    }r
  }
}
