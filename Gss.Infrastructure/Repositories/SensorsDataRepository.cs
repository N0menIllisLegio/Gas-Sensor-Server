using Gss.Core.DTOs.SensorData;
using Gss.Core.Entities;
using Gss.Core.Interfaces.Repositories;
using Gss.Core.Utils;
using Microsoft.EntityFrameworkCore;

namespace Gss.Infrastructure.Repositories;

public class SensorsDataRepository: ISensorsDataRepository
{
  private readonly DbSet<SensorData> _dbSet;

  public SensorsDataRepository(AppDbContext appDbContext)
  {
    _dbSet = appDbContext.SensorsData;
  }

  public async Task SingleInsertIfNotExists(SensorData sensorData, CancellationToken cancellationToken = default)
  {
    await _dbSet.SingleInsertAsync(sensorData, options =>
    {
      options.AutoMapOutputDirection = false;
      options.InsertIfNotExists = true;
    }, cancellationToken);
  }

  public async Task BulkInsertIfNotExists(List<SensorData> sensorData, CancellationToken cancellationToken = default)
  {
    var dataForInsertion = new List<SensorData>();

    foreach (var group in sensorData.GroupBy(data => new { data.MicrocontrollerSensorId, ValueReadTime = data.ReadTime }))
    {
      dataForInsertion.Add(group.First());
    }

    await _dbSet.BulkInsertAsync(dataForInsertion, options =>
    {
      options.AutoMapOutputDirection = false;
      options.InsertIfNotExists = true;
    }, cancellationToken);
  }

  public async Task<List<SensorDataDto>> GetSensorDataByPeriodAsync(Guid microcontrollerSensorId,
    DateTimeOffset watchingDate, SensorDataPeriod period, CancellationToken cancellationToken = default)
  {
    var query = period switch
    {
      SensorDataPeriod.Day => GetSensorDataQueryByDayPeriod(microcontrollerSensorId, watchingDate),
      SensorDataPeriod.Month => GetSensorDataQueryByMonthPeriod(microcontrollerSensorId, watchingDate),
      SensorDataPeriod.Year => GetSensorDataQueryByYearPeriod(microcontrollerSensorId, watchingDate),

      _ => throw new ArgumentException(nameof(period)),
    };

    return await query.ToListAsync(cancellationToken: cancellationToken);
  }

  private IQueryable<SensorDataDto> GetSensorDataQueryByYearPeriod(
    Guid microcontrollerSensorId, DateTimeOffset watchingDate)
  {
    return _dbSet
      .Where(sensorData => sensorData.MicrocontrollerSensorId == microcontrollerSensorId &&
                           sensorData.ReadTime.Year == watchingDate.Year)
      .GroupBy(sensorData => new
      {
        sensorData.ReadTime.Year,
        sensorData.ReadTime.Month,
        sensorData.MicrocontrollerSensorId
      })
      .OrderBy(group => group.Key.Year)
      .ThenBy(group => group.Key.Month)
      .Select(group => new SensorDataDto
      {
        WatchingDate = watchingDate,
        AverageValue = group.Average(s => s.Value),
        ReadTime = new DateTime(group.Key.Year, group.Key.Month, 1),
      });
  }

  private IQueryable<SensorDataDto> GetSensorDataQueryByMonthPeriod(
    Guid microcontrollerSensorId, DateTimeOffset watchingDate)
  {
    return _dbSet
      .Where(sensorData => sensorData.MicrocontrollerSensorId == microcontrollerSensorId &&
                           sensorData.ReadTime.Year == watchingDate.Year &&
                           sensorData.ReadTime.Month == watchingDate.Month)
      .GroupBy(sensorData => new
      {
        sensorData.ReadTime.Date,
        sensorData.MicrocontrollerSensorId
      })
      .OrderBy(group => group.Key.Date)
      .Select(group => new SensorDataDto
      {
        WatchingDate = watchingDate,
        AverageValue = group.Average(s => s.Value),
        ReadTime = group.Key.Date,
      });
  }

  private IQueryable<SensorDataDto> GetSensorDataQueryByDayPeriod(
    Guid microcontrollerSensorId, DateTimeOffset watchingDate)
  {
    return _dbSet
      .Where(sensorData => sensorData.MicrocontrollerSensorId == microcontrollerSensorId &&
                           sensorData.ReadTime.Date == watchingDate.Date)
      .GroupBy(sensorData => new
      {
        sensorData.ReadTime.Date,
        sensorData.ReadTime.TimeOfDay.Hours,
        sensorData.MicrocontrollerSensorId
      })
      .OrderBy(group => group.Key.Date)
      .ThenBy(group => group.Key.Hours)
      .Select(group => new SensorDataDto
      {
        WatchingDate = watchingDate,
        AverageValue = group.Average(s => s.Value),
        ReadTime = new DateTime(group.Key.Date.Year, group.Key.Date.Month, group.Key.Date.Day, group.Key.Hours, 0, 0),
      });
  }
}