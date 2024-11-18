using Gss.Core.Entities;
using Gss.Core.Enums;
using Gss.Core.Interfaces.Repositories;
using Gss.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Gss.Infrastructure.Repositories;

public class SensorsDataRepository: RepositoryBase<SensorData>, ISensorsDataRepository
{
  public SensorsDataRepository(AppDbContext appDbContext)
    : base(appDbContext)
  { }

  public async Task SingleInsertIfNotExists(SensorData sensorData)
  {
    await DbSet.SingleInsertAsync(sensorData, options =>
    {
      options.AutoMapOutputDirection = false;
      options.InsertIfNotExists = true;
    });
  }

  public async Task BulkInsertIfNotExists(List<SensorData> sensorData)
  {
    var dataForInsertion = new List<SensorData>();

    foreach (var group in sensorData
               .GroupBy(data => new { data.MicrocontrollerId, data.SensorId, data.ValueReadTime }))
    {
      dataForInsertion.Add(group.First());
    }

    await DbSet.BulkInsertAsync(dataForInsertion, options =>
    {
      options.AutoMapOutputDirection = false;
      options.InsertIfNotExists = true;
    });
  }

  public async Task<List<SensorDataModel>> GetSensorDataByPeriod(Guid microcontrollerId, Guid sensorId,
    DateTimeOffset watchingDate, SensorDataPeriod period)
  {
    var query = period switch
    {
      SensorDataPeriod.Day => GetSensorDataQueryByDayPeriod(microcontrollerId, sensorId, watchingDate),
      SensorDataPeriod.Month => GetSensorDataQueryByMonthPeriod(microcontrollerId, sensorId, watchingDate),
      SensorDataPeriod.Year => GetSensorDataQueryByYearPeriod(microcontrollerId, sensorId, watchingDate),
      _ => throw new ArgumentException(nameof(period)),
    };

    return await query.ToListAsync();
  }

  private IQueryable<SensorDataModel> GetSensorDataQueryByYearPeriod(Guid microcontrollerId, Guid sensorId,
    DateTimeOffset watchingDate)
  {
    return from sensorData in DbSet
      where sensorData.MicrocontrollerId == microcontrollerId
            && sensorData.SensorId == sensorId
            && sensorData.ValueReadTime.Year == watchingDate.Year
      select new
      {
        sensorData.MicrocontrollerId,
        sensorData.SensorId,
        sensorData.SensorValue,
        sensorData.ValueReadTime.Date
      }
      into splittedDateData
      group splittedDateData by new
      {
        splittedDateData.Date.Year,
        splittedDateData.Date.Month,
        splittedDateData.MicrocontrollerId,
        splittedDateData.SensorId
      }
      into groupedData
      orderby groupedData.Key.Year, groupedData.Key.Month
      select new SensorDataModel
      {
        MicrocontrollerID = groupedData.Key.MicrocontrollerId,
        SensorID = groupedData.Key.SensorId,
        ValueReadTime = DateTime.SpecifyKind(new DateTime(groupedData.Key.Year, groupedData.Key.Month, 1), DateTimeKind.Utc),
        AverageSensorValue = Math.Floor((decimal)groupedData.Average(s => s.SensorValue)),
      };
  }

  private IQueryable<SensorDataModel> GetSensorDataQueryByMonthPeriod(Guid microcontrollerId, Guid sensorId,
    DateTimeOffset watchingDate)
  {
    return from sensorData in DbSet
      where sensorData.MicrocontrollerId == microcontrollerId
            && sensorData.SensorId == sensorId
            && sensorData.ValueReadTime.Year == watchingDate.Year
            && sensorData.ValueReadTime.Month == watchingDate.Month
      select new
      {
        sensorData.MicrocontrollerId,
        sensorData.SensorId,
        sensorData.SensorValue,
        sensorData.ValueReadTime.Date
      }
      into splittedDateData
      group splittedDateData by new
      {
        splittedDateData.Date,
        splittedDateData.MicrocontrollerId,
        splittedDateData.SensorId
      }
      into groupedData
      orderby groupedData.Key.Date
      select new SensorDataModel
      {
        MicrocontrollerID = groupedData.Key.MicrocontrollerId,
        SensorID = groupedData.Key.SensorId,
        ValueReadTime = DateTime.SpecifyKind(groupedData.Key.Date, DateTimeKind.Utc),
        AverageSensorValue = Math.Floor((decimal)groupedData.Average(s => s.SensorValue)),
      };
  }

  private IQueryable<SensorDataModel> GetSensorDataQueryByDayPeriod(Guid microcontrollerId, Guid sensorId,
    DateTimeOffset watchingDate)
  {
    return from sensorData in DbSet
      where sensorData.MicrocontrollerId == microcontrollerId
            && sensorData.SensorId == sensorId
            && sensorData.ValueReadTime.Date == watchingDate.Date
      select new
      {
        sensorData.MicrocontrollerId,
        sensorData.SensorId,
        sensorData.SensorValue,
        sensorData.ValueReadTime.Date,
        sensorData.ValueReadTime.TimeOfDay,
      }
      into splittedDateData
      group splittedDateData by new
      {
        splittedDateData.Date,
        splittedDateData.TimeOfDay.Hours,
        splittedDateData.MicrocontrollerId,
        splittedDateData.SensorId
      }
      into groupedData
      orderby groupedData.Key.Date, groupedData.Key.Hours
      select new SensorDataModel
      {
        MicrocontrollerID = groupedData.Key.MicrocontrollerId,
        SensorID = groupedData.Key.SensorId,
        AverageSensorValue = Math.Floor((decimal)groupedData.Average(s => s.SensorValue)),
        ValueReadTime = new DateTime(groupedData.Key.Date.Year, groupedData.Key.Date.Month,
          groupedData.Key.Date.Day, groupedData.Key.Hours, 0, 0, DateTimeKind.Utc),
      };
  }
}