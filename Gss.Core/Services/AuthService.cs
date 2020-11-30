using System;
using System.Threading.Tasks;
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
    private readonly ITokenService _tokenService;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager _userManager;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public AuthService(ITokenService tokenService,
      SignInManager<User> signInManager, UserManager userManager,
      IRefreshTokenRepository refreshTokenRepository)
    {
      _tokenService = tokenService;
      _signInManager = signInManager;
      _userManager = userManager;
      _refreshTokenRepository = refreshTokenRepository;
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

        return new Response<TokenDto>(errorMessage);
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
        return new Response<TokenDto>(Messages.RefreshTokenNotExistsErrorString);
      }

      if (token.ExpirationDate < DateTime.UtcNow)
      {
        return new Response<TokenDto>(Messages.RefreshTokenExpiredErrorString);
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
