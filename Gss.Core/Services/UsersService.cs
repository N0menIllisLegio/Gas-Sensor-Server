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
  }
}
