using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Gss.Core.DTOs;
using Gss.Core.DTOs.Authentication;
using Gss.Core.DTOs.User;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;
using Gss.Core.Resources;
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
    private readonly IAuthenticationService _authService;
    private readonly IUsersService _usersService;

    public UsersController(UserManager userManager, IAuthenticationService authService, IUsersService usersService)
    {
      _userManager = userManager;
      _authService = authService;
      _usersService = usersService;
    }

    //[Authorize] Role = Administrator
    [HttpPost]
    [SwaggerOperation("Administrator Only", "Gets all users existing in database. Paged.")]
    [SwaggerResponse(200, type: typeof(Response<PagedResultDto<ExtendedUserDto>>))]
    public async Task<IActionResult> GetAllUsers([FromBody] PagedInfoDto pagedRequest)
    {
      var pagedResultDto = await _usersService.GetAllUsersAsync(pagedRequest);

      return Ok(new Response<PagedResultDto<ExtendedUserDto>>(pagedResultDto));
    }

    [Authorize]
    [HttpGet]
    [SwaggerOperation("Authorized Only", "Gets user by id.")]
    [SwaggerResponse(200, type: typeof(Response<UserDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    [SwaggerResponse(404, type: typeof(Response<object>))]
    public async Task<IActionResult> GetUserByID([FromQuery] IdDto dto)
    {
      var userDto = await _usersService.GetUserAsync(dto.ID);

      return Ok(new Response<UserDto>(userDto));
    }

    //[Authorize] Role = Administrator
    [HttpPost]
    [SwaggerOperation("Administrator Only", "Gets user by email.")]
    [SwaggerResponse(200, type: typeof(Response<ExtendedUserDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> GetUserByEmail([FromBody] EmailDto dto)
    {
      var extendedUserDto = await _usersService.GetUserAsync(dto.Email);

      return Ok(new Response<ExtendedUserDto>(extendedUserDto));
    }

    //[Authorize] Role = Administrator
    [HttpPost]
    [SwaggerOperation("Administrator Only", "Creates user.")]
    [SwaggerResponse(200, type: typeof(Response<ExtendedUserDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
      var extendedUserDto = await _usersService.AddUserAsync(dto);

      return Ok(new Response<ExtendedUserDto>(extendedUserDto));
    }

    //[Authorize] Role = Administrator
    [HttpPut("{id}")]
    [SwaggerOperation("Administrator Only", "Updates user.")]
    [SwaggerResponse(200, type: typeof(Response<ExtendedUserDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    [SwaggerResponse(404, type: typeof(Response<object>))]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateUserDto dto)
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
          .AddError(Messages.NotFoundErrorString, _user));
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
        ? Ok(new Response<ExtendedUserDto>(new ExtendedUserDto(user)))
        : BadRequest(new Response<object>()
          .AddErrors(result.Errors.Select(r => r.Description)));
    }

    //[Authorize] Role = Administrator
    [HttpPatch("{userID}/{newPassword}")]
    [SwaggerOperation("Administrator Only", "Updates user's password.")]
    [SwaggerResponse(200, type: typeof(Response<ExtendedUserDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    [SwaggerResponse(404, type: typeof(Response<object>))]
    public async Task<IActionResult> UpdatePassword(string userID, string newPassword)
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
          .AddError(Messages.NotFoundErrorString, _user));
      }

      string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
      var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

      return result.Succeeded
        ? Ok(new Response<ExtendedUserDto>(new ExtendedUserDto(user)))
        : BadRequest(new Response<object>()
          .AddErrors(result.Errors.Select(r => r.Description)));
    }

    //[Authorize] Role = Administrator
    [HttpPatch("{userID}/{roleName}")]
    [SwaggerOperation("Administrator Only", "Adds role to user.")]
    [SwaggerResponse(200, type: typeof(Response<ExtendedUserDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    [SwaggerResponse(404, type: typeof(Response<object>))]
    public async Task<IActionResult> AddRole(string userID, string roleName)
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
          .AddError(Messages.NotFoundErrorString, _user));
      }

      var result = await _userManager.AddToRoleAsync(user, roleName);

      return result.Succeeded
        ? Ok(new Response<ExtendedUserDto>(new ExtendedUserDto(user)))
        : BadRequest(new Response<object>()
          .AddErrors(result.Errors.Select(r => r.Description)));
    }

    //[Authorize] Role = Administrator
    [HttpPatch("{userID}/{roleName}")]
    [SwaggerOperation("Administrator Only", "Removes role from user.")]
    [SwaggerResponse(200, type: typeof(Response<ExtendedUserDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    [SwaggerResponse(404, type: typeof(Response<object>))]
    public async Task<IActionResult> RemoveRole(string userID, string roleName)
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
          .AddError(Messages.NotFoundErrorString, _user));
      }

      var result = await _userManager.RemoveFromRoleAsync(user, roleName);

      return result.Succeeded
        ? Ok(new Response<ExtendedUserDto>(new ExtendedUserDto(user)))
        : BadRequest(new Response<object>()
          .AddErrors(result.Errors.Select(r => r.Description)));
    }

    //[Authorize] Role = Administrator
    [HttpDelete("{userID}")]
    [SwaggerOperation("Administrator Only", "Deletes user.")]
    [SwaggerResponse(200, type: typeof(Response<ExtendedUserDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    [SwaggerResponse(404, type: typeof(Response<object>))]
    public async Task<IActionResult> Delete(string userID)
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
          .AddError(Messages.NotFoundErrorString, _user));
      }

      var result = await _userManager.DeleteAsync(user);

      return result.Succeeded
        ? Ok(new Response<ExtendedUserDto>(new ExtendedUserDto(user)))
        : BadRequest(new Response<object>()
          .AddErrors(result.Errors.Select(r => r.Description)));
    }

    [Authorize]
    [HttpPut]
    [SwaggerOperation("Authorized Only", "Updates user info.")]
    [SwaggerResponse(200, type: typeof(Response<UserDto>))]
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
        ? Ok(new Response<UserDto>())
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
      var result = await _authService.ConfirmEmailAsync(userID, token);
      var response = new Response<object>(result);

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
      var result = await _authService.ResetPasswordAsync(userID, token, dto.Password);
      var response = new Response<object>(result);

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
      var result = await _authService.ChangeEmailAsync(userID, token, newEmail);
      var response = new Response<object>(result);

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