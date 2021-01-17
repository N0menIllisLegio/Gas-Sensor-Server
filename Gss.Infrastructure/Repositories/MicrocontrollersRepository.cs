using Gss.Core.Entities;
using Gss.Core.Interfaces;

namespace Gss.Infrastructure.Repositories
{
  public class MicrocontrollersRepository : RepositoryBase<Microcontroller>, IMicrocontrollersRepository
  {
    public MicrocontrollersRepository(AppDbContext appDbContext)
      : base(appDbContext)
    { }
  }
}
