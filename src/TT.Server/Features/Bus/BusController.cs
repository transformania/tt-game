using Microsoft.AspNetCore.Mvc;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Queries;
using TT.Domain.Procedures;
using TT.Domain.ViewModels;
using TT.Server.Features.Identity;
using TT.Server.Features.Shared;

namespace TT.Server.Features.Bus;

[Route("/bus")]
public class BusController(ApplicationUserManager userManager) : LoggedInController(userManager)
{
    [HttpGet("")]
    public IActionResult Bus()
    {
        var me = PlayerProcedures.GetPlayerFromMembership(UserId);
        var canTakeBus = DomainRegistry.Repository.FindSingle(new PlayerIsAtBusStop { playerLocation = me.dbLocationName });

        if (!canTakeBus)
        {
            TempData["Error"] = "There is no bus stop here.";
            return RedirectToAction("Play", "Play");
        }

        var output = new BusStopsViewModel
        {
            Stops = DomainRegistry.Repository.Find(new GetBusStops { currentLocation = me.dbLocationName }).Where(b => b.Cost > 0).OrderBy(b => b.Cost),
            Player = DomainRegistry.Repository.FindSingle(new GetPlayerBusDetail { playerId = me.Id })
        };

        return View(output);
    }

    [HttpGet("go")]
    public IActionResult TakeBus(string destination)
    {
        var me = PlayerProcedures.GetPlayerFromMembership(UserId);

        try
        {
            var output = DomainRegistry.Repository.Execute(new TakeBus { playerId = me.Id, destination = destination });
            TempData["Result"] = output;

            return RedirectToAction("Play", "Play");
        }
        catch (DomainException e)
        {
            TempData["Error"] = e.Message;
            return RedirectToAction("Play", "Play");
        }
    }
}