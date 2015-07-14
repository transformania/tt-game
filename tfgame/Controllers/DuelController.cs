using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tfgame.dbModels.Models;
using tfgame.Filters;
using tfgame.Procedures;
using tfgame.Statics;

namespace tfgame.Controllers
{
     [InitializeSimpleMembership]
    public class DuelController : Controller
    {
        
        [Authorize]
        public ActionResult Duel()
        {
            return View();
        }

        [Authorize]
        public ActionResult IssueChallenge(int id)
        {
            Player me = PlayerProcedures.GetPlayerFromMembership();

            // assert player is animate
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to challenge someone to a duel.";
                return RedirectToAction("Play", "PvP");
            }

            // assert player is not already in a duel
            if (me.InDuel > 0)
            {
                TempData["Error"] = "You are already actively participating in a duel.";
                TempData["SubError"] = "You must finish your currently active duel before you can start a new one.";
                return RedirectToAction("Play", "PvP");
            }

            // assert that the player has not been in recent combat
            double minutesAgo = Math.Abs(Math.Floor(me.GetLastCombatTimestamp().Subtract(DateTime.UtcNow).TotalMinutes));
            if (minutesAgo < PvPStatics.DuelNoCombatMinutes)
            {
                TempData["Error"] = "You must wait another " + (PvPStatics.DuelNoCombatMinutes - minutesAgo) + " minutes without being in combat in order to challenge this opponent to a duel.";
                return RedirectToAction("Play", "PvP");
            }

            Player duelTarget = PlayerProcedures.GetPlayerFromMembership(id);

            // assert target is animate
            if (duelTarget.Mobility != "full")
            {
                TempData["Error"] = "Your target must be animate in order to challenge someone to a duel.";
                return RedirectToAction("Play", "PvP");
            }

            // assert target is not already in a duel
            if (duelTarget.InDuel > 0)
            {
                TempData["Error"] = "Your target is already actively participating in a duel.";
                TempData["SubError"] = "Your target must finish their currently active duel before they can start a new one.";
                return RedirectToAction("Play", "PvP");
            }

            // assert that the target has not been in recent combat
            minutesAgo = Math.Abs(Math.Floor(me.GetLastCombatTimestamp().Subtract(DateTime.UtcNow).TotalMinutes));
            if (minutesAgo < PvPStatics.DuelNoCombatMinutes)
            {
                TempData["Error"] = "Your target must wait longer without being in combat in order to duel you.";
                return RedirectToAction("Play", "PvP");
            }

            // assert both players are in the same location
            if (me.dbLocationName != duelTarget.dbLocationName)
            {
                TempData["Error"] = "You must be in the same location as your target in order to challenge them to a duel.";
                return RedirectToAction("Play", "PvP");
            }

            // assert both players are in an okay game mode
            bool weAreFriends = FriendProcedures.PlayerIsMyFriend(me, duelTarget);
            if (weAreFriends == false)
            {
                // player is in PvP; target is not
                if (me.GameMode == 2 && duelTarget.GameMode < 2)
                {
                    TempData["Error"] = "You must either be friends with your target or in the same game mode to challenge them to a duel.";
                    return RedirectToAction("Play", "PvP");
                }

                // player is not in PvP; target is
                else if (me.GameMode < 2 && duelTarget.GameMode == 2)
                {
                    TempData["Error"] = "You must either be friends with your target or in the same game mode to challenge them to a duel.";
                    return RedirectToAction("Play", "PvP");
                }
            }

            DuelProcedures.SendDuelChallenge(me, duelTarget);

            return View();
        }
	}
}