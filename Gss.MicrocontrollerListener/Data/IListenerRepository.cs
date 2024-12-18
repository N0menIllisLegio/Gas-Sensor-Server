using Gss.Core.Entities;

namespace Gss.MicrocontrollerListener.Data;

internal interface IListenerRepository
{
    Task<Microcontroller?> GetMicrocontrollerAsync(Guid microcontrollerId, CancellationToken cancellationToken = default);
}