using System.Threading.Tasks;
using Gss.Core.DTOs;
using Gss.Core.Entities;

namespace Gss.Core.Interfaces
{
  public interface ITokenService
  {
    public Task<TokenDto> GenerateAccessTokenAsync(User user);
    public string GetEmailFromAccessToken(string accessToken);
  }
}
