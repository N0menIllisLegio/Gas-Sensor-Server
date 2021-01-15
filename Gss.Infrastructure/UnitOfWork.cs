using System.Threading.Tasks;
using Gss.Core.Interfaces;
using Gss.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Gss.Infrastructure
{
  public class UnitOfWork: IUnitOfWork
  {
    private readonly AppDbContext _context;
    private IAzureImagesRepository _azureImages;
    private IMicrocontrollersRepository _microcontrollers;
    private IRefreshTokensRepository _refreshTokens;
    private ISensorsRepository _sensors;
    private ISensorsTypesRepository _sensorsTypes;
    private bool _disposedValue = false;

    public UnitOfWork(AppDbContext context)
    {
      _context = context;
    }

    public IAzureImagesRepository AzureImages => _azureImages = _azureImages ?? new AzureImagesRepository();
    public IMicrocontrollersRepository Microcontrollers => _microcontrollers = _microcontrollers ?? new MicrocontrollersRepository(_context);
    public IRefreshTokensRepository RefreshTokens => _refreshTokens = _refreshTokens ?? new RefreshTokensRepository(_context);
    public ISensorsRepository Sensors => _sensors = _sensors ?? new SensorsRepository(_context);
    public ISensorsTypesRepository SensorsTypes => _sensorsTypes = _sensorsTypes ?? new SensorsTypesRepository(_context);

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
          // TODO: dispose managed state (managed objects).
          _context.Dispose();
        }

        // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
        // TODO: set large fields to null.

        _disposedValue = true;
      }
    }

    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    // ~UnitOfWork()
    // {
    //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //   Dispose(false);
    // }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(true);
      // TODO: uncomment the following line if the finalizer is overridden above.
      // GC.SuppressFinalize(this);
    }
  }
}
