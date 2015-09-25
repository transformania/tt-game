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
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            QuestStart questStart = QuestProcedures.GetQuest(me.InQuest);

            bool canStartQuest = QuestProcedures.PlayerCanBeginQuest(me, questStart, null, PvPWorldStatProcedures.GetWorldTurnNumber());

            return RedirectToAction("Choice","Quest");
        }

        public ActionResult QuestsAvailableHere()
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            IEnumerable<QuestStart> quests = QuestProcedures.GetAvailableQuestsAtLocation(me, PvPWorldStatProcedures.GetWorldTurnNumber());
            return View(quests);
        }

        public ActionResult Quest()
        {
            IQuestRepository repo = new EFQuestRepository();

            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            return View();
        }

        public ActionResult Choice()
        {
            IQuestRepository repo = new EFQuestRepository();

            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            return PartialView();
        }
    }
}