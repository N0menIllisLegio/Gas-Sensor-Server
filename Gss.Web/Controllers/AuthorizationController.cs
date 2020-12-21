using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Gss.Web.Controllers
{
  [Route("api/[controller]/[action]")]
  [ApiController]
  public class AuthorizationController : ControllerBase
  {
    private readonly IAuthService _authService;

    public AuthorizationController(IAuthService authService)
    {
      _authService = authService;
    }

    [HttpPost]
    [SwaggerOperation(description: "Adds new user to database.")]
    [SwaggerResponse(200, type: typeof(Response<object>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Register([FromBody] CreateUserDto registerModel)
    {
      var result = await _authService.RegisterAsync(registerModel);
      var response = new Response<object>(result);

      return response.Succeeded
        ? Ok(response)
        : BadRequest(response);
    }

    [HttpPost]
    [SwaggerOperation(description: "Generates access/refresh token pair.")]
    [SwaggerResponse(200, type: typeof(Response<TokenDto>))]
    [SwaggerResponse(400, type: typeof(Response<TokenDto>))]
    public async Task<IActionResult> LogIn([FromBody] LoginDto loginModel)
    {
      var result = await _authService.LogInAsync(loginModel.Login, loginModel.Password);
      var response = new Response<TokenDto>(result);

      return response.Succeeded
        ? Ok(response)
        : BadRequest(response);
    }

    [HttpPost]
    [SwaggerOperation(description: "Refreshes access token.")]
    [SwaggerResponse(200, type: typeof(Response<TokenDto>))]
    [SwaggerResponse(400, type: typeof(Response<TokenDto>))]
    public async Task<IActionResult> RefreshToken([FromBody] RequestTokenRefreshDto requestTokens)
    {
      var result = await _authService.RefreshTokenAsync(requestTokens.AccessToken, requestTokens.RefreshToken);
      var response = new Response<TokenDto>(result);

      return response.Succeeded
        ? Ok(response)
        : BadRequest(response);
    }

    [Authorize]
    [HttpPost]
    [SwaggerOperation("Authorized Only", "Removes specified refresh token from database.")]
    [SwaggerResponse(200, type: typeof(Response<object>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> LogOut([FromBody] RequestTokenRefreshDto requestTokens)
    {
      await _authService.LogOutAsync(requestTokens.AccessToken, requestTokens.RefreshToken);

      return Ok(new Response<object>());
    }

    [Authorize]
    [HttpPost]
    [SwaggerOperation("Authorized Only", "Removes all refresh tokens of user from database.")]
    [SwaggerResponse(200, type: typeof(Response<object>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> LogOutFromAllDevices([FromBody] RequestTokenRefreshDto requestTokens)
    {
      await _authService.RevokeAccessFromAllDevicesAsync(requestTokens.AccessToken);

      return Ok(new Response<object>());
    }
  }
}
