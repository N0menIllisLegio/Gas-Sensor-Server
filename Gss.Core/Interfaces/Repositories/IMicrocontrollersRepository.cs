using System.Collections.Generic;
using System.Threading.Tasks;
using Gss.Core.Entities;
using Gss.Core.Models;

namespace Gss.Core.Interfaces.Repositories
{
  public interface IMicrocontrollersRepository: IRepositoryBase<Microcontroller>
  {
    Task<List<MapMicrocontrollerModel>> GetVisibleMicrocontrollers(double southWestLatitude, double southWestLongitude, double northEastLatitude, double northEastLongitude);
  }
}
