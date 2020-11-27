using System.Threading.Tasks;
using Gss.Core.Entities;

namespace Gss.Core.Interfaces
{
  public interface IRefreshTokenRepository
  {
    public Task AddRefreshTokenAsync(User user, string refreshToken);

    public Task<RefreshToken> GetRefreshTokenAsync(string email, string seekedRefreshToken);

    public Task DeleteRefreshTokenAsync(RefreshToken refreshToken);

    public Task DeleteAllUsersRefreshTokens(User user);
  }
}
