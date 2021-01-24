using System;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.DTOs.User;
using Gss.Core.Models;

namespace Gss.Core.Interfaces.Services
{
  public interface IUsersService
  {
    Task<PagedResultDto<ExtendedUserDto>> GetAllUsersAsync(PagedInfoDto pagedInfoDto);
    Task<UserDto> GetUserAsync(Guid userID);
    Task<ExtendedUserDto> GetUserAsync(string userEmail);
    Task<ExtendedUserDto> AddUserAsync(CreateUserDto createUserDto);
    Task<ExtendedUserDto> UpdateUserAsync(Guid userID, UpdateUserDto updateUserDto);
    Task<ExtendedUserDto> UpdatePasswordAsync(UpdatePasswordDto updatePasswordDto);
    Task<ExtendedUserDto> SetUserRoleAsync(SetUserRoleDto setUserRoleDto);
    Task<ExtendedUserDto> DeleteUserAsync(Guid userID);
    Task<UserDto> UpdateUserInfoAsync(UpdateUserInfoModel updateUserInfoModel);
  }
}
