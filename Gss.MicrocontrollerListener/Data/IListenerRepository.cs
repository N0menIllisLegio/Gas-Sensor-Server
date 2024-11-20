using Gss.Core.Entities;

namespace Gss.MicrocontrollerListener.Data;

internal interface IListenerRepository
{
    Task ResetMicrocontrollerRequestSensorValueAsync(Guid microcontrollerId);
    Task BulkInsertIfNotExistsAsync(List<SensorData> sensorData, CancellationToken cancellationToken = default);
    Task<Microcontroller?> GetMicrocontrollerAsync(Guid microcontrollerId);
    Task UpdateLastResponseTimeAsync(Guid microcontrollerId);
}