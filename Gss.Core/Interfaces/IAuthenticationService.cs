using System.Threading.Tasks;
using Gss.Core.DTOs.Authentication;
using Gss.Core.DTOs.User;

namespace Gss.Core.Interfaces
{
  public interface IAuthenticationService
  {
    Task SendResetPasswordConfirmationAsync(string email, string redirectUrl);
    Task<bool> ResetPasswordAsync(string userID, string token, string newPassword);
    Task SendEmailChangeConfirmationAsync(string email, string newEmail, string confirmationUrl);
    Task<bool> ChangeEmailAsync(string userID, string newEmail, string token);
    Task SendEmailConfirmationAsync(string email, string confirmationUrl);
    Task<bool> ConfirmEmailAsync(string userID, string token);
    Task RegisterAsync(CreateUserDto newUserDto);
    Task<TokenDto> LogInAsync(string login, string password);
    Task<TokenDto> RefreshTokenAsync(string accessToken, string refreshToken);
    Task LogOutAsync(string accessToken, string refreshToken);
    Task RevokeAccessFromAllDevicesAsync(string accessToken);
  }
}
