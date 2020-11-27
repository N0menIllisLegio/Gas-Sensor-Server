using System;
using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.Entities;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;
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

    public async Task<TokenDto> LogInAsync(string login, string password)
    {
      var user = await _userManager.FindByEmailAsync(login)
        ?? await _userManager.FindByNameAsync(login);

      var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

      if (!result.Succeeded)
      {
        return null;
      }

      var token = await _tokenService.GenerateTokenAsync(user);
      await _refreshTokenRepository.AddRefreshTokenAsync(user, token.RefreshToken);

      return token;
    }

    public async Task<TokenDto> RefreshTokenAsync(string accessToken, string refreshToken)
    {
      string accountEmail = _tokenService.GetEmailFromAccessToken(accessToken);
      var token = await _refreshTokenRepository.GetRefreshTokenAsync(accountEmail, refreshToken);

      if (token is null || token.ExpirationDate < DateTime.UtcNow)
      {
        return null;
      }

      await _refreshTokenRepository.DeleteRefreshTokenAsync(token);
      var user = await _userManager.FindByEmailAsync(accountEmail);
      var newToken = await _tokenService.GenerateTokenAsync(user);
      await _refreshTokenRepository.AddRefreshTokenAsync(user, newToken.RefreshToken);

      return newToken;
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

      if (user is null || user.RefreshTokens.Count == 0)
      {
        return;
      }

      await _refreshTokenRepository.DeleteAllUsersRefreshTokens(user);
    }
  }
}
