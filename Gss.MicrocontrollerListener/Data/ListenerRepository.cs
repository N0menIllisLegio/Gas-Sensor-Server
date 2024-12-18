using Gss.Core.Entities;
using Gss.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Gss.MicrocontrollerListener.Data;

internal sealed class ListenerRepository : IListenerRepository
{
    private readonly AppDbContext _appDbContext;

    public ListenerRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<Microcontroller?> GetMicrocontrollerAsync(Guid microcontrollerId, CancellationToken cancellationToken = default)
    {
        return await _appDbContext.Microcontrollers
            .Include(x => x.MicrocontrollerSensors)
                .ThenInclude(x => x.Sensor)
                    .ThenInclude(x => x.Type)
            .FirstOrDefaultAsync(x => x.Id == microcontrollerId, cancellationToken: cancellationToken);
    }

    public async Task UpdateLastResponseTimeAsync(Guid microcontrollerId, CancellationToken cancellationToken = default)
    {
        // TODO: move to handler
        await _appDbContext.Microcontrollers.Where(x => x.Id == microcontrollerId)
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.LastResponseTime, DateTimeOffset.UtcNow), cancellationToken: cancellationToken);
    }
}
