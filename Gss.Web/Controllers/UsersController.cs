using System.Threading.Tasks;
using AutoMapper;
using Gss.Core.DTOs;
using Gss.Core.DTOs.Authentication;
using Gss.Core.DTOs.User;
using Gss.Core.Interfaces;
using Gss.Core.Models;
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
    private readonly IAuthenticationService _authService;
    private readonly IUsersService _usersService;
    private readonly IMapper _mapper;

    public UsersController(IAuthenticationService authService, IUsersService usersService, IMapper mapper)
    {
      _authService = authService;
      _usersService = usersService;
      _mapper = mapper;
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
    [HttpPut]
    [SwaggerOperation("Administrator Only", "Updates user.")]
    [SwaggerResponse(200, type: typeof(Response<ExtendedUserDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    [SwaggerResponse(404, type: typeof(Response<object>))]
    public async Task<IActionResult> Update([FromBody] UpdateUserDto dto)
    {
      var extendedUserDto = await _usersService.UpdateUserAsync(dto);

      return Ok(new Response<ExtendedUserDto>(extendedUserDto));
    }

    //[Authorize] Role = Administrator
    [HttpPatch]
    [SwaggerOperation("Administrator Only", "Updates user's password.")]
    [SwaggerResponse(200, type: typeof(Response<ExtendedUserDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    [SwaggerResponse(404, type: typeof(Response<object>))]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto dto)
    {
      var extendedUserDto = await _usersService.UpdatePasswordAsync(dto);

      return Ok(new Response<ExtendedUserDto>(extendedUserDto));
    }

    //[Authorize] Role = Administrator
    [HttpPatch]
    [SwaggerOperation("Administrator Only", "Adds role to user.")]
    [SwaggerResponse(200, type: typeof(Response<ExtendedUserDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    [SwaggerResponse(404, type: typeof(Response<object>))]
    public async Task<IActionResult> SetRole([FromBody] SetUserRoleDto dto)
    {
      var extendedUserDto = await _usersService.SetUserRoleAsync(dto);

      return Ok(new Response<ExtendedUserDto>(extendedUserDto));
    }

    //[Authorize] Role = Administrator
    [HttpDelete]
    [SwaggerOperation("Administrator Only", "Deletes user.")]
    [SwaggerResponse(200, type: typeof(Response<ExtendedUserDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    [SwaggerResponse(404, type: typeof(Response<object>))]
    public async Task<IActionResult> Delete([FromBody] IdDto dto)
    {
      var extendedUserDto = await _usersService.DeleteUserAsync(dto.ID);

      return Ok(new Response<ExtendedUserDto>(extendedUserDto));
    }

    [Authorize]
    [HttpPut]
    [SwaggerOperation("Authorized Only", "Updates authorized user info.")]
    [SwaggerResponse(200, type: typeof(Response<UserDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserInfoDto dto)
    {
      var updateUserInfoModel = _mapper.Map<UpdateUserInfoModel>(dto);

      updateUserInfoModel.Email = User.Identity.Name;

      var userDto = await _usersService.UpdateUserInfoAsync(updateUserInfoModel);

      return Ok(new Response<UserDto>(userDto));
    }

    [HttpPost]
    [SwaggerOperation(description: "Confirms user's email. Token sends to unconirmed email.")]
    [SwaggerResponse(200, type: typeof(Response<object>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailDto dto)
    {
      await _authService.ConfirmEmailAsync(dto);

      return Ok(new Response<object>());
    }

    [HttpPost]
    [SwaggerOperation(description: "Changes user's password. Token send to confirmed email.")]
    [SwaggerResponse(200, type: typeof(Response<object>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
      await _authService.ResetPasswordAsync(dto);

      return Ok(new Response<object>());
    }

    [Authorize]
    [HttpPost]
    [SwaggerOperation("Authorized Only", "Changes user's email. Token sends to old email.")]
    [SwaggerResponse(200, type: typeof(Response<object>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailDto dto)
    {
      await _authService.ChangeEmailAsync(dto);

      return Ok(new Response<object>());
    }
  }
}