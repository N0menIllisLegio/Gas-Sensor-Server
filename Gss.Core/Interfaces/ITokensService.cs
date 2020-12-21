using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.Entities;

namespace Gss.Core.Interfaces
{
  public interface ITokensService
  {
    Task<TokenDto> GenerateTokenAsync(User user);
    string GetEmailFromAccessToken(string accessToken);
  }
}
