using System.Threading.Tasks;
using Gss.Core.DTOs;

namespace Gss.Core.Interfaces
{
  public interface IAuthService
  {
    Task<Response<TokenDto>> LogInAsync(string login, string password);

    Task<Response<TokenDto>> RefreshTokenAsync(string accessToken, string refreshToken);

    Task LogOutAsync(string accessToken, string refreshToken);

    Task RevokeAccessFromAllDevicesAsync(string accessToken);
  }
}
