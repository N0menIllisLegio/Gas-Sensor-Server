using System.Threading.Tasks;
using Gss.Core.DTOs.Authentication;
using Gss.Core.DTOs.User;

namespace Gss.Core.Interfaces
{
  public interface IAuthenticationService
  {
    Task SendResetPasswordConfirmationAsync(string email, string redirectUrl);
    Task ResetPasswordAsync(ChangePasswordDto changePasswordDto);
    Task SendEmailChangeConfirmationAsync(string email, string newEmail, string confirmationUrl);
    Task ChangeEmailAsync(ChangeEmailDto changeEmailDto);
    Task SendEmailConfirmationAsync(string email, string confirmationUrl);
    Task ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto);
    Task RegisterAsync(CreateUserDto newUserDto);
    Task<TokenDto> LogInAsync(string login, string password);
    Task<TokenDto> RefreshTokenAsync(string accessToken, string refreshToken);
    Task LogOutAsync(string accessToken, string refreshToken);
    Task RevokeAccessFromAllDevicesAsync(string accessToken);
  }
}
