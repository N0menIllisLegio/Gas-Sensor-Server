using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Gss.Web
{
  [Authorize]
  public class NotificationsHub: Hub
  { }
}
