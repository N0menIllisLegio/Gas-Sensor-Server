using System.Threading.Tasks;
using Gss.Core.DTOs;

namespace Gss.Core.Interfaces
{
  public interface IAuthService
  {
    Task<ServiceResultDto<object>> SendResetPasswordConfirmationAsync(string email, string redirectUrl);
    Task<ServiceResultDto<object>> ResetPasswordAsync(string userID, string token, string newPassword);
    Task<ServiceResultDto<object>> SendEmailChangeConfirmationAsync(string email, string newEmail, string confirmationUrl);
    Task<ServiceResultDto<object>> ChangeEmailAsync(string userID, string newEmail, string token);
    Task<ServiceResultDto<object>> SendEmailConfirmationAsync(string email, string confirmationUrl);
    Task<ServiceResultDto<object>> ConfirmEmailAsync(string userID, string token);
    Task<ServiceResultDto<object>> RegisterAsync(CreateUserDto newUserDto);
    Task<ServiceResultDto<TokenDto>> LogInAsync(string login, string password);
    Task<ServiceResultDto<TokenDto>> RefreshTokenAsync(string accessToken, string refreshToken);
    Task LogOutAsync(string accessToken, string refreshToken);
    Task RevokeAccessFromAllDevicesAsync(string accessToken);
  }
}
