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
            return View();
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

            return PartialView(output);
        }

        public ActionResult Choice(int Id)
        {
            IQuestRepository repo = new EFQuestRepository();

            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            return RedirectToAction("Quest");
        }
    }
}