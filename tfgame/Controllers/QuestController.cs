using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.Models;
using tfgame.Procedures;
using tfgame.Statics;
using tfgame.ViewModels;
using tfgame.ViewModels.Quest;

namespace tfgame.Controllers
{
    [Authorize]
    public class QuestController : Controller
    {

        public ActionResult StartQuest(int Id)
        {
            string myMembershipId = User.Identity.GetUserId();
            if (PvPStatics.AnimateUpdateInProgress == true)
            {
                TempData["Error"] = "Player update portion of the world update is still in progress.";
                TempData["SubError"] = "Try again a bit later when the update has progressed farther along.";
                return RedirectToAction("Play", "PvP");
            }

            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert player is in an okay form to do this
            if (me.Mobility != PvPStatics.MobilityFull)
            {
                TempData["Error"] = "You must be animate in order to begin a quest.";
                return RedirectToAction("Play", "PvP");
            }

            // assert that this player is not in a duel
            if (me.InDuel > 0)
            {
                TempData["Error"] = "You must finish your duel before you begin a quest.";
                return RedirectToAction("Play", "PvP");
            }

            // assert that this player is not in a quest
            if (me.InQuest > 0)
            {
                TempData["Error"] = "You must finish your current quest before you start another.";
                return RedirectToAction("Play", "PvP");
            }

            // assert player has not been in combat recently
            double lastAttackTimeAgo = Math.Abs(Math.Floor(me.GetLastCombatTimestamp().Subtract(DateTime.UtcNow).TotalMinutes));
            if (lastAttackTimeAgo < PvPStatics.DuelNoCombatMinutes)
            {
                TempData["Error"] = "You have been in combat too recently in order to begin this quest.";
                TempData["SubError"] = "You must stay out of combat for another " + (PvPStatics.DuelNoCombatMinutes - lastAttackTimeAgo) + " minutes.";
                return RedirectToAction("Play", "PvP");
            }

            QuestStart questStart = QuestProcedures.GetQuest(Id);

            // assert player is in the correct place
            if (me.dbLocationName != questStart.Location)
            {
                TempData["Error"] = "You are not in the correct location to begin this quest.";
                return RedirectToAction("Play", "PvP");
            }

            IEnumerable<QuestPlayerStatus> questPlayerStatuses = QuestProcedures.GetQuestPlayerStatuses(me);
            bool canStartQuest = QuestProcedures.PlayerCanBeginQuest(me, questStart, questPlayerStatuses, PvPWorldStatProcedures.GetWorldTurnNumber());

            // assert player meets level / game turn requirements for this quest
            if (canStartQuest==false)
            {
                TempData["Error"] = "You do not meet all of the criteria to begin this quest.";
                return RedirectToAction("Play", "PvP");
            }

            // all checks have passed; start the player on this quest
            QuestProcedures.PlayerBeginQuest(me, questStart);

            TempData["Result"] = "You started the quest " + questStart.Name + ".";
            return RedirectToAction("Questing", "Quest");
        }

        public ActionResult QuestsAvailableHere()
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            IEnumerable<QuestStart> quests = QuestProcedures.GetAvailableQuestsAtLocation(me, PvPWorldStatProcedures.GetWorldTurnNumber());
            return View(quests);
        }

        public ActionResult Questing()
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            QuestStart questStart = QuestProcedures.GetQuest(me.InQuest);
            return View(questStart);
        }

        public ActionResult Quest()
        {
            IQuestRepository repo = new EFQuestRepository();

            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            QuestPlayPageViewModel output = new QuestPlayPageViewModel();
            output.Player = PlayerProcedures.GetPlayerFormViewModel(me.Id);
            output.QuestStart = QuestProcedures.GetQuest(me.InQuest);
            output.QuestState = QuestProcedures.GetQuestState(me.InQuestState);
            output.ChildQuestStates = QuestProcedures.GetChildQuestStates(me.InQuestState);
            output.BuffBox = ItemProcedures.GetPlayerBuffsSQL(me);

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            return PartialView(output);
        }

        public ActionResult Choice(int Id)
        {
            IQuestRepository repo = new EFQuestRepository();

            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert player is animate
            if (me.Mobility != PvPStatics.MobilityFull)
            {
                TempData["Error"] = "You are not animate.";
                return RedirectToAction("Quest");
            }

            QuestState currentState = QuestProcedures.GetQuestState(me.InQuestState);
            QuestState desiredState = QuestProcedures.GetQuestState(Id);

            // assert desired state is in same quest
            if (desiredState.QuestId != me.InQuest)
            {
                TempData["Error"] = "Unavailable";
                return RedirectToAction("Quest");
            }

            QuestPlayPageViewModel output = new QuestPlayPageViewModel();
            output.Player = PlayerProcedures.GetPlayerFormViewModel(me.Id);
            output.QuestStart = QuestProcedures.GetQuest(me.InQuest);
            BuffBox buffs = ItemProcedures.GetPlayerBuffsSQL(me);

            // assert player has the right requirements for this
            if (QuestProcedures.QuestStateIsAvailable(desiredState, me, buffs) == false)
            {
                TempData["Error"] = "You're not able to do that.";
                return RedirectToAction("Quest");
            }

            QuestProcedures.PlayerSetQuestState(me, desiredState);
            PlayerProcedures.ChangePlayerActionManaNoTimestamp(1, 0, 0, me.Id);

            return RedirectToAction("Quest");
        }

        public ActionResult Abandon()
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert that this player is in a quest
            if (me.InQuest <= 0)
            {
                TempData["Error"] = "You are not in a quest.";
                return RedirectToAction("Play", "PvP");
            }

            return PartialView();
        }

        public ActionResult AbandonConfirm()
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert that this player is in a quest
            if (me.InQuest <= 0)
            {
                TempData["Error"] = "You are not in a quest.";
                return RedirectToAction("Play", "PvP");
            }


            // assert player is not currently in a quest with an end state
            QuestState state = QuestProcedures.GetQuestState(me.InQuestState);

            if (state.QuestEnds != null && state.QuestEnds.Count() > 0)
            {
                TempData["Error"] = "It is too late to abandon this quest.";
                TempData["SubError"] = "You must accept the consequences of your actions, be they for good or ill!";
                return RedirectToAction("Quest", "Quest");
            }

            QuestProcedures.PlayerEndQuest(me, (int)QuestStatics.QuestOutcomes.Failed);

            TempData["Result"] = "You abandoned your quest.";
            return RedirectToAction("Play", "PvP");

        }

        public ActionResult EndQuest()
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert that this player is in a quest
            if (me.InQuest <= 0)
            {
                TempData["Error"] = "You are not in a quest.";
                return RedirectToAction("Play", "PvP");
            }

            QuestStart quest = QuestProcedures.GetQuest(me.InQuest);
            QuestState state = QuestProcedures.GetQuestState(me.InQuestState);

            // assert that there is an end state to this quest state
            if (state.QuestEnds.Count()==0)
            {
                TempData["Error"] = "You are not yet at the end of your quest!";
                TempData["SubError"] = "If it is impossible for you to continue with this quest, you may abandon it.";
                return RedirectToAction("Quest", "Quest");
            }

            int endType = state.QuestEnds.First().EndType;

            // fail!
            if (endType == (int)QuestStatics.QuestOutcomes.Failed)
            {
                QuestProcedures.PlayerEndQuest(me, (int)QuestStatics.QuestOutcomes.Failed);
                TempData["Result"] = "You unfortunately failed the quest <b>" + quest.Name + "</b>.  Better luck next time!  If there is one...";
                return RedirectToAction("Play", "PvP");
            }

            // pass!
            QuestProcedures.PlayerEndQuest(me, (int)QuestStatics.QuestOutcomes.Completed);
            TempData["Result"] = "Congratulations, you completed the quest " + quest.Name + "!";
            return RedirectToAction("Play", "PvP");

        }

        public ActionResult ResetQuests()
        {

            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            if (PvPStatics.ChaosMode == false)
            {
                TempData["Error"] = "You can only do this in chaos mode.";
                return RedirectToAction("Play", "PvP");
            }

            QuestProcedures.PlayerClearAllQuestStatuses(me);
            
            return RedirectToAction("Play", "PvP");

        }
    }
}