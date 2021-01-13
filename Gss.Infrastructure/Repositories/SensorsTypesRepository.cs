using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Gss.Core.Entities;
using Gss.Core.Enums;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gss.Infrastructure.Repositories
{
  public class SensorsTypesRepository : ISensorsTypesRepository
  {
    private readonly AppDbContext _appDbContext;

    public SensorsTypesRepository(AppDbContext appDbContext)
    {
      _appDbContext = appDbContext;
    }

    public async Task<(List<SensorType> sensors, int totalQueriedSensorsCount)> GetSensorsTypesAsync(int pageNumber, int pageSize,
      SortOrder sortOrder = SortOrder.None,
      Expression<Func<SensorType, bool>> filter = null,
      Expression<Func<SensorType, object>> sorter = null,
      bool notTracking = false)
    {
      var (pagedSensorsTypesQuery, totalSensorsTypesQuery) = _appDbContext.SensorsTypes
        .AsQueryable().GetPage(pageNumber, pageSize, sortOrder, sorter, filter);

      if (notTracking)
      {
        pagedSensorsTypesQuery = pagedSensorsTypesQuery.AsNoTracking();
      }

      var sensorsTypes = await pagedSensorsTypesQuery.ToListAsync();
      int totalQueriedSensorsTypesCount = await totalSensorsTypesQuery.CountAsync();

      return (sensorsTypes, totalQueriedSensorsTypesCount);
    }

    public async Task<SensorType> GetSensorTypeAsync(Guid sensorTypeID)
    {
      return await _appDbContext.SensorsTypes.FindAsync(sensorTypeID);
    }

    public SensorType AddSensorType(SensorType sensorType, bool generateID = true)
    {
      if (generateID)
      {
        sensorType.ID = Guid.NewGuid();
      }

      return _appDbContext.SensorsTypes.Add(sensorType).Entity;
    }

    public SensorType UpdateSensorType(SensorType sensorType)
    {
      return _appDbContext.SensorsTypes.Update(sensorType).Entity;
    }

    public SensorType DeleteSensorType(SensorType sensorType)
    {
      return _appDbContext.SensorsTypes.Remove(sensorType).Entity;
    }

    public async Task<bool> SaveAsync()
    {
      try
      {
        await _appDbContext.SaveChangesAsync();

        return true;
      }
      catch
      {
        return false;
      }
    }
  }
}
