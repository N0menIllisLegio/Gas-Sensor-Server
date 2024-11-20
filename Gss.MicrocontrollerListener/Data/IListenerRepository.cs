using Gss.Core.Entities;

namespace Gss.MicrocontrollerListener.Data;

internal interface IListenerRepository
{
    Task ResetMicrocontrollerRequestSensorValueAsync(Guid microcontrollerId, CancellationToken cancellationToken = default);
    Task BulkInsertIfNotExistsAsync(List<SensorData> sensorData, CancellationToken cancellationToken = default);
    Task<Microcontroller?> GetMicrocontrollerAsync(Guid microcontrollerId, CancellationToken cancellationToken = default);
    Task UpdateLastResponseTimeAsync(Guid microcontrollerId, CancellationToken cancellationToken = default);
}