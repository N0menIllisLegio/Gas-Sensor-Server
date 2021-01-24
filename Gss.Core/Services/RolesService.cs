using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Gss.Core.DTOs;
using Gss.Core.DTOs.Role;
using Gss.Core.Exceptions;
using Gss.Core.Helpers;
using Gss.Core.Interfaces.Services;
using Gss.Core.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Gss.Core.Services
{
  public class RolesService : IRolesService
  {
    private const string _role = "Role";

    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IMapper _mapper;

    public RolesService(RoleManager<IdentityRole<Guid>> roleManager, IMapper mapper)
    {
      _roleManager = roleManager;
      _mapper = mapper;
    }

    public async Task<PagedResultDto<RoleDto>> GetAllRolesAsync(PagedInfoDto pagedInfoDto)
    {
      var query = _roleManager.Roles
        .SearchBy(pagedInfoDto.SearchString, sensorType => new { sensorType.Name }, pagedInfoDto.Filters)
        .AsNoTracking()
        .OrderBy(pagedInfoDto.SortOptions);

      var roles = await query.Skip((pagedInfoDto.PageNumber - 1) * pagedInfoDto.PageSize).Take(pagedInfoDto.PageSize).ToListAsync();

      return new PagedResultDto<RoleDto>
      {
        Items = roles.Select(_mapper.Map<RoleDto>),
        TotalItemsCount = await query.CountAsync(),
        PagedInfo = pagedInfoDto
      };
    }

    public async Task<RoleDto> GetRoleAsync(Guid roleID)
    {
      var role = await _roleManager.FindByIdAsync(roleID);

      if (role is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _role),
          HttpStatusCode.NotFound);
      }

      return _mapper.Map<RoleDto>(role);
    }

    public async Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto)
    {
      var role = _mapper.Map<IdentityRole<Guid>>(createRoleDto);
      var result = await _roleManager.CreateAsync(role);

      if (!result.Succeeded)
      {
        throw new AppException(String.Format(Messages.CreationFailedErrorString, _role),
          HttpStatusCode.BadRequest);
      }

      return _mapper.Map<RoleDto>(role);
    }

    public async Task<RoleDto> UpdateRoleAsync(Guid roleID, UpdateRoleDto updateRoleDto)
    {
      var role = await _roleManager.FindByIdAsync(roleID);

      if (role is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _role),
          HttpStatusCode.NotFound);
      }

      _mapper.Map(updateRoleDto, role);

      var result = await _roleManager.UpdateAsync(role);

      if (!result.Succeeded)
      {
        throw new AppException(String.Format(Messages.UpdateFailedErrorString, _role),
          HttpStatusCode.BadRequest);
      }

      return _mapper.Map<RoleDto>(role);
    }

    public async Task<RoleDto> DeleteRoleAsync(Guid roleID)
    {
      var role = await _roleManager.FindByIdAsync(roleID);

      if (role is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _role),
          HttpStatusCode.NotFound);
      }

      var result = await _roleManager.DeleteAsync(role);

      if (!result.Succeeded)
      {
        throw new AppException(String.Format(Messages.DeletionFailedErrorString, _role),
          HttpStatusCode.BadRequest);
      }

      return _mapper.Map<RoleDto>(role);
    }
  }
}
