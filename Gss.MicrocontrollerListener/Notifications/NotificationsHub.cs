using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Gss.MicrocontrollerListener.Notifications;

[Authorize]
internal class NotificationsHub : Hub
{
    public const string Url = "/api/notifications";
}