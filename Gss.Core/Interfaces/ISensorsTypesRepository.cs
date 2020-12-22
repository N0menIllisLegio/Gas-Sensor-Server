using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Gss.Core.Entities;
using Gss.Core.Enums;

namespace Gss.Core.Interfaces
{
  public interface ISensorsTypesRepository
  {
    Task<(List<SensorType> sensors, int totalQueriedSensorsCount)> GetSensorsTypesAsync(int pageNumber, int pageSize,
      SortOrder sortOrder = SortOrder.None,
      Expression<Func<SensorType, bool>> filter = null,
      Expression<Func<SensorType, object>> sorter = null,
      bool notTracking = false);

    Task<SensorType> GetSensorTypeAsync(Guid sensorTypeID);

    SensorType AddSensorType(SensorType sensorType, bool generateID = true);

    SensorType UpdateSensor(SensorType sensorType);

    SensorType DeleteSensor(SensorType sensorType);

    Task<bool> SaveAsync();
  }
}
