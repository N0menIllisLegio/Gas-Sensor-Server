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
  public class AuthenticationService: IAuthenticationService
  {
    private const string _emailConfirmationSubject = "Email confirmation";
    private const string _emailChangeSubject = "Email change";
    private const string _passwordResetSubject = "Password reset";

    private readonly ITokensService _tokenService;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager _userManager;
    private readonly IRefreshTokensRepository _refreshTokenRepository;
    private readonly IEmailService _emailService;

    public AuthenticationService(ITokensService tokenService,
      SignInManager<User> signInManager,
      UserManager userManager,
      IRefreshTokensRepository refreshTokenRepository,
      IEmailService emailService)
    {
      _tokenService = tokenService;
      _signInManager = signInManager;
      _userManager = userManager;
      _refreshTokenRepository = refreshTokenRepository;
      _emailService = emailService;
    }

    public async Task<ServiceResultDto<object>> SendEmailConfirmationAsync(string email, string confirmationUrl)
    {
      var user = await _userManager.FindByEmailAsync(email);

      if (user is null)
      {
        return new ServiceResultDto<object>()
          .AddError(Messages.NotFoundErrorString, "User");
      }

      if (user.EmailConfirmed)
      {
        return new ServiceResultDto<object>()
          .AddError(Messages.EmailAlreadyConfirmedErrorString);
      }

      string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
      confirmationUrl = $"{confirmationUrl}/{user.Id}/{HttpUtility.UrlEncode(token)}";
      bool sendSuccessfully = await _emailService.SendTextEmailAsync(user.Email, _emailConfirmationSubject, confirmationUrl);

      return sendSuccessfully
        ? new ServiceResultDto<object>()
        : new ServiceResultDto<object>()
          .AddError(Messages.FailedToSendEmailConfirmationErrorString);
    }

    public async Task<ServiceResultDto<object>> ConfirmEmailAsync(string userID, string token)
    {
      var user = await _userManager.FindByIdAsync(userID);

      if (user is null)
      {
        return new ServiceResultDto<object>()
          .AddError(Messages.NotFoundErrorString, "User");
      }

      var result = await _userManager.ConfirmEmailAsync(user, token);

      return result.Succeeded
          ? new ServiceResultDto<object>()
          : new ServiceResultDto<object>()
            .AddErrors(result.Errors.Select(r => r.Description));
    }

    public async Task<ServiceResultDto<object>> SendResetPasswordConfirmationAsync(string email, string redirectUrl)
    {
      var user = await _userManager.FindByEmailAsync(email);

      if (user is null)
      {
        return new ServiceResultDto<object>()
          .AddError(Messages.NotFoundErrorString, "User");
      }

      if (!user.EmailConfirmed)
      {
        return new ServiceResultDto<object>()
          .AddError(Messages.EmailNotConfirmedErrorString);
      }

      string token = await _userManager.GeneratePasswordResetTokenAsync(user);
      redirectUrl = $"{redirectUrl}/{user.Id}/{HttpUtility.UrlEncode(token)}";
      bool sendSuccessfully = await _emailService.SendTextEmailAsync(user.Email, _passwordResetSubject, redirectUrl);

      return sendSuccessfully
        ? new ServiceResultDto<object>()
        : new ServiceResultDto<object>()
          .AddError(Messages.FailedToSendEmailConfirmationErrorString);
    }

    public async Task<ServiceResultDto<object>> ResetPasswordAsync(string userID, string token, string newPassword)
    {
      var user = await _userManager.FindByIdAsync(userID);

      if (user is null)
      {
        return new ServiceResultDto<object>()
          .AddError(Messages.NotFoundErrorString, "User");
      }

      var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

      return result.Succeeded
         ? new ServiceResultDto<object>()
         : new ServiceResultDto<object>()
           .AddErrors(result.Errors.Select(r => r.Description));
    }

    public async Task<ServiceResultDto<object>> SendEmailChangeConfirmationAsync(string email, string newEmail, string confirmationUrl)
    {
      var user = await _userManager.FindByEmailAsync(email);

      if (user is null)
      {
        return new ServiceResultDto<object>()
          .AddError(Messages.NotFoundErrorString, "User");
      }

      if (!user.EmailConfirmed)
      {
        return new ServiceResultDto<object>()
          .AddError(Messages.EmailNotConfirmedErrorString);
      }

      string token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
      confirmationUrl = $"{confirmationUrl}/{user.Id}/{newEmail}/{HttpUtility.UrlEncode(token)}";
      bool sendSuccessfully = await _emailService.SendTextEmailAsync(user.Email, _emailChangeSubject, confirmationUrl);

      return sendSuccessfully
        ? new ServiceResultDto<object>()
        : new ServiceResultDto<object>()
          .AddError(Messages.FailedToSendEmailConfirmationErrorString);
    }

    public async Task<ServiceResultDto<object>> ChangeEmailAsync(string userID, string newEmail, string token)
    {
      var user = await _userManager.FindByIdAsync(userID);

      if (user is null)
      {
        return new ServiceResultDto<object>()
          .AddError(Messages.NotFoundErrorString, "User");
      }

      var result = await _userManager.ChangeEmailAsync(user, newEmail, token);

      return result.Succeeded
         ? new ServiceResultDto<object>()
         : new ServiceResultDto<object>()
           .AddErrors(result.Errors.Select(r => r.Description));
    }

    public async Task<ServiceResultDto<object>> RegisterAsync(CreateUserDto newUserDto)
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
          ? new ServiceResultDto<object>()
          : new ServiceResultDto<object>()
            .AddErrors(result.Errors.Select(r => r.Description));
    }

    public async Task<ServiceResultDto<TokenDto>> LogInAsync(string login, string password)
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

        return new ServiceResultDto<TokenDto>()
          .AddError(errorMessage);
      }

      var token = await _tokenService.GenerateTokenAsync(user);
      await _refreshTokenRepository.AddRefreshTokenAsync(user, token.RefreshToken);

      return new ServiceResultDto<TokenDto>(token);
    }

    public async Task<ServiceResultDto<TokenDto>> RefreshTokenAsync(string accessToken, string refreshToken)
    {
      string accountEmail = _tokenService.GetEmailFromAccessToken(accessToken);
      var token = await _refreshTokenRepository.GetRefreshTokenAsync(accountEmail, refreshToken);

      if (token is null)
      {
        return new ServiceResultDto<TokenDto>()
          .AddError(Messages.RefreshTokenNotExistsErrorString);
      }

      if (token.ExpirationDate < DateTime.UtcNow)
      {
        return new ServiceResultDto<TokenDto>()
          .AddError(Messages.RefreshTokenExpiredErrorString);
      }

      await _refreshTokenRepository.DeleteRefreshTokenAsync(token);
      var user = await _userManager.FindByEmailAsync(accountEmail);
      var newToken = await _tokenService.GenerateTokenAsync(user);
      await _refreshTokenRepository.AddRefreshTokenAsync(user, newToken.RefreshToken);

      return new ServiceResultDto<TokenDto>(newToken);
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
