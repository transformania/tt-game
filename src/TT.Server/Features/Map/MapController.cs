using Microsoft.AspNetCore.Mvc;
using TT.Domain.Legacy.Procedures.JokeShop;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;
using TT.Server.Features.Identity;
using TT.Server.Features.Shared;

namespace TT.Server.Features.Map;

[Route("/map")]
public class MapController(ApplicationUserManager userManager) : LoggedInController(userManager)
{
    [HttpGet("")]
    public IActionResult WorldMap(bool showEnchant = false)
    {
        var me = PlayerProcedures.GetPlayerFromMembership(UserId);
        var locations = LocationsStatics.LocationList.GetLocation;
        var here = locations.FirstOrDefault(l => l.dbName == me.dbLocationName);
        var isBlinded = EffectProcedures.PlayerHasActiveEffect(me, CharacterPrankProcedures.BLINDED_EFFECT);
        var isInDungeon = me.IsInDungeon();
        
        var output = new MapViewModel
        {
            LocationInfo = showEnchant ? CovenantProcedures.GetLocationInfos() : null,
            MinX = locations.Select(l => l.X).Min(),
            MaxY = locations.Select(l => l.Y).Max(),
            PlayerX = isBlinded ? 999 : here.X,
            PlayerY = isBlinded ? 999 : here.Y,
            IsInDungeon = isInDungeon,
            Locations = (isInDungeon && !showEnchant)
                ? locations.Where(l => l.Region == "")
                : locations.Where(l => l.Region != "dungeon"),
        };

        return View(output);
    }
}