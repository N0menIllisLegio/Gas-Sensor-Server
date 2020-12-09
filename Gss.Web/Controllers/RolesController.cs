using System;
using System.Linq;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;
using Gss.Core.Resources;
using Gss.Web.Filters;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Gss.Web.Controllers
{
  //TODO [Authorize]
  [Route("api/[controller]/[action]")]
  [ApiController]
  public class RolesController : ControllerBase
  {
    private const string _role = "Role";
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IEmailService _emailService;

    public RolesController(RoleManager<IdentityRole<Guid>> roleManager, IEmailService emailService)
    {
      _roleManager = roleManager;
      _emailService = emailService;
    }

    [Pagination]
    [HttpGet]
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoleByID(string id)
    {
      if (!ValidateGuidString(id))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      var role = await _roleManager.FindByIdAsync(id);

      if (role is null)
      {
        return NotFound(new Response<object>()
          .AddErrors(String.Format(Messages.NotFoundErrorString, _role)));
      }

      return Ok(new Response<IdentityRole<Guid>>(role));
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> GetRoleByName(string name)
    {
      var role = await _roleManager.FindByNameAsync(name);

      if (role is null)
      {
        return NotFound(new Response<object>()
          .AddErrors(String.Format(Messages.NotFoundErrorString, _role)));
      }

      return Ok(new Response<IdentityRole<Guid>>(role));
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] RoleDto roleModel)
    {
      var role = new IdentityRole<Guid> { Name = roleModel.Name };
      var result = await _roleManager.CreateAsync(role);

      return result.Succeeded
        ? Ok(new Response<IdentityRole<Guid>>(role))
        : BadRequest(new Response<object>()
          .AddErrors(result.Errors.Select(r => r.Description)));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRole(string id, [FromBody] RoleDto newRoleModel)
    {
      if (!ValidateGuidString(id))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      var oldRole = await _roleManager.FindByIdAsync(id);

      if (oldRole is null)
      {
        return NotFound(new Response<object>()
          .AddErrors(String.Format(Messages.NotFoundErrorString, _role)));
      }

      oldRole.Name = newRoleModel.Name;

      var result = await _roleManager.UpdateAsync(oldRole);

      return result.Succeeded
        ? Ok(new Response<IdentityRole<Guid>>(oldRole))
        : BadRequest(new Response<object>()
          .AddErrors(result.Errors.Select(r => r.Description)));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(string id)
    {
      if (!ValidateGuidString(id))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      var role = await _roleManager.FindByIdAsync(id);

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
