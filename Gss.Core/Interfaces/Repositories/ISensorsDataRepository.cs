using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gss.Core.Entities;
using Gss.Core.Enums;
using Gss.Core.Models;

namespace Gss.Core.Interfaces.Repositories
{
  public interface ISensorsDataRepository: IRepositoryBase<SensorData>
  {
    Task SingleInsertIfNotExists(SensorData sensorData);
    Task BulkInsertIfNotExists(List<SensorData> sensorData);
    Task<List<SensorDataModel>> GetSensorDataByPeriod(Guid microcontrollerID, Guid sensorID, DateTime watchingDate, SensorDataPeriod period);
  }
}
