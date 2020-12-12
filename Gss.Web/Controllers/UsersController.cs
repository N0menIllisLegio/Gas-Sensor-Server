using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Gss.Core.DTOs;
using Gss.Core.Entities;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;
using Gss.Core.Resources;
using Gss.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Gss.Web.Controllers
{
  // TODO 200 -> 201
  [Route("api/[controller]/[action]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    private const string _user = "User";

    private readonly UserManager _userManager;
    private readonly IAuthService _authService;

    public UsersController(UserManager userManager, IAuthService authService)
    {
      _userManager = userManager;
      _authService = authService;
    }

    //[Authorize] Role = Administrator
    [Pagination]
    [HttpGet]
    [SwaggerOperation("Administrator Only", "Gets all users existing in database. Paged.")]
    [SwaggerResponse(200, type: typeof(PagedResponse<IEnumerable<User>>))]
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

    [Authorize]
    [HttpGet("{userID}")]
    [SwaggerOperation("Authorized Only", "Gets user by id.")]
    [SwaggerResponse(200, type: typeof(Response<UserInfoDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    [SwaggerResponse(404, type: typeof(Response<object>))]
    public async Task<IActionResult> GetUserByID(string userID)
    {
      if (!ValidateGuidString(userID))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      var user = await _userManager.FindByIdAsync(userID);

      if (user is null)
      {
        return NotFound(new Response<object>()
          .AddErrors(String.Format(Messages.NotFoundErrorString, _user)));
      }

      var userInfoDto = new UserInfoDto
      {
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Gender = user.Gender,
        Birthday = user.Birthday,
        AvatarPath = user.AvatarPath
      };

      return Ok(new Response<UserInfoDto>(userInfoDto));
    }

    //[Authorize] Role = Administrator
    [HttpGet("{email}")]
    [SwaggerOperation("Administrator Only", "Gets user by email.")]
    [SwaggerResponse(200, type: typeof(Response<User>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
      var user = await _userManager.FindByEmailAsync(email);

      return user is not null
        ? Ok(new Response<User>(user))
        : NotFound(new Response<object>()
          .AddErrors(String.Format(Messages.NotFoundErrorString, _user)));
    }

    //[Authorize] Role = Administrator
    [HttpPost]
    [SwaggerOperation("Administrator Only", "Creates user.")]
    [SwaggerResponse(200, type: typeof(Response<User>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
      var user = new User
      {
        Email = dto.Email,
        PhoneNumber = dto.PhoneNumber,
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        Gender = dto.Gender,
        Birthday = dto.Birthday
      };

      var result = await _userManager.CreateAsync(user, dto.Password);

      return result.Succeeded
        ? Ok(new Response<User>(user))
        : BadRequest(new Response<object>()
          .AddErrors(result.Errors.Select(r => r.Description)));
    }

    //[Authorize] Role = Administrator
    [HttpPut("{id}")]
    [SwaggerOperation("Administrator Only", "Updates user.")]
    [SwaggerResponse(200, type: typeof(Response<User>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    [SwaggerResponse(404, type: typeof(Response<object>))]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto dto)
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

      user.FirstName = dto.FirstName;
      user.LastName = dto.LastName;
      user.Gender = dto.Gender;
      user.Birthday = dto.Birthday;
      user.PhoneNumber = dto.PhoneNumber;

      var result = await _userManager.UpdateAsync(user);

      if (!result.Succeeded)
      {
        return BadRequest(new Response<object>()
          .AddErrors(result.Errors.Select(r => r.Description)));
      }

      string token = await _userManager.GenerateChangeEmailTokenAsync(user, dto.Email);
      result = await _userManager.ChangeEmailAsync(user, dto.Email, token);

      return result.Succeeded
        ? Ok(new Response<User>(user))
        : BadRequest(new Response<object>()
          .AddErrors(result.Errors.Select(r => r.Description)));
    }

    //[Authorize] Role = Administrator
    [HttpPatch("{userID}/{newPassword}")]
    [SwaggerOperation("Administrator Only", "Updates user's password.")]
    [SwaggerResponse(200, type: typeof(Response<User>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    [SwaggerResponse(404, type: typeof(Response<object>))]
    public async Task<IActionResult> UpdateUserPassword(string userID, string newPassword)
    {
      if (!ValidateGuidString(userID))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      var user = await _userManager.FindByIdAsync(userID);

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

    //[Authorize] Role = Administrator
    [HttpPatch("{userID}/{roleName}")]
    [SwaggerOperation("Administrator Only", "Adds role to user.")]
    [SwaggerResponse(200, type: typeof(Response<User>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    [SwaggerResponse(404, type: typeof(Response<object>))]
    public async Task<IActionResult> AddUserRole(string userID, string roleName)
    {
      if (!ValidateGuidString(userID))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      var user = await _userManager.FindByIdAsync(userID);

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

    //[Authorize] Role = Administrator
    [HttpPatch("{userID}/{roleName}")]
    [SwaggerOperation("Administrator Only", "Removes role from user.")]
    [SwaggerResponse(200, type: typeof(Response<User>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    [SwaggerResponse(404, type: typeof(Response<object>))]
    public async Task<IActionResult> RemoveUserRole(string userID, string roleName)
    {
      if (!ValidateGuidString(userID))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      var user = await _userManager.FindByIdAsync(userID);

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

    //[Authorize] Role = Administrator
    [HttpDelete("{userID}")]
    [SwaggerOperation("Administrator Only", "Deletes user.")]
    [SwaggerResponse(200, type: typeof(Response<User>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    [SwaggerResponse(404, type: typeof(Response<object>))]
    public async Task<IActionResult> DeleteUser(string userID)
    {
      if (!ValidateGuidString(userID))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      var user = await _userManager.FindByIdAsync(userID);

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

    [Authorize]
    [HttpPut]
    [SwaggerOperation("Authorized Only", "Updates user info.")]
    [SwaggerResponse(200, type: typeof(Response<User>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserInfoDto dto)
    {
      var user = await _userManager.FindByEmailAsync(User.Identity.Name);

      user.FirstName = dto.FirstName;
      user.LastName = dto.LastName;
      user.Gender = dto.Gender;
      user.Birthday = dto.Birthday;
      user.PhoneNumber = dto.PhoneNumber;

      var result = await _userManager.UpdateAsync(user);

      return result.Succeeded
        ? Ok(new Response<User> { Succeeded = true })
        : BadRequest(new Response<object>()
          .AddErrors(result.Errors.Select(r => r.Description)));
    }

    [HttpPost("{userID?}/{token?}")]
    [SwaggerOperation(description: "Confirms user's email. Token sends to unconirmed email.")]
    [SwaggerResponse(200, type: typeof(Response<object>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> ConfirmEmail(string userID, string token)
    {
      if (!ValidateGuidString(userID))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      if (String.IsNullOrEmpty(token))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidEmailConfirmationTokenErrorString));
      }

      token = HttpUtility.UrlDecode(token).Replace(' ', '+');
      var response = await _authService.ConfirmEmailAsync(userID, token);

      return response.Succeeded
        ? Ok(response)
        : BadRequest(response);
    }

    [HttpPost("{userID}/{token}")]
    [SwaggerOperation(description: "Changes user's password. Token sends to confirmed email.")]
    [SwaggerResponse(200, type: typeof(Response<object>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> ChangePassword(string userID, string token, [FromBody] ChangePasswordDto dto)
    {
      if (!ValidateGuidString(userID))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      if (String.IsNullOrEmpty(token))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidPasswordResetTokenErrorString));
      }

      token = HttpUtility.UrlDecode(token).Replace(' ', '+');
      var response = await _authService.ResetPasswordAsync(userID, token, dto.Password);

      return response.Succeeded
        ? Ok(response)
        : BadRequest(response);
    }

    [Authorize]
    [HttpPost("{userID}/{newEmail}/{token}")]
    [SwaggerOperation("Authorized Only", "Changes user's email. Token sends to old email.")]
    [SwaggerResponse(200, type: typeof(Response<object>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> ChangeEmail(string userID, string newEmail, string token)
    {
      var emailValidator = new EmailAddressAttribute();

      if (!emailValidator.IsValid(newEmail))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidEmailErrorString));
      }

      if (!ValidateGuidString(userID))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      if (String.IsNullOrEmpty(token))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidPasswordResetTokenErrorString));
      }

      token = HttpUtility.UrlDecode(token).Replace(' ', '+');
      var response = await _authService.ChangeEmailAsync(userID, token, newEmail);

      return response.Succeeded
        ? Ok(response)
        : BadRequest(response);
    }

    private bool ValidateGuidString(string guid)
    {
      return Guid.TryParse(guid, out _);
    }
  }
}