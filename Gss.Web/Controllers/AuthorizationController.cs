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

      return result.Succeeded
        ? Ok(result)
        : BadRequest(result);
    }

    [HttpPost]
    [SwaggerOperation(description: "Generates access/refresh token pair.")]
    [SwaggerResponse(200, type: typeof(Response<TokenDto>))]
    [SwaggerResponse(400, type: typeof(Response<TokenDto>))]
    public async Task<IActionResult> LogIn([FromBody] LoginDto loginModel)
    {
      var response = await _authService.LogInAsync(loginModel.Login, loginModel.Password);

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
      var response = await _authService.RefreshTokenAsync(requestTokens.AccessToken, requestTokens.RefreshToken);

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

      return Ok(new Response<object>() { Succeeded = true });
    }

    [Authorize]
    [HttpPost]
    [SwaggerOperation("Authorized Only", "Removes all refresh tokens of user from database.")]
    [SwaggerResponse(200, type: typeof(Response<object>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> LogOutFromAllDevices([FromBody] RequestTokenRefreshDto requestTokens)
    {
      await _authService.RevokeAccessFromAllDevicesAsync(requestTokens.AccessToken);

      return Ok(new Response<object>() { Succeeded = true });
    }
  }
}
