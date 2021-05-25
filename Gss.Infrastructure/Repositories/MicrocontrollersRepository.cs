using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gss.Core.Entities;
using Gss.Core.Interfaces.Repositories;
using Gss.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Gss.Infrastructure.Repositories
{
  public class MicrocontrollersRepository : RepositoryBase<Microcontroller>, IMicrocontrollersRepository
  {
    public MicrocontrollersRepository(AppDbContext appDbContext)
      : base(appDbContext)
    { }

    public async Task<List<MapMicrocontrollerModel>> GetVisibleMicrocontrollers(double southWestLatitude, double southWestLongitude,
      double northEastLatitude, double northEastLongitude)
    {
      return await DbSet
        .Include(mc => mc.MicrocontrollerSensors).ThenInclude(micS => micS.Sensor).ThenInclude(sensor => sensor.Type)
        .Where(mc => mc.Latitude.HasValue && mc.Longitude.HasValue
          && mc.Latitude > southWestLatitude && mc.Latitude < northEastLatitude
          && mc.Longitude > southWestLongitude && mc.Longitude < northEastLongitude
          && mc.Public && mc.MicrocontrollerSensors.Count > 0)
        .OrderBy(mc => mc.Name)
        .Take(100)
        .Select(mc => new MapMicrocontrollerModel
        {
          MicrocontrollerID = mc.Id,
          Latitude = mc.Latitude.Value,
          Longitude = mc.Longitude.Value,
          SensorTypes = mc.MicrocontrollerSensors
            .Select(micS => micS.Sensor.Type)
            .ToList()
        })
        .ToListAsync();
    }
  }
}
