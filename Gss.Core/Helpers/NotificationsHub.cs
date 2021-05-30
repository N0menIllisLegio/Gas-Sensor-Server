using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Gss.Core.Helpers
{
  [Authorize]
  public class NotificationsHub: Hub
  { }
}
