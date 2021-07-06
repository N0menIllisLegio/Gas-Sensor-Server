using System.Threading.Tasks;
using Gss.Core.Interfaces;
using Gss.Core.Interfaces.Repositories;
using Gss.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Gss.Infrastructure
{
  public class UnitOfWork: IUnitOfWork
  {
    private readonly AppDbContext _context;
    private IAzureFilesRepository _azureImages;
    private IMicrocontrollersRepository _microcontrollers;
    private IRefreshTokensRepository _refreshTokens;
    private ISensorsRepository _sensors;
    private ISensorsTypesRepository _sensorsTypes;
    private ISensorsDataRepository _sensorsData;
    private bool _disposedValue = false;

    public UnitOfWork(AppDbContext context)
    {
      _context = context;
    }

    public IAzureFilesRepository AzureFiles => _azureImages ??= new AzureFilesRepository();
    public IMicrocontrollersRepository Microcontrollers => _microcontrollers ??= new MicrocontrollersRepository(_context);
    public IRefreshTokensRepository RefreshTokens => _refreshTokens ??= new RefreshTokensRepository(_context);
    public ISensorsRepository Sensors => _sensors ??= new SensorsRepository(_context);
    public ISensorsTypesRepository SensorsTypes => _sensorsTypes ??= new SensorsTypesRepository(_context);
    public ISensorsDataRepository SensorsData => _sensorsData ??= new SensorsDataRepository(_context);

    public async Task<bool> SaveAsync()
    {
      try
      {
        await _context.SaveChangesAsync();
        return true;
      }
      catch
      {
        return false;
      }
    }

    public IDbContextTransaction BeginTransaction()
    {
      return _context.Database.BeginTransaction();
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!_disposedValue)
      {
        if (disposing)
        {
          _context.Dispose();
        }

        _disposedValue = true;
      }
    }

    // Override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    // ~UnitOfWork()
    // {
    //   Dispose(false);
    // }

    public void Dispose()
    {
      Dispose(true);
      // Uncomment the following line if the finalizer is overridden above.
      // GC.SuppressFinalize(this);
    }
  }
}
