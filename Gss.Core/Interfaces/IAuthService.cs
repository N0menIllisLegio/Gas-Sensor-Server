using System.Threading.Tasks;
using Gss.Core.DTOs;

namespace Gss.Core.Interfaces
{
  public interface IAuthService
  {
    Task<Response<object>> SendResetPasswordConfirmationAsync(string email, string redirectUrl);
    Task<Response<object>> ResetPasswordAsync(string userID, string token, string newPassword);
    Task<Response<object>> SendEmailChangeConfirmationAsync(string email, string newEmail, string confirmationUrl);
    Task<Response<object>> ChangeEmailAsync(string userID, string newEmail, string token);
    Task<Response<object>> SendEmailConfirmationAsync(string email, string confirmationUrl);
    Task<Response<object>> ConfirmEmailAsync(string userID, string token);
    Task<Response<object>> RegisterAsync(CreateUserDto newUserDto);
    Task<Response<TokenDto>> LogInAsync(string login, string password);
    Task<Response<TokenDto>> RefreshTokenAsync(string accessToken, string refreshToken);
    Task LogOutAsync(string accessToken, string refreshToken);
    Task RevokeAccessFromAllDevicesAsync(string accessToken);
  }
}
