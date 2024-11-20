using Gss.Core.DTOs;
using Gss.Core.DTOs.Microcontroller;
using Gss.Core.DTOs.Sensor;

namespace Gss.Core.Interfaces.Services;

public interface IMicrocontrollersService
{
  Task<PagedResultDto<MicrocontrollerDto>> GetAllMicrocontrollersAsync(PagedInfoDto pagedInfo, CancellationToken cancellationToken = default);
  Task<PagedResultDto<MicrocontrollerDto>> GetPublicMicrocontrollersAsync(PagedInfoDto pagedInfo, CancellationToken cancellationToken = default);
  Task<PagedResultDto<MicrocontrollerDto>> GetUserMicrocontrollersAsync(Guid userId, PagedInfoDto pagedInfo, CancellationToken cancellationToken = default);
  Task<List<MapMicrocontrollerDto>> GetPublicMicrocontrollersMapAsync(MapRequestDto mapRequestDto, CancellationToken cancellationToken = default);
  Task<List<SensorDto>> GetMicrocontrollerSensorsAsync(Guid microcontrollerId, CancellationToken cancellationToken = default);
  Task<MicrocontrollerDto> GetMicrocontrollerAsync(Guid microcontrollerId, CancellationToken cancellationToken = default);

  Task<MicrocontrollerDto> AddMicrocontrollerAsync(CreateMicrocontrollerDto createMicrocontrollerDto, CancellationToken cancellationToken = default);
  Task UpdateMicrocontrollerAsync(Guid microcontrollerId, UpdateMicrocontrollerDto updateMicrocontrollerDto, CancellationToken cancellationToken = default);
  Task DeleteMicrocontrollerAsync(Guid microcontrollerId, CancellationToken cancellationToken = default);

  Task<RequestSensorValueResponseDto> RequestSensorValueAsync(Guid microcontrollerId, Guid microcontrollerSensorId, CancellationToken cancellationToken = default);
  Task SetSensorValueThresholdAsync(Guid microcontrollerSensorId, int? criticalValue, CancellationToken cancellationToken = default);
}