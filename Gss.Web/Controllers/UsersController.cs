using System;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.Entities;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;
using Gss.Web.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gss.Web.Controllers
{
  [Route("api/[controller]/[action]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
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
      if (!Guid.TryParse(id, out var result))
      {
        return BadRequest(Messages.InvalidGuidErrorString);
      }

      var user = await _userManager.FindByIdAsync(id);

      if (user is null)
      {
        return NotFound();
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

      return Ok(userInfoDto);
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserInfoDto newUserInfo)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var user = await _userManager.FindByEmailAsync(User.Identity.Name);

      user.FirstName = newUserInfo.FirstName;
      user.LastName = newUserInfo.LastName;
      user.Gender = newUserInfo.Gender;
      user.Birthday = newUserInfo.Birthday;
      user.PhoneNumber = newUserInfo.PhoneNumber;

      var result = await _userManager.UpdateAsync(user);

      if (!result.Succeeded)
      {
        return BadRequest(result.Errors);
      }

      return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] CreateUserDto registerModel)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var user = new User
      {
        Email = registerModel.Email,
        FirstName = registerModel.FirstName,
        LastName = registerModel.LastName,
        Gender = registerModel.Gender,
        Birthday = registerModel.Birthday,
        PhoneNumber = registerModel.PhoneNumber
      };

      var result = await _userManager.CreateAsync(user, registerModel.Password);

      if (!result.Succeeded)
      {
        return BadRequest(result.Errors);
      }

      var token = await _authService.LogInAsync(user.Email, registerModel.Password);

      return token is not null ? Ok(token) : BadRequest();
    }

    [HttpPost]
    public async Task<IActionResult> LogIn([FromBody] LoginDto loginModel)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var token = await _authService.LogInAsync(loginModel.Login, loginModel.Password);

      return token is not null
        ? Ok(token)
        : BadRequest(Messages.InvalidPasswordErrorString);
    }

    [HttpPost]
    public async Task<IActionResult> RefreshToken([FromBody] RequestTokenRefreshDto requestTokens)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var result = await _authService.RefreshTokenAsync(requestTokens.AccessToken, requestTokens.RefreshToken);

      return result is null
        ? BadRequest()
        : Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> LogOut([FromBody] RequestTokenRefreshDto requestTokens)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      await _authService.LogOutAsync(requestTokens.AccessToken, requestTokens.RefreshToken);

      return Ok();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> LogOutFromAllDevices([FromBody] RequestTokenRefreshDto requestTokens)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      await _authService.RevokeAccessFromAllDevicesAsync(requestTokens.AccessToken);

      return Ok();
    }
  }
}