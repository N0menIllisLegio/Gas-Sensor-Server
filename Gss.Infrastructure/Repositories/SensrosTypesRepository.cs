using Gss.Core.Interfaces;

namespace Gss.Infrastructure.Repositories
{
  public class SensrosTypesRepository : ISensrosTypesRepository
  {
    private readonly AppDbContext _appDbContext;

    public SensrosTypesRepository(AppDbContext appDbContext)
    {
      _appDbContext = appDbContext;
    }
  }
}
