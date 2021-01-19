using Gss.Core.Entities;
using Gss.Core.Interfaces.Repositories;

namespace Gss.Infrastructure.Repositories
{
  public class MicrocontrollersRepository : RepositoryBase<Microcontroller>, IMicrocontrollersRepository
  {
    public MicrocontrollersRepository(AppDbContext appDbContext)
      : base(appDbContext)
    { }
  }
}
