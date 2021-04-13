using System.Collections.Generic;
using System.Threading.Tasks;
using Gss.Core.Entities;

namespace Gss.Core.Interfaces.Repositories
{
  public interface ISensorsDataRepository: IRepositoryBase<SensorData>
  {
    Task BulkInsertIfNotExists(List<SensorData> sensorData);
  }
}
