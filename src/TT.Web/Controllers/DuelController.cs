using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;
using TT.Web.Services;

namespace TT.Web.Controllers
{
    [Authorize]
    public partial class DuelController : Controller
    {

        public virtual ActionResult Duel()
        {
            return View(MVC.Duel.Views.Duel);
        }

        public virtual ActionResult IssueChallenge(int id)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert player is animate
            if (me.Mobility != PvPStatics.MobilityFull)
            {
                TempData["Error"] = "You must be animate in order to challenge someone to a duel.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player is not already in a duel
            if (me.InDuel > 0)
            {
                TempData["Error"] = "You are already actively participating in a duel.";
                TempData["SubError"] = "You must finish your currently active duel before you can start a new one.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that this player is not in a quest
            if (me.InQuest > 0)
            {
                TempData["Error"] = "You must finish your quest before you can participate in a duel.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that the player has not been in recent combat
            var minutesAgo = Math.Abs(Math.Floor(me.GetLastCombatTimestamp().Subtract(DateTime.UtcNow).TotalMinutes));
            if (minutesAgo < TurnTimesStatics.GetMinutesSinceLastCombatBeforeQuestingOrDuelling())
            {
                TempData["Error"] = "You must wait another " + (TurnTimesStatics.GetMinutesSinceLastCombatBeforeQuestingOrDuelling() - minutesAgo) + " minutes without being in combat in order to challenge this opponent to a duel.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var duelTarget = PlayerProcedures.GetPlayer(id);

            // assert target is not a bot
            if (duelTarget.BotId != AIStatics.ActivePlayerBotId)
            {
                TempData["Error"] = "You cannot challenge an NPC to a duel.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert target is animate
            if (duelTarget.Mobility != PvPStatics.MobilityFull)
            {
                TempData["Error"] = "Your target must be animate in order to challenge someone to a duel.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert no blacklist exists
            if (BlacklistProcedures.PlayersHaveBlacklistedEachOther(me, duelTarget, "attack"))
            {
                TempData["Error"] = "This player has blacklisted you or is on your own blacklist.";
                TempData["SubError"] = "You cannot duel players who are on your blacklist.  Remove them from your blacklist first or ask them to remove you from theirs.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert target is not already in a duel
            if (duelTarget.InDuel > 0)
            {
                TempData["Error"] = "Your target is already actively participating in a duel.";
                TempData["SubError"] = "Your target must finish their currently active duel before they can start a new one.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that this player is not in a quest
            if (duelTarget.InQuest > 0)
            {
                TempData["Error"] = "Your target must finish their quest before you can duel them.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that the target has not been in recent combat
            minutesAgo = Math.Abs(Math.Floor(me.GetLastCombatTimestamp().Subtract(DateTime.UtcNow).TotalMinutes));
            if (minutesAgo < TurnTimesStatics.GetMinutesSinceLastCombatBeforeQuestingOrDuelling())
            {
                TempData["Error"] = "Your target must wait longer without being in combat in order to duel you.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert both players are in the same location
            if (me.dbLocationName != duelTarget.dbLocationName)
            {
                TempData["Error"] = "You must be in the same location as your target in order to challenge them to a duel.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert both players are in an okay game mode
            var weAreFriends = FriendProcedures.PlayerIsMyFriend(me, duelTarget);
            if (!weAreFriends)
            {
                // player is in PvP; target is not
                if (me.GameMode == (int)GameModeStatics.GameModes.PvP && duelTarget.GameMode < (int)GameModeStatics.GameModes.PvP)
                {
                    TempData["Error"] = "You must either be friends with your target or in the same game mode to challenge them to a duel.";
                    return RedirectToAction(MVC.PvP.Play());
                }

                // player is not in PvP; target is
                else if (me.GameMode < (int)GameModeStatics.GameModes.PvP && duelTarget.GameMode == (int)GameModeStatics.GameModes.PvP)
                {
                    TempData["Error"] = "You must either be friends with your target or in the same game mode to challenge them to a duel.";
                    return RedirectToAction(MVC.PvP.Play());
                }
            }

            // TODO:  assert player does not already have a pending duel request

            DuelProcedures.SendDuelChallenge(me, duelTarget);


            TempData["Result"] = "You have sent out a challenge to a duel to " + duelTarget.GetFullName() + ".";

            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult AcceptChallenge(int id)
        {
            var myMembershipId = User.Identity.GetUserId();

            if (PvPStatics.AnimateUpdateInProgress)
            {
                TempData["Error"] = "Player update portion of the world update is still in progress.";
                TempData["SubError"] = "Try again a bit later when the update has progressed farther along.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var duel = DuelProcedures.GetDuel(id);
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert duel challenge is not too old
            if (duel.ProposalTurn > PvPWorldStatProcedures.GetWorldTurnNumber() + 1)
            {
                TempData["Error"] = "This challenge to a duel has expired.";
                TempData["SubError"] = "Offers for a duel must be accpted within the same turn or in the next.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert duel is still active
            if (duel.Status != DuelProcedures.PENDING)
            {
                TempData["Error"] = "This duel has already started, has been completed, or was rejected.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var participants = DuelProcedures.GetPlayerViewModelsInDuel(duel.Id);
            var duelLocation = participants.First().Player.dbLocationName;

            var errorMessages = new List<string>();

            foreach (var p in participants)
            {

                // assert player is not a bot... somehow
                if (p.Player.BotId < AIStatics.ActivePlayerBotId)
                {
                    errorMessages.Add(p.Player.GetFullName() + " is an NPC and thus cannot engage in a duel.");
                }

                // assert player is animate
                if (p.Player.Mobility != PvPStatics.MobilityFull)
                {
                    errorMessages.Add("Duel cannot start yet.  " + p.Player.GetFullName() + " must be animate in order to challenge someone to a duel.");
                }

                // assert player is not already in a duel
                if (p.Player.InDuel > 0)
                {
                    errorMessages.Add("Duel cannot start yet.  " + p.Player.GetFullName() + " is already participating in a duel.  Each player must not be in an active duel in order to start a new one.");
                }

                // assert player is at the duel location
                if (p.Player.dbLocationName != duelLocation)
                {
                    errorMessages.Add("Duel cannot start yet.  All players must be in the same location to begin a duel.");
                }

                // assert player has sufficient WP to start
                if (p.Player.Health < p.Player.MaxHealth * .8M)
                {
                    errorMessages.Add("Duel cannot start yet.  " + p.Player.GetFullName() + " has too low willpower.  Each player must be at least 80% willpower in order to begin dueling.");
                }

                // assert player has sufficient Mana to start
                if (p.Player.Mana < p.Player.MaxMana * .8M)
                {
                    errorMessages.Add("Duel cannot start yet.  " + p.Player.GetFullName() + " has too low mana.  Each player must be at least 80% mana in order to begin dueling.");
                }

                // assert all players are in an okay game mode
                var weAreFriends = FriendProcedures.PlayerIsMyFriend(me, p.Player.ToDbPlayer());
                if (!weAreFriends)
                {
                    // player is in PvP; target is not
                    if (me.GameMode == (int)GameModeStatics.GameModes.PvP && p.Player.GameMode < (int)GameModeStatics.GameModes.PvP)
                    {
                        errorMessages.Add("You must either be friends with " + p.Player.GetFullName() + " or in the same game mode to challenge them to a duel.");
                    }

                    // player is not in PvP; target is
                    else if (me.GameMode < (int)GameModeStatics.GameModes.PvP && p.Player.GameMode == (int)GameModeStatics.GameModes.PvP)
                    {
                        errorMessages.Add("You must either be friends with " + p.Player.GetFullName() + " or in the same game mode to challenge them to a duel.");
                    }
                }

                // assert that the player has not been in recent combat
                var minutesAgo = Math.Abs(Math.Floor(p.Player.GetLastCombatTimestamp().Subtract(DateTime.UtcNow).TotalMinutes));
                if (minutesAgo < TurnTimesStatics.GetMinutesSinceLastCombatBeforeQuestingOrDuelling())
                {
                    errorMessages.Add("Duel cannot start yet.  " + p.Player.GetFullName() + " must wait another " + (TurnTimesStatics.GetMinutesSinceLastCombatBeforeQuestingOrDuelling() - minutesAgo) + " minutes without being in combat in order accept this challenge to a duel.");
                }

                // assert that both players are still online 
                minutesAgo = Math.Abs(Math.Floor(p.Player.LastActionTimestamp.Subtract(DateTime.UtcNow).TotalMinutes));
                if (minutesAgo > TurnTimesStatics.GetMinutesSinceLastCombatBeforeQuestingOrDuelling())
                {
                    errorMessages.Add("Duel cannot start yet.  " + p.Player.GetFullName() + " has been inactive for " + (TurnTimesStatics.GetMinutesSinceLastCombatBeforeQuestingOrDuelling() - minutesAgo) + " minutes.  " + p.Player.GetFullName() + " can become active again and eligible for the duel performing any action which takes AP.  Do not attack as this will reset their no combat timer.");
                }

            }

            if (errorMessages.Any())
            {
                var errors = "";
                foreach (var s in errorMessages)
                {
                    errors += s + "<br>";
                }
                TempData["Error"] = errors;
                return RedirectToAction(MVC.PvP.Play());
            }

            TempData["Result"] = "Your duel has started!";
            DuelProcedures.BeginDuel(duel.Id);

            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult DuelDetail(int id)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var duel = DuelProcedures.GetDuel(id);
            if (me.InDuel != duel.Id)
            {
                TempData["Error"] = "You are not in this duel.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var output = new DuelPlayersViewModel
            {
                Duel = duel,
                Combatants = DuelProcedures.GetPlayerViewModelsInDuel(duel.Id)
            };

            ViewBag.CurrentTurn = PvPWorldStatProcedures.GetWorldTurnNumber();
            ViewBag.TurnsRemaining = PvPStatics.MaximumDuelTurnLength - (ViewBag.CurrentTurn - duel.StartTurn);

            return View(MVC.Duel.Views.DuelDetail, output);
        }

        public virtual ActionResult AdvanceTurn()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            if (me.InDuel <= 0)
            {
                TempData["Error"] = "You are not in a duel.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var duel = DuelProcedures.GetDuel(me.InDuel);

            var combatants = DuelProcedures.GetPlayerViewModelsInDuel(duel.Id);

            if (!PvPStatics.ChaosMode)
            {
                foreach (var p in combatants)
                {
                    if (p.Player.TimesAttackingThisUpdate < PvPStatics.MaxAttacksPerUpdate)
                    {
                        TempData["Error"] = "Cannot advance this turn." + p.Player.GetFullName() + " has not used up all of their attacks.";
                        return RedirectToAction(MVC.PvP.Play());
                    }
                }
            }

            foreach (var p in combatants)
            {
                PlayerProcedures.SetAttackCount(p.Player.ToDbPlayer(), 0);
                PlayerProcedures.SetCleanseMeditateCount(p.Player.ToDbPlayer(), 0);
                var message = "<b>" + me.GetFullName() + " has advanced the duel turn.  Attacks and cleanse/meditate limits have been reset.  Attacks may resume in 20 seconds.</b>";
                PlayerLogProcedures.AddPlayerLog(p.Player.Id, message, true);
                NoticeService.PushNotice(p.Player.Id, message, NoticeService.PushType__PlayerLog);
            }

            DuelProcedures.SetLastDuelAttackTimestamp(duel.Id);

            TempData["Result"] = "Duel turn advanced.  All combatants have had their attack and cleanse/meditate limits reset.  Attacks may resume in 20 seconds.";
            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult DuelTimeout()
        {

            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            if (me.InDuel <= 0)
            {
                TempData["Error"] = "You are not in a duel.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var duel = DuelProcedures.GetDuel(me.InDuel);

            var turnsLeft = PvPStatics.MaximumDuelTurnLength - (PvPWorldStatProcedures.GetWorldTurnNumber() - duel.StartTurn);

            if (turnsLeft > 0)
            {
                TempData["Error"] = "You cannot end this duel as there are still turns remaining.";
                return RedirectToAction(MVC.PvP.Play());
            }


            DuelProcedures.EndDuel(duel.Id, DuelProcedures.TIMEOUT);

            TempData["Result"] = "This duel has timed out in a no-winner result.";
            return RedirectToAction(MVC.PvP.Play());


        }

    }
}
