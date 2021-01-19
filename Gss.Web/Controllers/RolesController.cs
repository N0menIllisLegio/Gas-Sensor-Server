using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.DTOs.Role;
using Gss.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Gss.Web.Controllers
{
  // TODO [Authorize] Role = Administrator
  // TODO 200 -> 201
  [Route("api/[controller]/[action]")]
  [ApiController]
  public class RolesController : ControllerBase
  {
    private readonly IRolesService _rolesService;

    public RolesController(IRolesService rolesService)
    {
      _rolesService = rolesService;
    }

    [HttpPost]
    [SwaggerOperation("Administrator Only", "Gets all roles existing in database. Paged. OrderedBy and FilteredBy is const - Name.")]
    [SwaggerResponse(200, type: typeof(Response<PagedResultDto<RoleDto>>))]
    public async Task<IActionResult> GetAllRoles([FromBody] PagedInfoDto pagedRequest)
    {
      var pagedResult = await _rolesService.GetAllRolesAsync(pagedRequest);

      return Ok(new Response<PagedResultDto<RoleDto>>(pagedResult));
    }

    [HttpGet]
    [SwaggerOperation("Administrator Only", "Gets role by id.")]
    [SwaggerResponse(200, type: typeof(Response<RoleDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    [SwaggerResponse(404, type: typeof(Response<object>))]
    public async Task<IActionResult> GetRole([FromQuery] IdDto dto)
    {
      var roleDto = await _rolesService.GetRoleAsync(dto.ID);

      return Ok(new Response<RoleDto>(roleDto));
    }

    [HttpPost]
    [SwaggerOperation("Administrator Only", "Creates role.")]
    [SwaggerResponse(200, type: typeof(Response<RoleDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    [SwaggerResponse(404, type: typeof(Response<object>))]
    public async Task<IActionResult> Create([FromBody] CreateRoleDto dto)
    {
      var roleDto = await _rolesService.CreateRoleAsync(dto);

      return Ok(new Response<RoleDto>(roleDto));
    }

    [HttpPut("{roleID}")]
    [SwaggerOperation("Administrator Only", "Updates role.")]
    [SwaggerResponse(200, type: typeof(Response<RoleDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    [SwaggerResponse(404, type: typeof(Response<object>))]
    public async Task<IActionResult> Update([FromBody] UpdateRoleDto dto)
    {
      var roleDto = await _rolesService.UpdateRoleAsync(dto);

      return Ok(new Response<RoleDto>(roleDto));
    }

    [HttpDelete("{roleID}")]
    [SwaggerOperation("Administrator Only", "Deletes role.")]
    [SwaggerResponse(200, type: typeof(Response<RoleDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    [SwaggerResponse(404, type: typeof(Response<object>))]
    public async Task<IActionResult> Delete(IdDto dto)
    {
      var roleDto = await _rolesService.DeleteRoleAsync(dto.ID);

      return Ok(new Response<RoleDto>(roleDto));
    }
  }
}
