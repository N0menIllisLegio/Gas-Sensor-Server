using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.Entities;
using Gss.Core.Helpers;
using Gss.Core.Resources;
using Gss.Web.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Gss.Web.Controllers
{
  //[Authorize] Role = Administrator
  [Route("api/[controller]/[action]")]
  [ApiController]
  public class AdminController: ControllerBase
  {
    private const string _user = "User";
    private readonly UserManager _userManager;

    public AdminController(UserManager userManager)
    {
      _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto userModel)
    {
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
        ? Ok(new Response<User>(user))
        : BadRequest(new Response<object>()
          .AddErrors(result.Errors.Select(r => r.Description)));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto userModel)
    {
      if (!ValidateGuidString(id))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      var user = await _userManager.FindByIdAsync(id);

      if (user is null)
      {
        return NotFound(new Response<object>()
          .AddErrors(String.Format(Messages.NotFoundErrorString, _user)));
      }

      user.FirstName = userModel.FirstName;
      user.LastName = userModel.LastName;
      user.Gender = userModel.Gender;
      user.Birthday = userModel.Birthday;
      user.PhoneNumber = userModel.PhoneNumber;

      var result = await _userManager.UpdateAsync(user);

      if (!result.Succeeded)
      {
        return BadRequest(new Response<object>()
          .AddErrors(result.Errors.Select(r => r.Description)));
      }

      string token = await _userManager.GenerateChangeEmailTokenAsync(user, userModel.Email);
      result = await _userManager.ChangeEmailAsync(user, userModel.Email, token);

      return result.Succeeded
        ? Ok(new Response<User>(user))
        : BadRequest(new Response<object>()
          .AddErrors(result.Errors.Select(r => r.Description)));
    }

    [HttpPatch("{id}/{newPassword}")]
    public async Task<IActionResult> UpdateUserPassword(string id, string newPassword)
    {
      if (!ValidateGuidString(id))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      var user = await _userManager.FindByIdAsync(id);

      if (user is null)
      {
        return NotFound(new Response<object>()
          .AddErrors(String.Format(Messages.NotFoundErrorString, _user)));
      }

      string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
      var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

      return result.Succeeded
        ? Ok(new Response<User>(user))
        : BadRequest(new Response<object>()
          .AddErrors(result.Errors.Select(r => r.Description)));
    }

    [HttpPatch("{id}/{roleName}")]
    public async Task<IActionResult> AddUserRole(string id, string roleName)
    {
      if (!ValidateGuidString(id))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      var user = await _userManager.FindByIdAsync(id);

      if (user is null)
      {
        return NotFound(new Response<object>()
          .AddErrors(String.Format(Messages.NotFoundErrorString, _user)));
      }

      var result = await _userManager.AddToRoleAsync(user, roleName);

      return result.Succeeded
        ? Ok(new Response<User>(user))
        : BadRequest(new Response<object>()
          .AddErrors(result.Errors.Select(r => r.Description)));
    }

    [HttpPatch("{id}/{roleName}")]
    public async Task<IActionResult> RemoveUserRole(string id, string roleName)
    {
      if (!ValidateGuidString(id))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      var user = await _userManager.FindByIdAsync(id);

      if (user is null)
      {
        return NotFound(new Response<object>()
          .AddErrors(String.Format(Messages.NotFoundErrorString, _user)));
      }

      var result = await _userManager.RemoveFromRoleAsync(user, roleName);

      return result.Succeeded
        ? Ok(new Response<User>(user))
        : BadRequest(new Response<object>()
          .AddErrors(result.Errors.Select(r => r.Description)));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
      if (!ValidateGuidString(id))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      var user = await _userManager.FindByIdAsync(id);

      if (user is null)
      {
        return NotFound(new Response<object>()
          .AddErrors(String.Format(Messages.NotFoundErrorString, _user)));
      }

      var result = await _userManager.DeleteAsync(user);

      return result.Succeeded
        ? Ok(new Response<User>(user))
        : BadRequest(new Response<object>()
          .AddErrors(result.Errors.Select(r => r.Description)));
    }

    [Pagination]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers(int pageNumber, int pageSize,
      bool orderAsc = false, string orderBy = "", string filterBy = null, string filter = "")
    {
      var users = await _userManager
        .GetPage(pageSize, pageNumber, orderAsc, orderBy, filterBy, filter);

      var response = new PagedResponse<IEnumerable<User>>(users, pageNumber, pageSize)
      {
        TotalRecords = _userManager.Users.Count(),
        OrderedBy = orderBy,
        OrderedByAscendind = orderAsc,
        Filter = filter,
        FilteredBy = filterBy
      };

      return Ok(response);
    }

    [HttpGet("{email}")]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
      var user = await _userManager.FindByEmailAsync(email);

      return user is not null
        ? Ok(new Response<User>(user))
        : NotFound(new Response<object>()
          .AddErrors(String.Format(Messages.NotFoundErrorString, _user)));
    }

    private bool ValidateGuidString(string guid)
    {
      return Guid.TryParse(guid, out var result);
    }
  }
}
