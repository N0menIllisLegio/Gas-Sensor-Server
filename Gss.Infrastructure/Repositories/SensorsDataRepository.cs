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

  public async Task<List<SensorDataDto>> GetSensorDataByPeriodAsync(Guid microcontrollerSensorId,
    DateOnly watchingDate, SensorDataPeriod period, CancellationToken cancellationToken = default)
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
    Guid microcontrollerSensorId, DateOnly watchingDate)
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
        WatchingDate = watchingDate.ToDateTime(TimeOnly.MinValue),
        AverageValue = group.Average(s => s.Value),
        ReadTime = new DateTime(group.Key.Year, group.Key.Month, 1),
      });
  }

  private IQueryable<SensorDataDto> GetSensorDataQueryByMonthPeriod(
    Guid microcontrollerSensorId, DateOnly watchingDate)
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
        WatchingDate = watchingDate.ToDateTime(TimeOnly.MinValue),
        AverageValue = group.Average(s => s.Value),
        ReadTime = group.Key.Date,
      });
  }

  private IQueryable<SensorDataDto> GetSensorDataQueryByDayPeriod(
    Guid microcontrollerSensorId, DateOnly watchingDate)
  {
    return _dbSet
      .Where(sensorData => sensorData.MicrocontrollerSensorId == microcontrollerSensorId &&
                           sensorData.ReadTime.Year == watchingDate.Year &&
                           sensorData.ReadTime.Month == watchingDate.Month &&
                           sensorData.ReadTime.Day == watchingDate.Day)
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
        WatchingDate = watchingDate.ToDateTime(new TimeOnly(group.Key.Hours, 0, 0)),
        AverageValue = group.Average(s => s.Value),
        ReadTime = new DateTime(group.Key.Date.Year, group.Key.Date.Month, group.Key.Date.Day, group.Key.Hours, 0, 0),
      });
  }
}