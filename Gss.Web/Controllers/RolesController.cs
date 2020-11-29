using System;
using System.Linq;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Web.Filters;
using Gss.Web.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gss.Web.Controllers
{
  //[Authorize]
  [Route("api/[controller]/[action]")]
  [ApiController]
  public class RolesController : ControllerBase
  {
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public RolesController(RoleManager<IdentityRole<Guid>> roleManager)
    {
      _roleManager = roleManager;
    }

    [Pagination]
    [HttpGet]
    public async Task<IActionResult> GetAllRoles(int pageNumber, int pageSize,
      bool orderAsc, string filter)
    {
      var filteredRoles = _roleManager.Roles
        .Where(r => r.Name.Contains(filter));

      var allRoles = orderAsc
        ? await filteredRoles.OrderBy(r => r.Name)
          .Skip((pageNumber - 1) * pageSize)
          .Take(pageSize)
          .ToListAsync()
        : await filteredRoles.OrderByDescending(r => r.Name)
          .Skip((pageNumber - 1) * pageSize)
          .Take(pageSize)
          .ToListAsync();

      return Ok(allRoles);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoleByID(string id)
    {
      if (!ValidateGuidString(id))
      {
        return BadRequest(Messages.InvalidGuidErrorString);
      }

      var role = await _roleManager.FindByIdAsync(id);

      return role is null ? NotFound() : Ok(role);
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> GetRoleByName(string name)
    {
      if (String.IsNullOrEmpty(name))
      {
        return BadRequest(String.Format(Messages.EmptyOrNullErrorString, "Role", "name"));
      }

      var role = await _roleManager.FindByNameAsync(name);

      return role is null ? NotFound() : Ok(role);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] RoleDto roleModel)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var role = new IdentityRole<Guid> { Name = roleModel.Name };
      var result = await _roleManager.CreateAsync(role);

      return result.Succeeded
        ? Ok(role)
        : BadRequest(result.Errors);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRole(string id, [FromBody] RoleDto newRoleModel)
    {
      if (!ValidateGuidString(id))
      {
        return BadRequest(Messages.InvalidGuidErrorString);
      }

      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var oldRole = await _roleManager.FindByIdAsync(id);

      if (oldRole is null)
      {
        return NotFound();
      }

      oldRole.Name = newRoleModel.Name;

      var result = await _roleManager.UpdateAsync(oldRole);

      return result.Succeeded
        ? Ok(oldRole)
        : BadRequest(result.Errors);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(string id)
    {
      if (!ValidateGuidString(id))
      {
        return BadRequest(Messages.InvalidGuidErrorString);
      }

      var role = await _roleManager.FindByIdAsync(id);

      if (role is null)
      {
        return NotFound();
      }

      var result = await _roleManager.DeleteAsync(role);

      return result.Succeeded ? Ok() : BadRequest(result.Errors);
    }

    private bool ValidateGuidString(string guid)
    {
      return Guid.TryParse(guid, out var result);
    }
  }
}
