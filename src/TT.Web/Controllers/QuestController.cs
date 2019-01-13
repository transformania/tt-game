using System;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using TT.Domain;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Messages.Queries;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels.Quest;

namespace TT.Web.Controllers
{
    [Authorize]
    [OutputCache(Location = OutputCacheLocation.None)]
    public partial class QuestController : Controller
    {

        public virtual ActionResult StartQuest(int Id)
        {
            var myMembershipId = User.Identity.GetUserId();
            if (PvPStatics.AnimateUpdateInProgress)
            {
                TempData["Error"] = "Player update portion of the world update is still in progress.";
                TempData["SubError"] = "Try again a bit later when the update has progressed farther along.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert player is in an okay form to do this
            if (me.Mobility != PvPStatics.MobilityFull)
            {
                TempData["Error"] = "You must be animate in order to begin a quest.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that this player is not in a duel
            if (me.InDuel > 0)
            {
                TempData["Error"] = "You must finish your duel before you begin a quest.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that this player is not in a quest
            if (me.InQuest > 0)
            {
                TempData["Error"] = "You must finish your current quest before you start another.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player has not been in combat recently
            var lastAttackTimeAgo = Math.Abs(Math.Floor(me.GetLastCombatTimestamp().Subtract(DateTime.UtcNow).TotalMinutes));
            if (lastAttackTimeAgo < PvPStatics.MinutesSinceLastCombatBeforeQuestingOrDuelling)
            {
                TempData["Error"] = "You have been in combat too recently in order to begin this quest.";
                TempData["SubError"] = "You must stay out of combat for another " + (PvPStatics.MinutesSinceLastCombatBeforeQuestingOrDuelling - lastAttackTimeAgo) + " minutes.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var questStart = QuestProcedures.GetQuest(Id);

            // assert player is in the correct place
            if (me.dbLocationName != questStart.Location)
            {
                TempData["Error"] = "You are not in the correct location to begin this quest.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var gameTurnNum = PvPWorldStatProcedures.GetWorldTurnNumber();

            // assert player did not fail or abandon this quest too soon ago
            var lastTurnAttempted = QuestProcedures.GetLastTurnQuestEnded(me, questStart.Id);
            if (PvPWorldStatProcedures.GetWorldTurnNumber() - lastTurnAttempted < QuestStatics.QuestFailCooldownTurnLength)
            {
                TempData["Error"] = "You recently failed or abandoned this quest.";
                TempData["SubError"] = "You must wait another " + (QuestStatics.QuestFailCooldownTurnLength - (gameTurnNum - lastTurnAttempted)) + " turns before you can attempt this quest again.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var questPlayerStatuses = QuestProcedures.GetQuestPlayerStatuses(me);

            var canStartQuest = QuestProcedures.PlayerCanBeginQuest(me, questStart, questPlayerStatuses, gameTurnNum);

            // assert player meets level / game turn requirements for this quest
            if (!canStartQuest)
            {
                TempData["Error"] = "You do not meet all of the criteria to begin this quest.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // all checks have passed; start the player on this quest
            QuestProcedures.PlayerBeginQuest(me, questStart);
            LocationLogProcedures.AddLocationLog(me.dbLocationName, "<span class='playerMediatingNotification'><b>" + me.GetFullName() + "</b> began the quest <b>" + questStart.Name + "</b> here.</span>");

            TempData["Result"] = "You started the quest " + questStart.Name + ".";
            return RedirectToAction(MVC.Quest.Questing());
        }

        public virtual ActionResult QuestsAvailableHere()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            var output = new QuestsAtLocationViewModel
            {
                AllQuests = QuestProcedures.GetAllQuestStartsAtLocation(me.dbLocationName),
                AvailableQuests = QuestProcedures.GetAvailableQuestsAtLocation(me, PvPWorldStatProcedures.GetWorldTurnNumber()),
            };

            return View(MVC.Quest.Views.QuestsAvailableHere, output);
        }

        public virtual ActionResult Questing()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            var questStart = QuestProcedures.GetQuest(me.InQuest);
            return View(MVC.Quest.Views.Questing, questStart);
        }

        public virtual ActionResult Quest()
        {
            IQuestRepository repo = new EFQuestRepository();

            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            var output = new QuestPlayPageViewModel();
            output.Player = PlayerProcedures.GetPlayerFormViewModel(me.Id);
            output.QuestStart = QuestProcedures.GetQuest(me.InQuest);
            output.QuestState = QuestProcedures.GetQuestState(me.InQuestState);
            output.QuestConnections = QuestProcedures.GetChildQuestConnections(me.InQuestState);
            output.BuffBox = ItemProcedures.GetPlayerBuffs(me);
            output.QuestPlayerVariables = QuestProcedures.GetAllQuestPlayerVariablesFromQuest(output.QuestStart.Id, me.Id);
            output.NewMessages = DomainRegistry.Repository.FindSingle(new GetUnreadMessageCountByPlayer { OwnerId = me.Id });

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            output.SetConnectionText((string)TempData["ConnectionText"]);

            return PartialView(MVC.Quest.Views.Quest, output);
        }

        public virtual ActionResult Choice(int Id)
        {
            IQuestRepository repo = new EFQuestRepository();

            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert player is animate
            if (me.Mobility != PvPStatics.MobilityFull)
            {
                TempData["Error"] = "You are not animate.";
                return RedirectToAction(MVC.Quest.Quest());
            }

            // assert player has enough AP
            if (me.ActionPoints < QuestStatics.ActionAPCost)
            {
                TempData["Error"] = "You don't have enough action points for this.";
                TempData["SubError"] = "Wait a while; you will get more action points soon.";
                return RedirectToAction(MVC.Quest.Quest());
            }

            var currentState = QuestProcedures.GetQuestState(me.InQuestState);
            var desiredConnection = QuestProcedures.GetQuestConnection(Id);
            var nextState = QuestProcedures.GetQuestState(desiredConnection.QuestStateToId);

            // assert desired state is in same quest
            if (nextState.QuestId != me.InQuest || desiredConnection.QuestId != me.InQuest)
            {
                TempData["Error"] = "Unavailable";
                return RedirectToAction(MVC.Quest.Quest());
            }

            // assert a connection does exist between current state and chosen one
            if (desiredConnection.QuestStateFromId != me.InQuestState)
            {
                TempData["Error"] = "Unavailable";
                return RedirectToAction(MVC.Quest.Quest());
            }

            var output = new QuestPlayPageViewModel();
            output.Player = PlayerProcedures.GetPlayerFormViewModel(me.Id);
            output.QuestStart = QuestProcedures.GetQuest(me.InQuest);
            output.QuestPlayerVariables = QuestProcedures.GetAllQuestPlayerVariablesFromQuest(output.QuestStart.Id, me.Id);
            var buffs = ItemProcedures.GetPlayerBuffs(me);

            // assert player has the right requirements for this
            if (!QuestProcedures.QuestConnectionIsAvailable(desiredConnection, me, buffs, output.QuestPlayerVariables))
            {
                TempData["Error"] = "You're not able to do that.";
                return RedirectToAction(MVC.Quest.Quest());
            }

            // make rolls for pass / fail
            if (desiredConnection.RequiresRolls())
            {
                var passes = QuestProcedures.RollForQuestConnection(desiredConnection, me, buffs, output.QuestPlayerVariables);

                // player fails; reroute to the failure quest state
                if (!passes)
                {
                    nextState = QuestProcedures.GetQuestState(desiredConnection.QuestStateFailToId);
                    TempData["RollResult"] = "fail";
                }
                else
                {
                    TempData["RollResult"] = "pass";
                }
            }
            else
            {
                TempData["RollResult"] = "none";
            }

            QuestProcedures.PlayerSetQuestState(me, nextState);
            QuestProcedures.ProcessQuestStatePreactions(me, nextState);

            PlayerProcedures.ChangePlayerActionManaNoTimestamp(1, 0, 0, me.Id);

            TempData["ConnectionText"] = desiredConnection.Text;

            return RedirectToAction(MVC.Quest.Quest());
        }

        public virtual ActionResult Abandon()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert that this player is in a quest
            if (me.InQuest <= 0)
            {
                TempData["Error"] = "You are not in a quest.";
                return RedirectToAction(MVC.PvP.Play());
            }

            return PartialView(MVC.Quest.Views.Abandon);
        }

        public virtual ActionResult AbandonConfirm()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert that this player is in a quest
            if (me.InQuest <= 0)
            {
                TempData["Error"] = "You are not in a quest.";
                return RedirectToAction(MVC.PvP.Play());
            }


            // assert player is not currently in a quest with an end state
            var state = QuestProcedures.GetQuestState(me.InQuestState);

            if (state.QuestEnds != null && state.QuestEnds.Any())
            {
                TempData["Error"] = "It is too late to abandon this quest.";
                TempData["SubError"] = "You must accept the consequences of your actions, be they for good or ill!";
                return RedirectToAction(MVC.Quest.Quest());
            }

            QuestProcedures.PlayerEndQuest(me, (int)QuestStatics.QuestOutcomes.Failed);

            TempData["Result"] = "You abandoned your quest.";
            return RedirectToAction(MVC.PvP.Play());

        }

        public virtual ActionResult EndQuest(bool restore)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert that this player is in a quest
            if (me.InQuest <= 0)
            {
                TempData["Error"] = "You are not in a quest.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var quest = QuestProcedures.GetQuest(me.InQuest);
            var state = QuestProcedures.GetQuestState(me.InQuestState);

            // assert that there is an end state to this quest state
            if (!state.QuestEnds.Any())
            {
                TempData["Error"] = "You are not yet at the end of your quest!";
                TempData["SubError"] = "If it is impossible for you to continue with this quest, you may abandon it.";
                return RedirectToAction(MVC.Quest.Quest());
            }

            var endType = state.QuestEnds.First().EndType;

            // if the player is not animate, either restore them or create a new item for them to exist as
            if (me.Mobility != PvPStatics.MobilityFull)
            {
                if (restore && me.Mobility != PvPStatics.MobilityFull)
                {
                    PlayerProcedures.InstantRestoreToBase(me);
                }
                else if (!restore)
                {
                    var newform = FormStatics.GetForm(me.FormSourceId);
                    ItemProcedures.PlayerBecomesItem(me, newform, null);
                }
            }

            // fail!
            if (endType == (int)QuestStatics.QuestOutcomes.Failed)
            {
                QuestProcedures.PlayerEndQuest(me, (int)QuestStatics.QuestOutcomes.Failed);
                QuestProcedures.ClearQuestPlayerVariables(me.Id, me.InQuest);

                StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__QuestsFailed, 1);

                TempData["Result"] = "You unfortunately failed the quest <b>" + quest.Name + "</b>.  Better luck next time!  If there is one...";
                return RedirectToAction(MVC.PvP.Play());
            }

            // pass!
            var victoryMessage = QuestProcedures.PlayerEndQuest(me, (int)QuestStatics.QuestOutcomes.Completed);
            QuestProcedures.ClearQuestPlayerVariables(me.Id, me.InQuest);

            StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__QuestsPassed, 1);

            TempData["Result"] = "Congratulations, you completed the quest <b>" + quest.Name + "</b>!" + victoryMessage;

            return RedirectToAction(MVC.PvP.Play());

        }

        public virtual ActionResult ResetQuests()
        {

            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            if (!PvPStatics.ChaosMode)
            {
                TempData["Error"] = "You can only do this in chaos mode.";
                return RedirectToAction(MVC.PvP.Play());
            }

            QuestProcedures.PlayerClearAllQuestStatuses(me);
            QuestProcedures.ClearQuestPlayerVariables(me.Id, me.InQuest);

            return RedirectToAction(MVC.PvP.Play());

        }
    }
}