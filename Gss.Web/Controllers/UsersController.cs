﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Gss.Core.DTOs;
using Gss.Core.Entities;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;
using Gss.Core.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gss.Web.Controllers
{
  [Route("api/[controller]/[action]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    private const string _emailConfirmationRouteName = "EmailConfirmation";

    private readonly UserManager _userManager;
    private readonly IAuthService _authService;

    public UsersController(UserManager userManager, IAuthService authService)
    {
      _userManager = userManager;
      _authService = authService;
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserByID(string id)
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
          .AddErrors(String.Format(Messages.NotFoundErrorString, "User")));
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

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserInfoDto newUserInfo)
    {
      var user = await _userManager.FindByEmailAsync(User.Identity.Name);

      user.FirstName = newUserInfo.FirstName;
      user.LastName = newUserInfo.LastName;
      user.Gender = newUserInfo.Gender;
      user.Birthday = newUserInfo.Birthday;
      user.PhoneNumber = newUserInfo.PhoneNumber;

      var result = await _userManager.UpdateAsync(user);

      return result.Succeeded
        ? Ok(new Response<User> { Succeeded = true })
        : BadRequest(new Response<object>()
          .AddErrors(result.Errors.Select(r => r.Description)));
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] CreateUserDto registerModel)
    {
      var result = await _authService.RegisterAsync(registerModel);

      return result.Succeeded
        ? Ok(result)
        : BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> LogIn([FromBody] LoginDto loginModel)
    {
      var response = await _authService.LogInAsync(loginModel.Login, loginModel.Password);

      return response.Succeeded
        ? Ok(response)
        : BadRequest(response);
    }

    [HttpPost]
    public async Task<IActionResult> RefreshToken([FromBody] RequestTokenRefreshDto requestTokens)
    {
      var response = await _authService.RefreshTokenAsync(requestTokens.AccessToken, requestTokens.RefreshToken);

      return response.Succeeded
        ? Ok(response)
        : BadRequest(response);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> LogOut([FromBody] RequestTokenRefreshDto requestTokens)
    {
      await _authService.LogOutAsync(requestTokens.AccessToken, requestTokens.RefreshToken);

      return Ok(new Response<TokenDto>() { Succeeded = true });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> LogOutFromAllDevices([FromBody] RequestTokenRefreshDto requestTokens)
    {
      await _authService.RevokeAccessFromAllDevicesAsync(requestTokens.AccessToken);

      return Ok(new Response<TokenDto>() { Succeeded = true });
    }

    // EMAILS

    [HttpGet("{email}")]
    public async Task<IActionResult> SendEmailConfirmation(string email)
    {
      // TODO actually this url should lead to front which will call API
      // this may lead to change in AuthService url params generation (in that case remove '?' from attribute below)
      string url = Url.RouteUrl(_emailConfirmationRouteName, new { userID = String.Empty, token = String.Empty }, Request.Scheme);
      var response = await _authService.SendEmailConfirmationAsync(email, url);

      return response.Succeeded
        ? Ok(response)
        : BadRequest(response);
    }

    [HttpGet("{userID?}/{token?}", Name = _emailConfirmationRouteName)]
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

    [HttpGet("{email}")]
    public async Task<IActionResult> SendResetPasswordConfirmation(string email)
    {
      // TODO actually this url should lead to front which will call API
      // this may lead to change in AuthService url params generation
      string url = Url.RouteUrl(_emailConfirmationRouteName, new { userID = String.Empty, token = String.Empty }, Request.Scheme);
      var response = await _authService.SendResetPasswordConfirmationAsync(email, url);

      return response.Succeeded
        ? Ok(response)
        : BadRequest(response);
    }

    [HttpPost("{userID}/{token}")]
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
    [HttpGet("{newEmail}")]
    public async Task<IActionResult> SendEmailChangeConfirmation(string newEmail)
    {
      var emailValidator = new EmailAddressAttribute();

      if (!emailValidator.IsValid(newEmail))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidEmailErrorString));
      }

      string email = User.Identity.Name;

      // TODO actually this url should lead to front which will call API
      // this may lead to change in AuthService url params generation
      string url = Url.RouteUrl(_emailConfirmationRouteName, new { userID = String.Empty, token = String.Empty }, Request.Scheme);
      var response = await _authService.SendEmailChangeConfirmationAsync(email, newEmail, url);

      return response.Succeeded
        ? Ok(response)
        : BadRequest(response);
    }

    [Authorize]
    [HttpGet("{userID}/{newEmail}/{token}")]
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