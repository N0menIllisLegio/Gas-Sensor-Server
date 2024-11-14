using Microsoft.AspNetCore.SignalR;

namespace Gss.Core.Helpers
{
  public class UserEmailProvider: IUserIdProvider
  {
    public string GetUserId(HubConnectionContext connection)
    {
      return connection.User?.Identity.Name;
    }
  }
}
