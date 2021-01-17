using System;
using System.Linq;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.DTOs.Role;
using Gss.Core.Helpers;
using Gss.Core.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace Gss.Web.Controllers
{
  // TODO [Authorize] Role = Administrator
  // TODO 200 -> 201
  [Route("api/[controller]/[action]")]
  [ApiController]
  public class RolesController : ControllerBase
  {
    private const string _role = "Role";
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public RolesController(RoleManager<IdentityRole<Guid>> roleManager)
    {
      _roleManager = roleManager;
    }

    [HttpGet]
    [SwaggerOperation("Administrator Only", "Gets all roles existing in database. Paged. OrderedBy and FilteredBy is const - Name.")]
    [SwaggerResponse(200, type: typeof(PagedResponse<IdentityRole<Guid>>))]
    public async Task<IActionResult> GetAllRoles([FromQuery] PagedInfoDto pagedRequest)
    {
      var (pagedRolesQuery, totalRolesQuery) = _roleManager.Roles.AsQueryable()
        .GetPage(pagedRequest.PageNumber, pagedRequest.PageSize,
          pagedRequest.SortOrder,
          (role) => role.Name,
          (role) => role.Name.Contains(pagedRequest.Filter));

      var pagedRoles = await pagedRolesQuery.AsNoTracking().ToListAsync();

      var response = new PagedResponse<IdentityRole<Guid>>(pagedRoles, pagedRequest.PageNumber, pagedRequest.PageSize)
      {
        TotalRecords = await totalRolesQuery.CountAsync(),
        OrderedBy = nameof(IdentityRole<Guid>.Name),
        SortOrder = pagedRequest.SortOrder,
        Filter = pagedRequest.Filter,
        FilteredBy = nameof(IdentityRole<Guid>.Name)
      };

      return Ok(response);
    }

    [HttpGet("{roleID}")]
    [SwaggerOperation("Administrator Only", "Gets role by id.")]
    [SwaggerResponse(200, type: typeof(Response<IdentityRole<Guid>>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    [SwaggerResponse(404, type: typeof(Response<object>))]
    public async Task<IActionResult> GetRoleByID(string roleID)
    {
      if (!ValidateGuidString(roleID))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      var role = await _roleManager.FindByIdAsync(roleID);

      if (role is null)
      {
        return NotFound(new Response<object>()
          .AddError(Messages.NotFoundErrorString, _role));
      }

      return Ok(new Response<IdentityRole<Guid>>(role));
    }

    [HttpGet("{roleName}")]
    [SwaggerOperation("Administrator Only", "Gets role by name.")]
    [SwaggerResponse(200, type: typeof(Response<IdentityRole<Guid>>))]
    [SwaggerResponse(404, type: typeof(Response<object>))]
    public async Task<IActionResult> GetRoleByName(string roleName)
    {
      var role = await _roleManager.FindByNameAsync(roleName);

      if (role is null)
      {
        return NotFound(new Response<object>()
          .AddError(Messages.NotFoundErrorString, _role));
      }

      return Ok(new Response<IdentityRole<Guid>>(role));
    }

    [HttpPost]
    [SwaggerOperation("Administrator Only", "Creates role.")]
    [SwaggerResponse(200, type: typeof(Response<IdentityRole<Guid>>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    [SwaggerResponse(404, type: typeof(Response<object>))]
    public async Task<IActionResult> Create([FromBody] RoleDto dto)
    {
      var role = new IdentityRole<Guid> { Name = dto.Name };
      var result = await _roleManager.CreateAsync(role);

      return result.Succeeded
        ? Ok(new Response<IdentityRole<Guid>>(role))
        : BadRequest(new Response<object>()
          .AddErrors(result.Errors.Select(r => r.Description)));
    }

    [HttpPut("{roleID}")]
    [SwaggerOperation("Administrator Only", "Updates role.")]
    [SwaggerResponse(200, type: typeof(Response<IdentityRole<Guid>>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    [SwaggerResponse(404, type: typeof(Response<object>))]
    public async Task<IActionResult> Update(string roleID, [FromBody] RoleDto dto)
    {
      if (!ValidateGuidString(roleID))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      var oldRole = await _roleManager.FindByIdAsync(roleID);

      if (oldRole is null)
      {
        return NotFound(new Response<object>()
          .AddError(Messages.NotFoundErrorString, _role));
      }

      oldRole.Name = dto.Name;

      var result = await _roleManager.UpdateAsync(oldRole);

      return result.Succeeded
        ? Ok(new Response<IdentityRole<Guid>>(oldRole))
        : BadRequest(new Response<object>()
          .AddErrors(result.Errors.Select(r => r.Description)));
    }

    [HttpDelete("{roleID}")]
    [SwaggerOperation("Administrator Only", "Deletes role.")]
    [SwaggerResponse(200, type: typeof(Response<IdentityRole<Guid>>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    [SwaggerResponse(404, type: typeof(Response<object>))]
    public async Task<IActionResult> Delete(string roleID)
    {
      if (!ValidateGuidString(roleID))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      var role = await _roleManager.FindByIdAsync(roleID);

      if (role is null)
      {
        return NotFound(new Response<object>()
          .AddError(Messages.NotFoundErrorString, _role));
      }

      var result = await _roleManager.DeleteAsync(role);

      return result.Succeeded
        ? Ok(new Response<IdentityRole<Guid>>())
        : BadRequest(new Response<object>()
          .AddErrors(result.Errors.Select(r => r.Description)));
    }

    private bool ValidateGuidString(string guid)
    {
      return Guid.TryParse(guid, out _);
    }
  }
}
