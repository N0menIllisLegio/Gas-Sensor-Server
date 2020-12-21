using System;
using System.Threading.Tasks;
using Gss.Core.Entities;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gss.Infrastructure.Repositories
{
  public class RefreshTokensRepository: IRefreshTokensRepository
  {
    private readonly AppDbContext _appDbContext;
    public RefreshTokensRepository(AppDbContext appDbContext)
    {
      _appDbContext = appDbContext;
    }

    public async Task AddRefreshTokenAsync(User user, string refreshToken)
    {
      _appDbContext.RefreshTokens.Add(new RefreshToken
      {
        Token = refreshToken,
        ExpirationDate = DateTime.UtcNow.AddDays(Settings.JWT.RefreshTokenLifetimeDays),
        User = user
      });

      await _appDbContext.SaveChangesAsync();
    }

    public async Task<RefreshToken> GetRefreshTokenAsync(string email, string seekedRefreshToken)
    {
      return await _appDbContext.RefreshTokens.FirstOrDefaultAsync(refreshToken =>
        refreshToken.User.Email == email && refreshToken.Token == seekedRefreshToken);
    }

    public async Task DeleteRefreshTokenAsync(RefreshToken refreshToken)
    {
      _appDbContext.RefreshTokens.Remove(refreshToken);

      await _appDbContext.SaveChangesAsync();
    }

    public async Task DeleteAllUsersRefreshTokens(User user)
    {
      _appDbContext.RefreshTokens.RemoveRange(user.RefreshTokens);

      await _appDbContext.SaveChangesAsync();
    }
  }
}
