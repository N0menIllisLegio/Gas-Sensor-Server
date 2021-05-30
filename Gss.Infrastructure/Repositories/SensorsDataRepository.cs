using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gss.Core.Entities;
using Gss.Core.Enums;
using Gss.Core.Interfaces.Repositories;
using Gss.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Gss.Infrastructure.Repositories
{
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
        .GroupBy(data => new { data.MicrocontrollerID, data.SensorID, data.ValueReadTime }))
      {
        dataForInsertion.Add(group.First());
      }

      await DbSet.BulkInsertAsync(dataForInsertion, options =>
      {
        options.AutoMapOutputDirection = false;
        options.InsertIfNotExists = true;
      });
    }

    public async Task<List<SensorDataModel>> GetSensorDataByPeriod(Guid microcontrollerID, Guid sensorID,
      DateTime watchingDate, SensorDataPeriod period)
    {
      var query = period switch
      {
        SensorDataPeriod.Day => GetSensorDataQueryByDayPeriod(microcontrollerID, sensorID, watchingDate),
        SensorDataPeriod.Month => GetSensorDataQueryByMonthPeriod(microcontrollerID, sensorID, watchingDate),
        SensorDataPeriod.Year => GetSensorDataQueryByYearPeriod(microcontrollerID, sensorID, watchingDate),
        _ => throw new ArgumentException(nameof(period)),
      };

      return await query.ToListAsync();
    }

    private IQueryable<SensorDataModel> GetSensorDataQueryByYearPeriod(Guid microcontrollerID, Guid sensorID,
      DateTime watchingDate)
    {
      return from sensorData in DbSet
             where sensorData.MicrocontrollerID == microcontrollerID
              && sensorData.SensorID == sensorID
              && sensorData.ValueReadTime.Year == watchingDate.Year
             select new
             {
               sensorData.MicrocontrollerID,
               sensorData.SensorID,
               sensorData.SensorValue,
               sensorData.ValueReadTime.Date
             }
             into splittedDateData
             group splittedDateData by new
             {
               splittedDateData.Date.Year,
               splittedDateData.Date.Month,
               splittedDateData.MicrocontrollerID,
               splittedDateData.SensorID
             }
             into groupedData
             orderby groupedData.Key.Year, groupedData.Key.Month
             select new SensorDataModel
             {
               MicrocontrollerID = groupedData.Key.MicrocontrollerID,
               SensorID = groupedData.Key.SensorID,
               ValueReadTime = new DateTime(groupedData.Key.Year, groupedData.Key.Month, 1),
               AverageSensorValue = Math.Floor((decimal)groupedData.Average(s => s.SensorValue)),
             };
    }

    private IQueryable<SensorDataModel> GetSensorDataQueryByMonthPeriod(Guid microcontrollerID, Guid sensorID,
      DateTime watchingDate)
    {
      return from sensorData in DbSet
             where sensorData.MicrocontrollerID == microcontrollerID
              && sensorData.SensorID == sensorID
              && sensorData.ValueReadTime.Year == watchingDate.Year
              && sensorData.ValueReadTime.Month == watchingDate.Month
             select new
             {
               sensorData.MicrocontrollerID,
               sensorData.SensorID,
               sensorData.SensorValue,
               sensorData.ValueReadTime.Date
             }
             into splittedDateData
             group splittedDateData by new
             {
               splittedDateData.Date,
               splittedDateData.MicrocontrollerID,
               splittedDateData.SensorID
             }
             into groupedData
             orderby groupedData.Key.Date
             select new SensorDataModel
             {
               MicrocontrollerID = groupedData.Key.MicrocontrollerID,
               SensorID = groupedData.Key.SensorID,
               ValueReadTime = groupedData.Key.Date,
               AverageSensorValue = Math.Floor((decimal)groupedData.Average(s => s.SensorValue)),
             };
    }

    private IQueryable<SensorDataModel> GetSensorDataQueryByDayPeriod(Guid microcontrollerID, Guid sensorID,
      DateTime watchingDate)
    {
      return from sensorData in DbSet
             where sensorData.MicrocontrollerID == microcontrollerID
               && sensorData.SensorID == sensorID
               && sensorData.ValueReadTime.Date == watchingDate.Date
             select new
             {
               sensorData.MicrocontrollerID,
               sensorData.SensorID,
               sensorData.SensorValue,
               sensorData.ValueReadTime.Date,
               sensorData.ValueReadTime.TimeOfDay,
             }
             into splittedDateData
             group splittedDateData by new
             {
               splittedDateData.Date,
               splittedDateData.TimeOfDay.Hours,
               splittedDateData.MicrocontrollerID,
               splittedDateData.SensorID
             }
             into groupedData
             orderby groupedData.Key.Date, groupedData.Key.Hours
             select new SensorDataModel
             {
               MicrocontrollerID = groupedData.Key.MicrocontrollerID,
               SensorID = groupedData.Key.SensorID,
               AverageSensorValue = Math.Floor((decimal)groupedData.Average(s => s.SensorValue)),
               ValueReadTime = new DateTime(groupedData.Key.Date.Year, groupedData.Key.Date.Month,
                 groupedData.Key.Date.Day, groupedData.Key.Hours, 0, 0),
             };
    }
  }
}
