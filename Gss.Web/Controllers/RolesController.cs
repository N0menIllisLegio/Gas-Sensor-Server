using System;
using System.Linq;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.Helpers;
using Gss.Core.Resources;
using Gss.Web.Filters;
using Microsoft.AspNetCore.Identity;
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
    private const string _role = "Role";
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public RolesController(RoleManager<IdentityRole<Guid>> roleManager)
    {
      _roleManager = roleManager;
    }

    [Pagination]
    [HttpGet]
    [SwaggerOperation("Administrator Only", "Gets all roles existing in database. Paged.")]
    [SwaggerResponse(200, type: typeof(PagedResponse<IdentityRole<Guid>>))]
    public async Task<IActionResult> GetAllRoles(int pageNumber, int pageSize,
      bool orderAsc = false, string filter = "")
    {
      var pagedRoles = await _roleManager.GetPage(pageNumber, pageSize, orderAsc, filter);

      var response = new PagedResponse<IdentityRole<Guid>>(pagedRoles, pageNumber, pageSize)
      {
        TotalRecords = _roleManager.Roles.Count(),
        OrderedBy = "Name",
        OrderedByAscendind = orderAsc,
        Filter = filter,
        FilteredBy = "Name"
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
          .AddErrors(String.Format(Messages.NotFoundErrorString, _role)));
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
          .AddErrors(String.Format(Messages.NotFoundErrorString, _role)));
      }

      return Ok(new Response<IdentityRole<Guid>>(role));
    }

    [HttpPost]
    [SwaggerOperation("Administrator Only", "Creates role.")]
    [SwaggerResponse(200, type: typeof(Response<IdentityRole<Guid>>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    [SwaggerResponse(404, type: typeof(Response<object>))]
    public async Task<IActionResult> CreateRole([FromBody] RoleDto dto)
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
    public async Task<IActionResult> UpdateRole(string roleID, [FromBody] RoleDto dto)
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
          .AddErrors(String.Format(Messages.NotFoundErrorString, _role)));
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
    public async Task<IActionResult> DeleteRole(string roleID)
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
          .AddErrors(String.Format(Messages.NotFoundErrorString, _role)));
      }

      var result = await _roleManager.DeleteAsync(role);

      return result.Succeeded
        ? Ok(new Response<IdentityRole<Guid>> { Succeeded = true })
        : BadRequest(new Response<object>()
          .AddErrors(result.Errors.Select(r => r.Description)));
    }

    private bool ValidateGuidString(string guid)
    {
      return Guid.TryParse(guid, out _);
    }
  }
}
