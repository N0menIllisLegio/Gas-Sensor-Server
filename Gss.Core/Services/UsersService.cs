using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Gss.Core.DTOs;
using Gss.Core.DTOs.User;
using Gss.Core.Entities;
using Gss.Core.Exceptions;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;
using Gss.Core.Models;
using Gss.Core.Resources;
using Microsoft.EntityFrameworkCore;

namespace Gss.Core.Services
{
  public class UsersService : IUsersService
  {
    private const string _user = "User";

    private readonly UserManager _userManager;
    private readonly IAuthenticationService _authService;
    private readonly IMapper _mapper;

    public UsersService(UserManager userManager, IAuthenticationService authService, IMapper mapper)
    {
      _userManager = userManager;
      _authService = authService;
      _mapper = mapper;
    }

    public async Task<PagedResultDto<ExtendedUserDto>> GetAllUsersAsync(PagedInfoDto pagedInfoDto)
    {
      var query = _userManager.Users
        .SearchBy(pagedInfoDto.SearchString, user => new { user.FirstName, user.LastName, user.Email }, pagedInfoDto.Filters)
        .AsNoTracking()
        .OrderBy(pagedInfoDto.SortOptions);

      var users = await query.Skip((pagedInfoDto.PageNumber - 1) * pagedInfoDto.PageSize).Take(pagedInfoDto.PageSize).ToListAsync();

      return new PagedResultDto<ExtendedUserDto>
      {
        Items = users.Select(_mapper.Map<ExtendedUserDto>),
        TotalItemsCount = await query.CountAsync(),
        PagedInfo = pagedInfoDto
      };
    }

    public async Task<UserDto> GetUserAsync(Guid userID)
    {
      var user = await _userManager.FindByIdAsync(userID);

      if (user is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _user),
          HttpStatusCode.NotFound);
      }

      return _mapper.Map<UserDto>(user);
    }

    public async Task<ExtendedUserDto> GetUserAsync(string userEmail)
    {
      var user = await _userManager.FindByEmailAsync(userEmail);

      if (user is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _user),
          HttpStatusCode.NotFound);
      }

      return _mapper.Map<ExtendedUserDto>(user);
    }

    public async Task<ExtendedUserDto> AddUserAsync(CreateUserDto createUserDto)
    {
      var user = _mapper.Map<User>(createUserDto);

      var result = await _userManager.CreateAsync(user, createUserDto.Password);

      if (!result.Succeeded)
      {
        string errorMessage = String.Join(", ", result.Errors.Select(r => r.Description));
        throw new AppException(errorMessage, HttpStatusCode.BadRequest);
      }

      return _mapper.Map<ExtendedUserDto>(user);
    }

    public async Task<ExtendedUserDto> UpdateUserAsync(UpdateUserDto updateUserDto)
    {
      var user = await _userManager.FindByIdAsync(updateUserDto.ID);

      if (user is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _user),
          HttpStatusCode.NotFound);
      }

      user.FirstName = updateUserDto.FirstName;
      user.LastName = updateUserDto.LastName;
      user.Gender = updateUserDto.Gender;
      user.Birthday = updateUserDto.Birthday;
      user.PhoneNumber = updateUserDto.PhoneNumber;

      var result = await _userManager.UpdateAsync(user);

      if (!result.Succeeded)
      {
        throw new AppException(String.Join(", ", result.Errors.Select(r => r.Description)),
          HttpStatusCode.BadRequest);
      }

      string token = await _userManager.GenerateChangeEmailTokenAsync(user, updateUserDto.Email);
      result = await _userManager.ChangeEmailAsync(user, updateUserDto.Email, token);

      if (!result.Succeeded)
      {
        throw new AppException(String.Join(", ", result.Errors.Select(r => r.Description)),
          HttpStatusCode.BadRequest);
      }

      return _mapper.Map<ExtendedUserDto>(user);
    }

    public async Task<ExtendedUserDto> UpdatePasswordAsync(UpdatePasswordDto updatePasswordDto)
    {
      var user = await _userManager.FindByIdAsync(updatePasswordDto.UserID);

      if (user is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _user),
          HttpStatusCode.NotFound);
      }

      string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
      var result = await _userManager.ResetPasswordAsync(user, resetToken, updatePasswordDto.NewPassword);

      if (!result.Succeeded)
      {
        throw new AppException(String.Join(", ", result.Errors.Select(r => r.Description)),
          HttpStatusCode.BadRequest);
      }

      return _mapper.Map<ExtendedUserDto>(user);
    }

    public async Task<ExtendedUserDto> SetUserRoleAsync(SetUserRoleDto setUserRoleDto)
    {
      var user = await _userManager.FindByIdAsync(setUserRoleDto.UserID);

      if (user is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _user),
          HttpStatusCode.NotFound);
      }

      var roles = await _userManager.GetRolesAsync(user);

      if (roles is not null && roles.Count > 0)
      {
        await _userManager.RemoveFromRolesAsync(user, roles);
      }

      var result = await _userManager.AddToRoleAsync(user, setUserRoleDto.RoleName);

      if (!result.Succeeded)
      {
        throw new AppException(String.Join(", ", result.Errors.Select(r => r.Description)),
          HttpStatusCode.BadRequest);
      }

      return _mapper.Map<ExtendedUserDto>(user);
    }

    public async Task<ExtendedUserDto> DeleteUserAsync(Guid userID)
    {
      var user = await _userManager.FindByIdAsync(userID);

      if (user is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _user),
          HttpStatusCode.NotFound);
      }

      var result = await _userManager.DeleteAsync(user);

      if (!result.Succeeded)
      {
        throw new AppException(String.Join(", ", result.Errors.Select(r => r.Description)),
          HttpStatusCode.BadRequest);
      }

      return _mapper.Map<ExtendedUserDto>(user);
    }

    public async Task<UserDto> UpdateUserInfoAsync(UpdateUserInfoModel updateUserInfoModel)
    {
      var user = await _userManager.FindByEmailAsync(updateUserInfoModel.Email);

      user.FirstName = updateUserInfoModel.FirstName;
      user.LastName = updateUserInfoModel.LastName;
      user.Gender = updateUserInfoModel.Gender;
      user.Birthday = updateUserInfoModel.Birthday;
      user.PhoneNumber = updateUserInfoModel.PhoneNumber;

      var result = await _userManager.UpdateAsync(user);

      if (!result.Succeeded)
      {
        throw new AppException(String.Join(", ", result.Errors.Select(r => r.Description)),
          HttpStatusCode.BadRequest);
      }

      return _mapper.Map<UserDto>(user);
    }
  }
}
