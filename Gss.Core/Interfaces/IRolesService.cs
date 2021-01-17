using System;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.DTOs.Role;

namespace Gss.Core.Interfaces
{
  public interface IRolesService
  {
    Task<PagedResultDto<RoleDto>> GetAllRolesAsync(PagedInfoDto pagedInfoDto);
    Task<RoleDto> GetRoleAsync(Guid id);
    Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto);
    Task<RoleDto> UpdateRoleAsync(UpdateRoleDto updateRoleDto);
    Task<RoleDto> DeleteRoleAsync(Guid id);
  }
}
