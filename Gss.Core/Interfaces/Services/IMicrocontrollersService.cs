using System;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.DTOs.Microcontroller;

namespace Gss.Core.Interfaces.Services
{
  public interface IMicrocontrollersService
  {
    Task<PagedResultDto<MicrocontrollerDto>> GetPublicMicrocontrollersAsync(PagedInfoDto pagedInfo);
    Task<PagedResultDto<MicrocontrollerDto>> GetAllMicrocontrollersAsync(PagedInfoDto pagedInfo);
    Task<PagedResultDto<MicrocontrollerDto>> GetUserMicrocontrollersAsync(string requestedByEmail, Guid userID, PagedInfoDto pagedInfo);
    Task<MicrocontrollerDto> GetMicrocontrollerAsync(string requestedByEmail, Guid microcontrollerID);
    Task<MicrocontrollerDto> AddMicrocontrollerAsync(string requestedByEmail, CreateMicrocontrollerDto createMicrocontrollerDto);
    Task<MicrocontrollerDto> UpdateMicrocontrollerAsync(string requestedByEmail, Guid microcontrollerID, UpdateMicrocontrollerDto updateMicrocontrollerDto);
    Task<MicrocontrollerDto> DeleteMicrocontrollerAsync(string requestedByEmail, Guid microcontrollerID);
    Task<MicrocontrollerDto> ChangeMicrocontrollerOwnerAsync(Guid microcontrollerID, Guid newOwnerID);
    Task<MicrocontrollerDto> AddSensorAsync(string requestedByEmail, AddSensorDto addSensorDto);
    Task<MicrocontrollerDto> RemoveSensorAsync(string requestedByEmail, RemoveSensorDto removeSensorDto);
  }
}
