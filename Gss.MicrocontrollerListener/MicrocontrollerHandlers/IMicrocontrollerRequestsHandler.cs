using Gss.Core.Entities;

namespace Gss.MicrocontrollerListener.MicrocontrollerHandlers;

internal interface IMicrocontrollerRequestsHandler
{
    Task<Microcontroller?> HandleRequestAsync(AuthRequest authRequest, CancellationToken cancellationToken = default);
    SensorData? HandleRequest(DataRequest dataRequest, Microcontroller connectedMicrocontroller, CancellationToken cancellationToken = default);
    Task<SensorData> HandleRequestGetSensorDataAsync(DataRequest dataRequest, Microcontroller connectedMicrocontroller, CancellationToken cancellationToken = default);
}