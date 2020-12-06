using System;
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
      if (!Guid.TryParse(id, out var result))
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

    [HttpPost("{userID}")]
    public async Task<IActionResult> SendConfirmationEmail(string userID)
    {
      if (!Guid.TryParse(userID, out var result))
      {
        return BadRequest(new Response<object>()
          .AddErrors(Messages.InvalidGuidErrorString));
      }

      string url = Url.RouteUrl(_emailConfirmationRouteName, new { userID, token = String.Empty }, Request.Scheme);
      var response = await _authService.SendEmailConfirmationAsync(userID, url);

      return response.Succeeded
        ? Ok(response)
        : BadRequest(response);
    }

    [HttpGet("{userID}/{token?}", Name = _emailConfirmationRouteName)]
    public async Task<IActionResult> ConfirmEmail(string userID, string token)
    {
      if (!Guid.TryParse(userID, out var result))
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
  }
}