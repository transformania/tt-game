using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TT.Server.Features.Identity;

namespace TT.Server.Features.Shared;

[Authorize]
public abstract class LoggedInController : Controller
{
    public ApplicationUserManager UserManager { get; private set; }
    public string UserId => UserManager.GetUserId(User);
    public string UserIp => HttpContext.Connection.RemoteIpAddress?.ToString();

    protected LoggedInController(ApplicationUserManager userManager)
    {
        UserManager = userManager;
    }
}