using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Gss.Core.DTOs;
using Gss.Core.Entities;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;
using Gss.Core.Resources;
using Microsoft.AspNetCore.Identity;

namespace Gss.Core.Services
{
  public class AuthService: IAuthService
  {
    private const string _emailConfirmationSubject = "Email Confirmation";

    private readonly ITokenService _tokenService;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager _userManager;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IEmailService _emailService;

    public AuthService(ITokenService tokenService,
      SignInManager<User> signInManager,
      UserManager userManager,
      IRefreshTokenRepository refreshTokenRepository,
      IEmailService emailService)
    {
      _tokenService = tokenService;
      _signInManager = signInManager;
      _userManager = userManager;
      _refreshTokenRepository = refreshTokenRepository;
      _emailService = emailService;
    }

    public async Task<Response<object>> SendEmailConfirmationAsync(string userID, string confirmationUrl)
    {
      var user = await _userManager.FindByIdAsync(userID);

      if (user is null)
      {
        return new Response<object>()
          .AddErrors(String.Format(Messages.NotFoundErrorString, "User"));
      }

      if (user.EmailConfirmed)
      {
        return new Response<object>()
          .AddErrors(String.Format(Messages.EmailAlreadyConfirmedErrorString));
      }

      string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
      confirmationUrl = $"{confirmationUrl}/{HttpUtility.UrlEncode(token)}";
      bool sendSuccessfully = await _emailService.SendTextEmailAsync(user.Email, _emailConfirmationSubject, confirmationUrl);

      return sendSuccessfully
        ? new Response<object> { Succeeded = true }
        : new Response<object>()
          .AddErrors(Messages.FailedToSendEmailConfirmationErrorString);
    }

    public async Task<Response<object>> ConfirmEmailAsync(string userID, string token)
    {
      var user = await _userManager.FindByIdAsync(userID);

      if (user is null)
      {
        return new Response<object>()
          .AddErrors(String.Format(Messages.NotFoundErrorString, "User"));
      }

      var result = await _userManager.ConfirmEmailAsync(user, token);

      return result.Succeeded
          ? new Response<object>() { Succeeded = true }
          : new Response<object>()
            .AddErrors(result.Errors.Select(r => r.Description));
    }

    public async Task<Response<object>> RegisterAsync(CreateUserDto newUserDto)
    {
      var user = new User
      {
        Email = newUserDto.Email,
        FirstName = newUserDto.FirstName,
        LastName = newUserDto.LastName,
        Gender = newUserDto.Gender,
        Birthday = newUserDto.Birthday,
        PhoneNumber = newUserDto.PhoneNumber
      };

      var result = await _userManager.CreateAsync(user, newUserDto.Password);

      return result.Succeeded
          ? new Response<object>() { Succeeded = true }
          : new Response<object>()
            .AddErrors(result.Errors.Select(r => r.Description));
    }

    public async Task<Response<TokenDto>> LogInAsync(string login, string password)
    {
      var user = await _userManager.FindByEmailAsync(login);
      var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, false);

      if (!signInResult.Succeeded)
      {
        string errorMessage = signInResult.IsNotAllowed
          ? Messages.EmailNotConfirmedErrorString
            : signInResult.IsLockedOut
          ? Messages.UserIsLockedOutErrorString
          : Messages.InvalidEmailOrPasswordErrorString;

        return new Response<TokenDto>()
          .AddErrors(errorMessage);
      }

      var token = await _tokenService.GenerateTokenAsync(user);
      await _refreshTokenRepository.AddRefreshTokenAsync(user, token.RefreshToken);

      return new Response<TokenDto>(token);
    }

    public async Task<Response<TokenDto>> RefreshTokenAsync(string accessToken, string refreshToken)
    {
      string accountEmail = _tokenService.GetEmailFromAccessToken(accessToken);
      var token = await _refreshTokenRepository.GetRefreshTokenAsync(accountEmail, refreshToken);

      if (token is null)
      {
        return new Response<TokenDto>()
          .AddErrors(Messages.RefreshTokenNotExistsErrorString);
      }

      if (token.ExpirationDate < DateTime.UtcNow)
      {
        return new Response<TokenDto>()
          .AddErrors(Messages.RefreshTokenExpiredErrorString);
      }

      await _refreshTokenRepository.DeleteRefreshTokenAsync(token);
      var user = await _userManager.FindByEmailAsync(accountEmail);
      var newToken = await _tokenService.GenerateTokenAsync(user);
      await _refreshTokenRepository.AddRefreshTokenAsync(user, newToken.RefreshToken);

      return new Response<TokenDto>(newToken);
    }

    public async Task LogOutAsync(string accessToken, string refreshToken)
    {
      string accountEmail = _tokenService.GetEmailFromAccessToken(accessToken);
      var token = await _refreshTokenRepository.GetRefreshTokenAsync(accountEmail, refreshToken);

      if (token is not null)
      {
        await _refreshTokenRepository.DeleteRefreshTokenAsync(token);
      }
    }

    public async Task RevokeAccessFromAllDevicesAsync(string accessToken)
    {
      string accountEmail = _tokenService.GetEmailFromAccessToken(accessToken);
      var user = await _userManager.FindByEmailAsync(accountEmail);

      if (user is not null && user.RefreshTokens.Count > 0)
      {
        await _refreshTokenRepository.DeleteAllUsersRefreshTokens(user);
      }
    }
  }
}
