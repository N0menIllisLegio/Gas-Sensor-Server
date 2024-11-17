using Gss.Core.Interfaces;
using Gss.Core.Interfaces.Repositories;
using Gss.Infrastructure.Repositories;

namespace Gss.Infrastructure;

public class UnitOfWork: IUnitOfWork
{
  private readonly AppDbContext _context;

  private IMicrocontrollersRepository? _microcontrollers;
  private ISensorsRepository? _sensors;
  private ISensorsTypesRepository? _sensorsTypes;
  private ISensorsDataRepository? _sensorsData;

  private bool _disposedValue;

  public UnitOfWork(AppDbContext context)
  {
      _context = context;
    }

  public IMicrocontrollersRepository Microcontrollers =>
    _microcontrollers ??= new MicrocontrollersRepository(_context);

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

  ~UnitOfWork()
  {
      Dispose(false);
    }

  public void Dispose()
  {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
}