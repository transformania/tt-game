using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tfgame.dbModels.Models;
using tfgame.Extensions;
using tfgame.Procedures;
using tfgame.Statics;
using tfgame.ViewModels;
using Microsoft.AspNet.Identity;

namespace tfgame.Controllers
{
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
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

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

            Player duelTarget = PlayerProcedures.GetPlayer(id);

            // assert target is not a bot
            if (duelTarget.BotId < 0)
            {
                TempData["Error"] = "You cannot challenge an NPC to a bot.";
            }

            // assert target is animate
            if (duelTarget.Mobility != "full")
            {
                TempData["Error"] = "Your target must be animate in order to challenge someone to a duel.";
                return RedirectToAction("Play", "PvP");
            }

            // assert no blacklist exists
            if (BlacklistProcedures.PlayersHaveBlacklistedEachOther(me, duelTarget, "attack") == true)
            {
                TempData["Error"] = "This player has blacklisted you or is on your own blacklist.";
                TempData["SubError"] = "You cannot duel players who are on your blacklist.  Remove them from your blacklist first or ask them to remove you from theirs.";
                return RedirectToAction("Play");
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

            // TODO:  assert player does not already have a pending duel request

            DuelProcedures.SendDuelChallenge(me, duelTarget);


            TempData["Result"] = "You have sent out a challenge to a duel to " + duelTarget.GetFullName() + ".";

            return RedirectToAction("Play", "PvP");
        }

        [Authorize]
        public ActionResult AcceptChallenge(int id)
        {
            string myMembershipId = User.Identity.GetUserId();
            Duel duel = DuelProcedures.GetDuel(id);
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert duel challenge is not too old
            if (duel.ProposalTurn > PvPWorldStatProcedures.GetWorldTurnNumber() + 1)
            {
                TempData["Error"] = "This challenge to a duel has expired.";
                TempData["SubError"] = "Offers for a duel must be accpted within the same turn or in the next.";
                return RedirectToAction("Play", "PvP");
            }

            // assert duel is still active
            if (duel.Status != DuelProcedures.PENDING)
            {
                TempData["Error"] = "This duel has already started, has been completed, or was rejected.";
                return RedirectToAction("Play", "PvP");
            }

            List<PlayerFormViewModel> participants = DuelProcedures.GetPlayerViewModelsInDuel(duel.Id);
            string duelLocation = participants.First().Player.dbLocationName;
            int challengerGameMode = participants.First().Player.GameMode;

            foreach (PlayerFormViewModel p in participants)
            {

                // assert player is not a bot... somehow
                if (p.Player.BotId < 0)
                {
                    TempData["Error"] =  p.Player.GetFullName() + " is an NPC and thus cannot engage in a duel.";
                    return RedirectToAction("Play", "PvP");
                }

                // assert player is animate
                if (p.Player.Mobility != "full")
                {
                    TempData["Error"] = "Duel cannot start yet.  " + p.Player.GetFullName() + " must be animate in order to challenge someone to a duel.";
                    return RedirectToAction("Play", "PvP");
                }

                // assert player is not already in a duel
                if (p.Player.InDuel > 0)
                {
                    TempData["Error"] = "Duel cannot start yet.  " + p.Player.GetFullName() + " is already participating in a duel.";
                    TempData["SubError"] = "Each player must not be in an active duel in order to start a new one.";
                    return RedirectToAction("Play", "PvP");
                }

                // assert player is at the duel location
                if (p.Player.dbLocationName != duelLocation)
                {
                    TempData["Error"] = "Duel cannot start yet.  All players must be in the same location to begin a duel.";
                    return RedirectToAction("Play", "PvP");
                }

                // assert player has sufficient WP  to start
                if (p.Player.Health < p.Player.MaxHealth * .8M)
                {
                    TempData["Error"] = "Duel cannot start yet.  " + p.Player.GetFullName() + " has too low willpower.";
                    TempData["SubError"] = "Each player must be at least 80% willpower in order to begin dueling.";
                    return RedirectToAction("Play", "PvP");
                }

                // assert player has sufficient Mana to start
                if (p.Player.Mana < p.Player.MaxMana * .8M)
                {
                    TempData["Error"] = "Duel cannot start yet.  " + p.Player.GetFullName() + " has too low mana.";
                    TempData["SubError"] = "Each player must be at least 80% mana in order to begin dueling.";
                    return RedirectToAction("Play", "PvP");
                }

                // assert all players are in an okay game mode
                bool weAreFriends = FriendProcedures.PlayerIsMyFriend(me, p.Player.ToDbPlayer());
                if (weAreFriends == false)
                {
                    // player is in PvP; target is not
                    if (me.GameMode == 2 && p.Player.GameMode < 2)
                    {
                        TempData["Error"] = "You must either be friends with " + p.Player.GetFullName() + " or in the same game mode to challenge them to a duel.";
                        return RedirectToAction("Play", "PvP");
                    }

                    // player is not in PvP; target is
                    else if (me.GameMode < 2 && p.Player.GameMode == 2)
                    {
                        TempData["Error"] = "You must either be friends with " + p.Player.GetFullName() + " or in the same game mode to challenge them to a duel.";
                        return RedirectToAction("Play", "PvP");
                    }
                }

                // assert that the player has not been in recent combat
                double minutesAgo = Math.Abs(Math.Floor(p.Player.GetLastCombatTimestamp().Subtract(DateTime.UtcNow).TotalMinutes));
                if (minutesAgo < PvPStatics.DuelNoCombatMinutes)
                {
                    TempData["Error"] =  "Duel cannot start yet.  " + p.Player.GetFullName() + " must wait another " + (PvPStatics.DuelNoCombatMinutes - minutesAgo) + " minutes without being in combat in order accept this challenge to a duel.";
                    return RedirectToAction("Play", "PvP");
                }

            }

            TempData["Result"] = "Your duel has started!";
            DuelProcedures.BeginDuel(duel.Id);

            return RedirectToAction("Play","PvP");
        }

         [Authorize]
        public ActionResult DuelDetail(int id)
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            Duel duel = DuelProcedures.GetDuel(id);
            if (me.InDuel != duel.Id)
            {
                TempData["Error"] = "You are not in this duel.";
                return RedirectToAction("Play", "PvP");
            }

            DuelPlayersViewModel output = new DuelPlayersViewModel
            {
                Duel = duel,
                Combatants = DuelProcedures.GetPlayerViewModelsInDuel(duel.Id)
            };

            ViewBag.CurrentTurn = PvPWorldStatProcedures.GetWorldTurnNumber();
            ViewBag.TurnsRemaining = PvPStatics.MaximumDuelTurnLength - (ViewBag.CurrentTurn - duel.StartTurn);

            return View(output);
        }

         [Authorize]
         public ActionResult AdvanceTurn()
         {
             string myMembershipId = User.Identity.GetUserId();
             Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

             if (me.InDuel <= 0)
             {
                 TempData["Error"] = "You are not in a duel.";
                 return RedirectToAction("Play", "PvP");
             }

             Duel duel = DuelProcedures.GetDuel(me.InDuel);

             List<PlayerFormViewModel> combatants = DuelProcedures.GetPlayerViewModelsInDuel(duel.Id);

             if (PvPStatics.ChaosMode == false)
             {
                 foreach (PlayerFormViewModel p in combatants)
                 {
                     if (p.Player.TimesAttackingThisUpdate < PvPStatics.MaxAttacksPerUpdate)
                     {
                         TempData["Error"] = "Cannot advance this turn." + p.Player.GetFullName() + " has not used up all of their attacks.";
                         return RedirectToAction("Play", "PvP");
                     }
                 }
             }

             foreach (PlayerFormViewModel p in combatants)
             {
                 PlayerProcedures.SetAttackCount(p.Player.ToDbPlayer(), 0);
                 PlayerProcedures.SetCleanseMeditateCount(p.Player.ToDbPlayer(), 0);
                 string message = "<b>" + me.GetFullName() + " has advanced the duel turn.  Attacks and cleanse/meditate limits have been reset.  Attacks may resume in 20 seconds.</b>";
                 PlayerLogProcedures.AddPlayerLog(p.Player.Id, message, true);
                 NoticeProcedures.PushNotice(p.Player.Id, message, NoticeProcedures.PushType__PlayerLog);
             }

             DuelProcedures.SetLastDuelAttackTimestamp(duel.Id);

             TempData["Result"] = "Duel turn advanced.  All combatants have had their attack and cleanse/meditate limits reset.  Attacks may resume in 20 seconds.";
             return RedirectToAction("Play", "PvP");
         }

         [Authorize]
         public ActionResult DuelTimeout()
         {

             string myMembershipId = User.Identity.GetUserId();
             Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

             if (me.InDuel <= 0)
             {
                 TempData["Error"] = "You are not in a duel.";
                 return RedirectToAction("Play", "PvP");
             }

             Duel duel = DuelProcedures.GetDuel(me.InDuel);

             int turnsLeft = PvPStatics.MaximumDuelTurnLength - (PvPWorldStatProcedures.GetWorldTurnNumber() - duel.StartTurn);

             if (turnsLeft > 0)
             {
                 TempData["Error"] = "You cannot end this duel as there are still turns remaining.";
                 return RedirectToAction("Play", "PvP");
             }


             DuelProcedures.EndDuel(duel.Id, DuelProcedures.TIMEOUT);

             TempData["Result"] = "This duel has timed out in a no-winner result.";
             return RedirectToAction("Play", "PvP");


         }


	}
}