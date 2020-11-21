using System;
using System.Threading.Tasks;
using Gss.Web.Resources;
using Gss.Web.ViewModels;
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

    [HttpGet]
    public async Task<IActionResult> GetAllRoles()
    {
      var allRoles = await _roleManager.Roles.ToListAsync();

      return Ok(allRoles);
    }

    [HttpGet]
    public async Task<IActionResult> GetRoleById(string id)
    {
      if (!ValidateGuidString(id))
      {
        return BadRequest(Messages.InvalidGuidErrorString);
      }

      var role = await _roleManager.FindByIdAsync(id);

      return Ok(role);
    }

    [HttpGet]
    public async Task<IActionResult> GetRoleByName(string name)
    {
      if (String.IsNullOrEmpty(name))
      {
        return BadRequest(Messages.InvalidRoleNameErrorString);
      }

      var role = await _roleManager.FindByNameAsync(name);

      return Ok(role);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] RoleViewModel role)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var result = await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = role.Name });

      if (!result.Succeeded)
      {
        return BadRequest(result.Errors);
      }

      return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateRole(string id, [FromBody] RoleViewModel newRoleModel)
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

      if (!result.Succeeded)
      {
        return BadRequest(result.Errors);
      }

      return Ok();
    }

    [HttpDelete]
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

      if (!result.Succeeded)
      {
        return BadRequest(result.Errors);
      }

      return Ok();
    }

    private bool ValidateGuidString(string guid)
    {
      return Guid.TryParse(guid, out var result);
    }
  }
}
