using System;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.DTOs.User;

namespace Gss.Core.Interfaces
{
  public interface IUsersService
  {
    Task<PagedResultDto<ExtendedUserDto>> GetAllUsersAsync(PagedInfoDto pagedInfoDto);
    Task<UserDto> GetUserAsync(Guid userID);
    Task<ExtendedUserDto> GetUserAsync(string userEmail);
    Task<ExtendedUserDto> AddUserAsync(CreateUserDto createUserDto);
  }
}
