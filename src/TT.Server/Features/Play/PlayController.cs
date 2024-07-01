using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TT.Domain.Procedures;
using TT.Server.Features.Identity;
using TT.Server.Features.Shared;

namespace TT.Server.Features.Play;

[Route("/play")]
[Authorize]
public class PlayController(ApplicationUserManager userManager) : LoggedInController(userManager)
{
    [HttpGet("")]
    public async Task<IActionResult> Play()
    {
        var me = PlayerProcedures.GetPlayerFromMembership(UserId);
        if (me == null)
        {
            return RedirectToAction("Restart", "Character");
        }

        ViewBag.ErrorMessage = TempData["Error"];
        ViewBag.SubErrorMessage = TempData["SubError"];
        ViewBag.Result = TempData["Result"];
        ViewBag.BodyClasses = $"location-{me.dbLocationName}"; // NB. This could leak location to owned items

        return View();
    }
}