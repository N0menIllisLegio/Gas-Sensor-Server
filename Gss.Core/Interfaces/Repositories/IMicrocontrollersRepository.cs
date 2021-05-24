using System.Collections.Generic;
using System.Threading.Tasks;
using Gss.Core.Entities;

namespace Gss.Core.Interfaces.Repositories
{
  public interface IMicrocontrollersRepository: IRepositoryBase<Microcontroller>
  {
    Task<List<Microcontroller>> GetVisibleMicrocontrollers(double southWestLatitude, double southWestLongitude, double northEastLatitude, double northEastLongitude);
  }
}
