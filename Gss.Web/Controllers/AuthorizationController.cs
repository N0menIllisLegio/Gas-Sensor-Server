﻿using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.DTOs.Authentication;
using Gss.Core.DTOs.User;
using Gss.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Gss.Web.Controllers
{
  [Route("api/[controller]/[action]")]
  [ApiController]
  public class AuthorizationController : ControllerBase
  {
    private readonly IAuthenticationService _authService;

    public AuthorizationController(IAuthenticationService authService)
    {
      _authService = authService;
    }

    [HttpPost]
    [SwaggerOperation(description: "Registers new user.")]
    [SwaggerResponse(200, type: typeof(Response<object>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Register([FromBody] CreateUserDto registerModel)
    {
      await _authService.RegisterAsync(registerModel);

      return Ok(new Response<object>());
    }

    [HttpPost]
    [SwaggerOperation(description: "Generates access/refresh token pair.")]
    [SwaggerResponse(200, type: typeof(Response<TokenDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> LogIn([FromBody] LoginDto loginModel)
    {
      var tokenDto = await _authService.LogInAsync(loginModel.Login, loginModel.Password);

      return Ok(new Response<TokenDto>(tokenDto));
    }

    [HttpPost]
    [SwaggerOperation(description: "Refreshes access token.")]
    [SwaggerResponse(200, type: typeof(Response<TokenDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> RefreshToken([FromBody] RequestTokenRefreshDto requestTokens)
    {
      var tokenDto = await _authService.RefreshTokenAsync(requestTokens.AccessToken, requestTokens.RefreshToken);

      return Ok(new Response<TokenDto>(tokenDto));
    }

    [Authorize]
    [HttpPost]
    [SwaggerOperation("Authorized", "Removes specified refresh token from database.")]
    [SwaggerResponse(200, type: typeof(Response<object>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> LogOut([FromBody] RequestTokenRefreshDto requestTokens)
    {
      await _authService.LogOutAsync(requestTokens.AccessToken, requestTokens.RefreshToken);

      return Ok(new Response<object>());
    }

    [Authorize]
    [HttpPost]
    [SwaggerOperation("Authorized", "Removes all refresh tokens of user from database.")]
    [SwaggerResponse(200, type: typeof(Response<object>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> LogOutFromAllDevices([FromBody] RequestTokenRefreshDto requestTokens)
    {
      await _authService.RevokeAccessFromAllDevicesAsync(requestTokens.AccessToken);

      return Ok(new Response<object>());
    }
  }
}
