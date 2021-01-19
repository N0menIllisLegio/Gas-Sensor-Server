using System.Threading.Tasks;
using Gss.Core.Entities;

namespace Gss.Core.Interfaces.Repositories
{
  public interface IRefreshTokensRepository
  {
    Task AddRefreshTokenAsync(User user, string refreshToken);
    Task<RefreshToken> GetRefreshTokenAsync(string email, string seekedRefreshToken);
    Task DeleteRefreshTokenAsync(RefreshToken refreshToken);
    Task DeleteAllUsersRefreshTokens(User user);
  }
}
