using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Gss.MicrocontrollerDataReceiver;

[Authorize]
public class NotificationsHub: Hub
{ }