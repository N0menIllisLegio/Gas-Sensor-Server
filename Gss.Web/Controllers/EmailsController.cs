using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.Interfaces;
using Gss.Core.Resources;
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

    private readonly IAuthService _authService;

    public EmailsController(IAuthService authService)
    {
      _authService = authService;
    }

    [Authorize]
    [HttpGet("{newEmail}")]
    [SwaggerOperation("Authorized Only", "Sends message to specified email with email change confirmation token.")]
    [SwaggerResponse(200, type: typeof(Response<object>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
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
      // string url = Url.RouteUrl(_emailConfirmationRouteName, new { userID = String.Empty, token = String.Empty }, Request.Scheme);
      string url = $"{Request.Scheme}://{Request.Host}/api/SendEmailChangeConfirmation";
      var response = await _authService.SendEmailChangeConfirmationAsync(email, newEmail, url);

      return response.Succeeded
        ? Ok(response)
        : BadRequest(response);
    }

    [HttpGet("{email}")]
    [SwaggerOperation(description: "Sends message to specified email with reset password token.")]
    [SwaggerResponse(200, type: typeof(Response<object>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> SendResetPasswordConfirmation(string email)
    {
      // TODO actually this url should lead to front which will call API
      // this may lead to change in AuthService url params generation
      // string url = Url.RouteUrl(_emailConfirmationRouteName, new { userID = String.Empty, token = String.Empty }, Request.Scheme);
      string url = $"{Request.Scheme}://{Request.Host}/api/SendResetPasswordConfirmation";
      var response = await _authService.SendResetPasswordConfirmationAsync(email, url);

      return response.Succeeded
        ? Ok(response)
        : BadRequest(response);
    }

    [HttpGet("{email}")]
    [SwaggerOperation(description: "Sends message to specified email with email confirmation token.")]
    [SwaggerResponse(200, type: typeof(Response<object>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> SendEmailConfirmation(string email)
    {
      // TODO actually this url should lead to front which will call API
      // this may lead to change in AuthService url params generation (in that case remove '?' from attribute below)
      // string url = Url.RouteUrl(_emailConfirmationRouteName, new { userID = String.Empty, token = String.Empty }, Request.Scheme);
      string url = $"{Request.Scheme}://{Request.Host}/api/SendEmailConfirmation";
      var response = await _authService.SendEmailConfirmationAsync(email, url);

      return response.Succeeded
        ? Ok(response)
        : BadRequest(response);
    }
  }
}
