using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gss.Core.Entities;
using Gss.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gss.Infrastructure.Repositories
{
  public class MicrocontrollersRepository : RepositoryBase<Microcontroller>, IMicrocontrollersRepository
  {
    public MicrocontrollersRepository(AppDbContext appDbContext)
      : base(appDbContext)
    { }

    public async Task<List<Microcontroller>> GetVisibleMicrocontrollers(double southWestLatitude, double southWestLongitude,
      double northEastLatitude, double northEastLongitude)
    {
      return await DbSet.Where(mc => mc.Latitude.HasValue && mc.Longitude.HasValue
        && mc.Latitude > southWestLatitude && mc.Latitude < northEastLatitude
        && mc.Longitude > southWestLongitude && mc.Longitude < northEastLongitude)
        .OrderBy(mc => mc.Name)
        .Take(100)
        .ToListAsync();
    }
  }
}
