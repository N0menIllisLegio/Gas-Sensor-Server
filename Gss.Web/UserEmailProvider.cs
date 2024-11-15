using Microsoft.AspNetCore.SignalR;

namespace Gss.Web
{
  public class UserEmailProvider: IUserIdProvider
  {
    public string GetUserId(HubConnectionContext connection)
    {
      return connection.User?.Identity.Name;
    }
  }
}
