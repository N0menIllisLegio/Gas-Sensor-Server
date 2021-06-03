using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Gss.Core.DTOs.Authentication;
using Gss.Core.Entities;
using Gss.Core.Helpers;
using Gss.Core.Interfaces.Services;
using Microsoft.IdentityModel.Tokens;

namespace Gss.Core.Services
{
  public class TokensService : ITokensService
  {
    private readonly UserManager _userManager;

    public TokensService(UserManager userManager)
    {
      _userManager = userManager;
    }

    public string GetEmailFromAccessToken(string accessToken)
    {
      var tokenHandler = new JwtSecurityTokenHandler();

      if (!tokenHandler.CanReadToken(accessToken))
      {
        return null;
      }

      var jwtSecurityToken = tokenHandler.ReadJwtToken(accessToken);

      if (jwtSecurityToken is null
        || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
          StringComparison.InvariantCultureIgnoreCase))
      {
        return null;
      }

      return jwtSecurityToken.Claims.FirstOrDefault(claim =>
        claim.Type == ClaimsIdentity.DefaultNameClaimType)?.Value;
    }

    public async Task<TokenDto> GenerateTokenAsync(User user)
    {
      var userClaims = await GetUserClaimsAsync(user);

      var issueTime = DateTime.UtcNow;
      var expirationTime = issueTime.Add(
        TimeSpan.FromMinutes(Settings.JWT.AccessTokenLifetimeMinutes));

      var jwt = new JwtSecurityToken(
        issuer: Settings.JWT.Issuer,
        audience: Settings.JWT.Audience,
        notBefore: issueTime,
        claims: userClaims.Claims,
        expires: expirationTime,
        signingCredentials: new SigningCredentials(
          Settings.JWT.Key,
          SecurityAlgorithms.HmacSha256));

      return new TokenDto
      {
        AccessToken = new JwtSecurityTokenHandler().WriteToken(jwt),
        AccessTokenExpiration = expirationTime,
        RefreshToken = GenerateRefreshToken(),
        UserID = user.Id,
        Administrator = await _userManager.IsAdministrator(user.Email)
      };
    }

    private async Task<ClaimsIdentity> GetUserClaimsAsync(User user)
    {
      var roles = await _userManager.GetRolesAsync(user);

      var claims = new List<Claim>
      {
        new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email)
      };

      foreach (string role in roles)
      {
        claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role));
      }

      var claimsIdentity =
        new ClaimsIdentity(claims, "Token",
          ClaimsIdentity.DefaultNameClaimType,
          ClaimsIdentity.DefaultRoleClaimType);

      return claimsIdentity;
    }

    private string GenerateRefreshToken()
    {
      byte[] randomNumber = new byte[32];

      using var rng = RandomNumberGenerator.Create();
      rng.GetBytes(randomNumber);

      return Convert.ToBase64String(randomNumber);
    }
  }
}
