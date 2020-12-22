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
  public class SensorsRepository : ISensorsRepository
  {
    private readonly AppDbContext _appDbContext;

    public SensorsRepository(AppDbContext appDbContext)
    {
      _appDbContext = appDbContext;
    }

    public async Task<(List<Sensor> sensors, int totalQueriedSensorsCount)> GetSensorsAsync(int pageSize, int pageNumber,
      SortOrder sortOrder = SortOrder.None,
      Expression<Func<Sensor, bool>> filter = null,
      Expression<Func<Sensor, object>> sorter = null,
      bool notTracking = false)
    {
      var (pagedSensorsQuery, totalSensorsQuery) = _appDbContext.Sensors
        .AsQueryable().GetPage(pageNumber, pageSize, sortOrder, sorter, filter);

      if (notTracking)
      {
        pagedSensorsQuery = pagedSensorsQuery.AsNoTracking();
      }

      var sensors = await pagedSensorsQuery.ToListAsync();
      int totalQueriedSensorsCount = await totalSensorsQuery.CountAsync();

      return (sensors, totalQueriedSensorsCount);
    }

    public async Task<Sensor> GetSensorAsync(Guid sensorID)
    {
      return await _appDbContext.Sensors.FindAsync(sensorID);
    }

    public Sensor AddSensor(Sensor sensor, bool generateID = true)
    {
      if (generateID)
      {
        sensor.ID = Guid.NewGuid();
      }

      return _appDbContext.Sensors.Add(sensor).Entity;
    }

    public Sensor UpdateSensor(Sensor sensor)
    {
      return _appDbContext.Sensors.Update(sensor).Entity;
    }

    public Sensor DeleteSensor(Sensor sensor)
    {
      return _appDbContext.Sensors.Remove(sensor).Entity;
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
