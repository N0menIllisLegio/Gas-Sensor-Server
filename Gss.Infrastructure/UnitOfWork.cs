using Gss.Core.Interfaces;
using Gss.Core.Interfaces.Repositories;
using Gss.Infrastructure.Repositories;

namespace Gss.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    private bool _disposedValue;

    private IMicrocontrollersRepository? _microcontrollers;
    private ISensorsRepository? _sensors;
    private ISensorsDataRepository? _sensorsData;
    private ISensorsTypesRepository? _sensorsTypes;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IMicrocontrollersRepository Microcontrollers =>
        _microcontrollers ??= new MicrocontrollersRepository(_context);

    public ISensorsRepository Sensors => _sensors ??= new SensorsRepository(_context);
    public ISensorsTypesRepository SensorsTypes => _sensorsTypes ??= new SensorsTypesRepository(_context);
    public ISensorsDataRepository SensorsData => _sensorsData ??= new SensorsDataRepository(_context);

    public async Task<bool> SaveAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing) _context.Dispose();

            _disposedValue = true;
        }
    }

    ~UnitOfWork()
    {
        Dispose(false);
    }
}