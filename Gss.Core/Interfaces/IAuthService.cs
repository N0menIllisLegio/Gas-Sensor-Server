using System.Threading.Tasks;
using Gss.Core.DTOs;

namespace Gss.Core.Interfaces
{
  public interface IAuthService
  {
    Task<Response<object>> SendEmailConfirmationAsync(string userID, string confirmationUrl);

    Task<Response<object>> ConfirmEmailAsync(string userID, string token);

    Task<Response<object>> RegisterAsync(CreateUserDto newUserDto);

    Task<Response<TokenDto>> LogInAsync(string login, string password);

    Task<Response<TokenDto>> RefreshTokenAsync(string accessToken, string refreshToken);

    Task LogOutAsync(string accessToken, string refreshToken);

    Task RevokeAccessFromAllDevicesAsync(string accessToken);
  }
}
