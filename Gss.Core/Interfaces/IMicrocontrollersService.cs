using System;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.DTOs.Microcontroller;

namespace Gss.Core.Interfaces
{
  public interface IMicrocontrollersService
  {
    Task<PagedResultDto<MicrocontrollerDto>> GetPublicMicrocontrollersAsync(PagedInfoDto pagedInfo);
    Task<PagedResultDto<MicrocontrollerDto>> GetAllMicrocontrollersAsync(PagedInfoDto pagedInfo);
    Task<PagedResultDto<MicrocontrollerDto>> GetUserMicrocontrollersAsync(Guid userID, string requestedByEmail, PagedInfoDto pagedInfo);
    Task<MicrocontrollerDto> GetMicrocontrollerAsync(Guid microcontrollerID, string requestedByEmail);
    Task<MicrocontrollerDto> AddMicrocontrollerAsync(CreateMicrocontrollerDto dto, string ownerEmail);
    Task<MicrocontrollerDto> UpdateMicrocontrollerAsync(UpdateMicrocontrollerDto dto, string ownerEmail);
    Task<MicrocontrollerDto> DeleteMicrocontrollerAsync(Guid microcontrollerID, string ownerEmail);
    Task<MicrocontrollerDto> ChangeMicrocontrollerOwnerAsync(Guid microcontrollerID, Guid newOwnerID);
  }
}
