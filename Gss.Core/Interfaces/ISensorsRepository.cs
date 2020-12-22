using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Gss.Core.Entities;
using Gss.Core.Enums;

namespace Gss.Core.Interfaces
{
  public interface ISensorsRepository
  {
    Task<(List<Sensor> sensors, int totalQueriedSensorsCount)> GetSensorsAsync(int pageNumber, int pageSize,
      SortOrder sortOrder = SortOrder.None,
      Expression<Func<Sensor, bool>> filter = null,
      Expression<Func<Sensor, object>> sorter = null,
      bool notTracking = false);

    Task<Sensor> GetSensorAsync(Guid sensorID);

    Sensor AddSensor(Sensor sensor, bool generateID = true);

    Sensor UpdateSensor(Sensor sensor);

    Sensor DeleteSensor(Sensor sensor);

    Task<bool> SaveAsync();
  }
}
