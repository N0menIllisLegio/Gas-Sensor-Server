﻿using System;
using System.Threading.Tasks;
using AutoMapper;
using Gss.Core.DTOs;
using Gss.Core.DTOs.Authentication;
using Gss.Core.DTOs.User;
using Gss.Core.Interfaces.Services;
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

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    [SwaggerOperation("Administrator Only", "Gets all users existing in database. Paged.")]
    [SwaggerResponse(200, type: typeof(Response<PagedResultDto<ExtendedUserDto>>))]
    public async Task<IActionResult> GetAllUsers([FromBody] PagedInfoDto pagedRequest)
    {
      var pagedResultDto = await _usersService.GetAllUsersAsync(pagedRequest);

      return Ok(new Response<PagedResultDto<ExtendedUserDto>>(pagedResultDto));
    }

    [HttpGet("{id}")]
    [SwaggerOperation(description: "Gets user by id.")]
    [SwaggerResponse(200, type: typeof(Response<UserDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    [SwaggerResponse(404, type: typeof(Response<object>))]
    public async Task<IActionResult> GetUserByID([FromRoute] Guid id)
    {
      var userDto = await _usersService.GetUserAsync(id);

      return Ok(new Response<UserDto>(userDto));
    }

    [Authorize]
    [HttpGet("{id}")]
    [SwaggerOperation("Administrator Only", "Gets user by id.")]
    [SwaggerResponse(200, type: typeof(Response<ExtendedUserDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> GetExtendedUserByID([FromRoute] Guid id)
    {
      string requestedBy = User.Identity.Name;
      var extendedUserDto = await _usersService.GetExtendedUserAsync(requestedBy, id);

      return Ok(new Response<ExtendedUserDto>(extendedUserDto));
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    [SwaggerOperation("Administrator Only", "Creates user.")]
    [SwaggerResponse(200, type: typeof(Response<ExtendedUserDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
      var extendedUserDto = await _usersService.AddUserAsync(dto);

      return Ok(new Response<ExtendedUserDto>(extendedUserDto));
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id}")]
    [SwaggerOperation("Administrator Only", "Updates user.")]
    [SwaggerResponse(200, type: typeof(Response<ExtendedUserDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    [SwaggerResponse(404, type: typeof(Response<object>))]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateUserDto dto)
    {
      var extendedUserDto = await _usersService.UpdateUserAsync(id, dto);

      return Ok(new Response<ExtendedUserDto>(extendedUserDto));
    }

    [Authorize(Roles = "Administrator")]
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

    [Authorize(Roles = "Administrator")]
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

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id}")]
    [SwaggerOperation("Administrator Only", "Deletes user.")]
    [SwaggerResponse(200, type: typeof(Response<ExtendedUserDto>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    [SwaggerResponse(404, type: typeof(Response<object>))]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
      var extendedUserDto = await _usersService.DeleteUserAsync(id);

      return Ok(new Response<ExtendedUserDto>(extendedUserDto));
    }

    [Authorize]
    [HttpPut]
    [SwaggerOperation("Authorized", "Updates authorized user info.")]
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
    [SwaggerOperation("Authorized", "Changes user's email. Token sends to old email.")]
    [SwaggerResponse(200, type: typeof(Response<object>))]
    [SwaggerResponse(400, type: typeof(Response<object>))]
    public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailDto dto)
    {
      await _authService.ChangeEmailAsync(dto);

      return Ok(new Response<object>());
    }
  }
}