using System.Threading.Tasks;
using Gss.Core.DTOs;

namespace Gss.Core.Interfaces
{
  public interface IAuthService
  {
    public Task<TokenDto> LogInAsync(string login, string password);

    public Task<TokenDto> RefreshTokenAsync(string accessToken, string refreshToken);

    public Task LogOutAsync(string accessToken, string refreshToken);
  }
}
