using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using Gss.Core.DTOs.Authentication;
using Gss.Core.DTOs.User;
using Gss.Core.Entities;
using Gss.Core.Exceptions;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;
using Gss.Core.Interfaces.Services;
using Gss.Core.Resources;
using Microsoft.AspNetCore.Identity;

namespace Gss.Core.Services
{
  public class AuthenticationService: IAuthenticationService
  {
    private const string _user = "User";
    private const string _emailConfirmationSubject = "Email confirmation";
    private const string _emailChangeSubject = "Email change";
    private const string _passwordResetSubject = "Password reset";

    private readonly ITokensService _tokenService;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;

    public AuthenticationService(ITokensService tokenService,
      SignInManager<User> signInManager,
      UserManager userManager,
      IUnitOfWork unitOfWork,
      IEmailService emailService,
      IMapper mapper)
    {
      _tokenService = tokenService;
      _signInManager = signInManager;
      _userManager = userManager;
      _unitOfWork = unitOfWork;
      _emailService = emailService;
      _mapper = mapper;
    }

    public async Task SendEmailConfirmationAsync(string email, string confirmationUrl)
    {
      var user = await _userManager.FindByEmailAsync(email);

      if (user is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _user),
          HttpStatusCode.NotFound);
      }

      if (user.EmailConfirmed)
      {
        throw new AppException(Messages.EmailAlreadyConfirmedErrorString,
          HttpStatusCode.BadRequest);
      }

      string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
      confirmationUrl = $"{confirmationUrl}/{user.Id}/{HttpUtility.UrlEncode(token)}";
      bool sendSuccessfully = await _emailService.SendTextEmailAsync(user.Email, _emailConfirmationSubject, confirmationUrl);

      if (!sendSuccessfully)
      {
        throw new AppException(String.Format(Messages.FailedToSendEmailErrorString, "email confirmation"),
          HttpStatusCode.ServiceUnavailable);
      }
    }

    public async Task ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto)
    {
      string token = HttpUtility.UrlDecode(confirmEmailDto.Token).Replace(' ', '+');
      var user = await _userManager.FindByIdAsync(confirmEmailDto.UserID);

      if (user is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _user),
          HttpStatusCode.NotFound);
      }

      var result = await _userManager.ConfirmEmailAsync(user, token);

      if (!result.Succeeded)
      {
        throw new AppException(Messages.InvalidEmailConfirmationTokenErrorString, HttpStatusCode.BadRequest);
      }
    }

    public async Task SendResetPasswordConfirmationAsync(string email, string redirectUrl)
    {
      var user = await _userManager.FindByEmailAsync(email);

      if (user is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _user),
          HttpStatusCode.NotFound);
      }

      if (!user.EmailConfirmed)
      {
        throw new AppException(Messages.EmailNotConfirmedErrorString,
          HttpStatusCode.Forbidden);
      }

      string token = await _userManager.GeneratePasswordResetTokenAsync(user);
      redirectUrl = $"{redirectUrl}/{user.Id}/{HttpUtility.UrlEncode(token)}";
      bool sendSuccessfully = await _emailService.SendTextEmailAsync(user.Email, _passwordResetSubject, redirectUrl);

      if (!sendSuccessfully)
      {
        throw new AppException(String.Format(Messages.FailedToSendEmailErrorString, "password reset token"),
          HttpStatusCode.ServiceUnavailable);
      }
    }

    public async Task ResetPasswordAsync(ChangePasswordDto changePasswordDto)
    {
      changePasswordDto.Token = HttpUtility.UrlDecode(changePasswordDto.Token).Replace(' ', '+');

      var user = await _userManager.FindByIdAsync(changePasswordDto.UserID);

      if (user is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _user),
          HttpStatusCode.NotFound);
      }

      var result = await _userManager.ResetPasswordAsync(user, changePasswordDto.Token, changePasswordDto.Password);

      if (!result.Succeeded)
      {
        throw new AppException(Messages.InvalidPasswordResetTokenErrorString, HttpStatusCode.BadRequest);
      }
    }

    public async Task SendEmailChangeConfirmationAsync(string email, string newEmail, string confirmationUrl)
    {
      var user = await _userManager.FindByEmailAsync(email);

      if (user is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _user),
          HttpStatusCode.NotFound);
      }

      if (!user.EmailConfirmed)
      {
        throw new AppException(Messages.EmailNotConfirmedErrorString,
          HttpStatusCode.Forbidden);
      }

      string token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
      confirmationUrl = $"{confirmationUrl}/{user.Id}/{newEmail}/{HttpUtility.UrlEncode(token)}";
      bool sendSuccessfully = await _emailService.SendTextEmailAsync(user.Email, _emailChangeSubject, confirmationUrl);

      if (!sendSuccessfully)
      {
        throw new AppException(String.Format(Messages.FailedToSendEmailErrorString, "email change confirmation token"),
          HttpStatusCode.ServiceUnavailable);
      }
    }

    public async Task ChangeEmailAsync(ChangeEmailDto changeEmailDto)
    {
      changeEmailDto.Token = HttpUtility.UrlDecode(changeEmailDto.Token).Replace(' ', '+');

      var user = await _userManager.FindByIdAsync(changeEmailDto.UserID);

      if (user is null)
      {
        throw new AppException(String.Format(Messages.NotFoundErrorString, _user),
          HttpStatusCode.NotFound);
      }

      var result = await _userManager.ChangeEmailAsync(user, changeEmailDto.NewEmail, changeEmailDto.Token);

      if (!result.Succeeded)
      {
        throw new AppException(Messages.InvalidEmailChangeTokenErrorString, HttpStatusCode.BadRequest);
      }
    }

    public async Task RegisterAsync(CreateUserDto createUserDto)
    {
      var user = _mapper.Map<User>(createUserDto);
      var result = await _userManager.CreateAsync(user, createUserDto.Password);

      if (!result.Succeeded)
      {
        string errorMessage = String.Join('\n', result.Errors.Select(r => r.Description));
        throw new AppException(errorMessage, HttpStatusCode.BadRequest);
      }
    }

    public async Task<TokenDto> LogInAsync(string login, string password)
    {
      var user = await _userManager.FindByEmailAsync(login);
      SignInResult signInResult;

      try
      {
        signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, false);
      }
      catch
      {
        throw new AppException(Messages.InvalidEmailOrPasswordErrorString, HttpStatusCode.BadRequest);
      }

      if (!signInResult.Succeeded)
      {
        if (signInResult.IsNotAllowed)
        {
          throw new AppException(Messages.EmailNotConfirmedErrorString, HttpStatusCode.Forbidden);
        }

        if (signInResult.IsLockedOut)
        {
          throw new AppException(Messages.UserIsLockedOutErrorString, HttpStatusCode.Forbidden);
        }

        throw new AppException(Messages.InvalidEmailOrPasswordErrorString, HttpStatusCode.BadRequest);
      }

      var token = await _tokenService.GenerateTokenAsync(user);
      await _unitOfWork.RefreshTokens.AddRefreshTokenAsync(user, token.RefreshToken);

      return token;
    }

    public async Task<TokenDto> RefreshTokenAsync(string accessToken, string refreshToken)
    {
      string accountEmail = _tokenService.GetEmailFromAccessToken(accessToken);
      var token = await _unitOfWork.RefreshTokens.GetRefreshTokenAsync(accountEmail, refreshToken);

      if (token is null)
      {
        throw new AppException(Messages.RefreshTokenNotExistsErrorString, HttpStatusCode.BadRequest);
      }

      if (token.ExpirationDate < DateTime.UtcNow)
      {
        throw new AppException(Messages.RefreshTokenExpiredErrorString, HttpStatusCode.BadRequest);
      }

      await _unitOfWork.RefreshTokens.DeleteRefreshTokenAsync(token);
      var user = await _userManager.FindByEmailAsync(accountEmail);
      var newToken = await _tokenService.GenerateTokenAsync(user);
      await _unitOfWork.RefreshTokens.AddRefreshTokenAsync(user, newToken.RefreshToken);

      return newToken;
    }

    public async Task LogOutAsync(string accessToken, string refreshToken)
    {
      string accountEmail = _tokenService.GetEmailFromAccessToken(accessToken);
      var token = await _unitOfWork.RefreshTokens.GetRefreshTokenAsync(accountEmail, refreshToken);

      if (token is not null)
      {
        await _unitOfWork.RefreshTokens.DeleteRefreshTokenAsync(token);
      }
    }

    public async Task RevokeAccessFromAllDevicesAsync(string accessToken)
    {
      string accountEmail = _tokenService.GetEmailFromAccessToken(accessToken);
      var user = await _userManager.FindByEmailAsync(accountEmail);

      if (user is not null && user.RefreshTokens.Count > 0)
      {
        await _unitOfWork.RefreshTokens.DeleteAllUsersRefreshTokens(user);
      }
    }
  }
}
