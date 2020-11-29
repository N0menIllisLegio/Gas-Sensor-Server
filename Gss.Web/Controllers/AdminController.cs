using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.Entities;
using Gss.Core.Helpers;
using Gss.Web.Filters;
using Gss.Web.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Gss.Web.Controllers
{
  //[Authorize] Role = Administrator
  [Route("api/[controller]/[action]")]
  [ApiController]
  public class AdminController: ControllerBase
  {
    private readonly UserManager _userManager;

    public AdminController(UserManager userManager)
    {
      _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto userModel)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var user = new User
      {
        Email = userModel.Email,
        PhoneNumber = userModel.PhoneNumber,
        FirstName = userModel.FirstName,
        LastName = userModel.LastName,
        Gender = userModel.Gender,
        Birthday = userModel.Birthday
      };

      var result = await _userManager.CreateAsync(user, userModel.Password);

      return result.Succeeded
        ? Ok(user)
        : BadRequest(result.Errors);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto userModel)
    {
      if (!ValidateGuidString(id))
      {
        return BadRequest(Messages.InvalidGuidErrorString);
      }

      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var user = await _userManager.FindByIdAsync(id);

      if (user is null)
      {
        return NotFound();
      }

      user.FirstName = userModel.FirstName;
      user.LastName = userModel.LastName;
      user.Gender = userModel.Gender;
      user.Birthday = userModel.Birthday;
      user.PhoneNumber = userModel.PhoneNumber;

      var result = await _userManager.UpdateAsync(user);

      if (!result.Succeeded)
      {
        return BadRequest(result.Errors);
      }

      string token = await _userManager.GenerateChangeEmailTokenAsync(user, userModel.Email);
      result = await _userManager.ChangeEmailAsync(user, userModel.Email, token);

      return result.Succeeded
        ? Ok(user)
        : BadRequest(result.Errors);
    }

    [HttpPatch("{id}/{newPassword}")]
    public async Task<IActionResult> UpdateUserPassword(string id, string newPassword)
    {
      if (!ValidateGuidString(id))
      {
        return BadRequest(Messages.InvalidGuidErrorString);
      }

      var user = await _userManager.FindByIdAsync(id);

      if (user is null)
      {
        return NotFound();
      }

      string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
      var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

      return result.Succeeded
        ? Ok(user)
        : BadRequest(result.Errors);
    }

    [HttpPatch("{id}/{roleName}")]
    public async Task<IActionResult> AddUserRole(string id, string roleName)
    {
      if (!ValidateGuidString(id))
      {
        return BadRequest(Messages.InvalidGuidErrorString);
      }

      var user = await _userManager.FindByIdAsync(id);

      if (user is null)
      {
        return NotFound();
      }

      var result = await _userManager.AddToRoleAsync(user, roleName);

      return result.Succeeded
        ? Ok(user)
        : BadRequest(result.Errors);
    }

    [HttpPatch("{id}/{roleName}")]
    public async Task<IActionResult> RemoveUserRole(string id, string roleName)
    {
      if (!ValidateGuidString(id))
      {
        return BadRequest(Messages.InvalidGuidErrorString);
      }

      var user = await _userManager.FindByIdAsync(id);

      if (user is null)
      {
        return NotFound();
      }

      var result = await _userManager.RemoveFromRoleAsync(user, roleName);

      return result.Succeeded
        ? Ok(user)
        : BadRequest(result.Errors);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
      if (!ValidateGuidString(id))
      {
        return BadRequest(Messages.InvalidGuidErrorString);
      }

      var user = await _userManager.FindByIdAsync(id);

      if (user is null)
      {
        return NotFound();
      }

      var result = await _userManager.DeleteAsync(user);

      return result.Succeeded
        ? Ok(user)
        : BadRequest(result.Errors);
    }

    [Pagination]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers(int pageNumber, int pageSize,
      bool orderAsc, string orderBy, string filterBy, string filter)
    {
      var users = await _userManager
        .GetPage(pageSize, pageNumber, orderAsc, orderBy, filterBy, filter);

      return Ok(users);
    }

    [HttpGet("{email}")]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
      if (String.IsNullOrEmpty(email))
      {
        return BadRequest(String.Format(Messages.EmptyOrNullErrorString, "User", "email"));
      }

      var user = await _userManager.FindByEmailAsync(email);

      return user is null ? NotFound() : Ok(user);
    }

    private bool ValidateGuidString(string guid)
    {
      return Guid.TryParse(guid, out var result);
    }
  }
}
