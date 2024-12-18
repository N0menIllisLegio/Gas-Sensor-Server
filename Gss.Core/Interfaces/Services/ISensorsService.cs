using Gss.Core.DTOs;
using Gss.Core.DTOs.Sensor;

namespace Gss.Core.Interfaces.Services;

public interface ISensorsService
{
    Task<PagedResultDto<SensorDto>> GetAllSensorsAsync(PagedInfoDto pagedInfoDto,
        CancellationToken cancellationToken = default);

    Task<SensorDto> GetSensorAsync(Guid sensorId, CancellationToken cancellationToken = default);
    Task<SensorDto> CreateSensorAsync(CreateSensorDto createSensorDto, CancellationToken cancellationToken = default);

    Task UpdateSensorAsync(Guid sensorId, UpdateSensorDto updateSensorDto,
        CancellationToken cancellationToken = default);

    Task DeleteSensorAsync(Guid sensorId, CancellationToken cancellationToken = default);
}