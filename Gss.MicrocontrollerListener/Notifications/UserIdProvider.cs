using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Gss.MicrocontrollerListener.Notifications;

internal class UserIdProvider: IUserIdProvider
{
  public string GetUserId(HubConnectionContext connection)
  {
    return connection.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
  }
}