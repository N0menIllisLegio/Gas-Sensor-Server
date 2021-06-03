using System.Threading.Tasks;
using Gss.Core.DTOs.Authentication;
using Gss.Core.Entities;

namespace Gss.Core.Interfaces.Services
{
  public interface ITokensService
  {
    Task<TokenDto> GenerateTokenAsync(User user);
    string GetEmailFromAccessToken(string accessToken);
  }
}
