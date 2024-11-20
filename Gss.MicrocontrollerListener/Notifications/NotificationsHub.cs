using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Gss.MicrocontrollerListener.Notifications;

[Authorize]
public class NotificationsHub : Hub
{
    public const string Url = "/api/notifications";
}