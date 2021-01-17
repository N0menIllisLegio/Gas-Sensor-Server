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
  public class EmailsController : ControllerBase
  {
    // private const string _emailConfirmationRouteName = "EmailConfirmation";

    private readonly IAuthenticationService _authService;

    public EmailsController(IAuthenticationService authService)
    {
      _authService = authService;
    }

    [Authorize]
    [HttpPost]
    [SwaggerOperation("Authorized Only", "Sends message to specified email with email change confirmation token.")]
    [SwaggerResponse(200, type: typeof(Response<object>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> SendEmailChangeConfirmation([FromBody] EmailDto dto)
    {
      string email = User.Identity.Name;

      // TODO actually this url should lead to front which will call API
      // this may lead to change in AuthService url params generation
      // string url = Url.RouteUrl(_emailConfirmationRouteName, new { userID = String.Empty, token = String.Empty }, Request.Scheme);
      string url = $"{Request.Scheme}://{Request.Host}/api/SendEmailChangeConfirmation";
      await _authService.SendEmailChangeConfirmationAsync(email, dto.Email, url);

      return Ok(new Response<object>());
    }

    [HttpPost]
    [SwaggerOperation(description: "Sends message to specified email with reset password token.")]
    [SwaggerResponse(200, type: typeof(Response<object>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> SendResetPasswordConfirmation([FromBody] EmailDto dto)
    {
      // TODO actually this url should lead to front which will call API
      // this may lead to change in AuthService url params generation
      // string url = Url.RouteUrl(_emailConfirmationRouteName, new { userID = String.Empty, token = String.Empty }, Request.Scheme);
      string url = $"{Request.Scheme}://{Request.Host}/api/SendResetPasswordConfirmation";
      await _authService.SendResetPasswordConfirmationAsync(dto.Email, url);

      return Ok(new Response<object>());
    }

    [HttpPost]
    [SwaggerOperation(description: "Sends message to specified email with email confirmation token.")]
    [SwaggerResponse(200, type: typeof(Response<object>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> SendEmailConfirmation([FromBody] EmailDto dto)
    {
      // TODO actually this url should lead to front which will call API
      // this may lead to change in AuthService url params generation (in that case remove '?' from attribute below)
      // string url = Url.RouteUrl(_emailConfirmationRouteName, new { userID = String.Empty, token = String.Empty }, Request.Scheme);
      string url = $"{Request.Scheme}://{Request.Host}/api/SendEmailConfirmation";
      await _authService.SendEmailConfirmationAsync(dto.Email, url);

      return Ok(new Response<object>());
    }
  }
}
