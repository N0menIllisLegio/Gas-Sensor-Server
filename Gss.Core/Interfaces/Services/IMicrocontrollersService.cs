using Gss.Core.DTOs;
using Gss.Core.DTOs.Microcontroller;
using Gss.Core.DTOs.Sensor;
using Gss.Core.Entities;

namespace Gss.Core.Interfaces.Services;

public interface IMicrocontrollersService
{
  Task<PagedResultDto<MicrocontrollerDto>> GetAllMicrocontrollersAsync(PagedInfoDto pagedInfo);
  Task<PagedResultDto<MicrocontrollerDto>> GetPublicMicrocontrollersAsync(PagedInfoDto pagedInfo);
  Task<PagedResultDto<MicrocontrollerDto>> GetUserMicrocontrollersAsync(Guid userId, PagedInfoDto pagedInfo);
  Task<List<MapMicrocontrollerDto>> GetPublicMicrocontrollersMapAsync(MapRequestDto mapRequestDto);
  Task<List<SensorDto>> GetMicrocontrollerSensorsAsync(Guid microcontrollerId);
  Task<MicrocontrollerDto> GetMicrocontrollerAsync(Guid microcontrollerId);

  Task<MicrocontrollerDto> AddMicrocontrollerAsync(CreateMicrocontrollerDto createMicrocontrollerDto);
  Task UpdateMicrocontrollerAsync(Guid microcontrollerId, UpdateMicrocontrollerDto updateMicrocontrollerDto);
  Task DeleteMicrocontrollerAsync(Guid microcontrollerId);

  Task<RequestSensorValueResponseDto> RequestSensorValueAsync(Guid microcontrollerId, Guid microcontrollerSensorId);
  Task SetSensorValueThresholdAsync(Guid microcontrollerSensorId, int? criticalValue);

  Task<Microcontroller?> AuthenticateMicrocontrollersAsync(Guid microcontrollerId, string microcontrollerKey);
}