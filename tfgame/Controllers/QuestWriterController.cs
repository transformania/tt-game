using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.Procedures;
using tfgame.Statics;
using tfgame.ViewModels.Quest;

namespace tfgame.Controllers
{
    [Authorize(Roles = PvPStatics.Permissions_QuestWriter)]
    public class QuestWriterController : Controller
    {
        // GET: QuestWriter
        public ActionResult Index()
        {
            List<QuestStart> output = QuestProcedures.GetAllQuestStarts().ToList();

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            return View(output);
        }

        public ActionResult QuestStart(int Id)
        {

            IQuestRepository repo = new EFQuestRepository();

            QuestStart questStart = repo.QuestStarts.FirstOrDefault(q => q.Id == Id);

            if (questStart == null)
            {
                questStart = new QuestStart
                {
                    IsLive = false,
                    Name = "[ UNNAMED QUEST ]",
                    MinStartTurn = 0,
                    MinStartLevel = 0,
                    MaxStartTurn = 99999,
                    MaxStartLevel = 99999,
                };
            }

            IEnumerable<QuestStart> allStarts = repo.QuestStarts;
            ViewBag.OtherQuests = allStarts;

            QuestState firstState = repo.QuestStates.FirstOrDefault(f => f.Id == questStart.StartState);
            ViewBag.firstState = firstState;

            return PartialView(questStart);
        }

        public ActionResult QuestStartSend(QuestStart input)
        {

            QuestWriterProcedures.SaveQuestStart(input);

            TempData["Result"] = "Save succeeded.";
            return RedirectToAction("Index", "QuestWriter");
        }

        public ActionResult QuestState(int Id, int QuestId, int ParentStateId)
        {
            IQuestRepository repo = new EFQuestRepository();

            QuestState questState = repo.QuestStates.FirstOrDefault(q => q.Id == Id);

            if (questState == null)
            {
                questState = new QuestState
                {
                    ChoiceText = "[ CHOICE TEXT ]",
                    QuestEndId = -1,
                    ParentQuestStateId = ParentStateId,
                    Text = "",
                    QuestId = QuestId,
                    QuestStateName = "[ state name ]",
                };
            } else
            {

            }

            QuestStateFormViewModel output = new QuestStateFormViewModel();
            output.QuestState = questState;
            output.ParentQuestState = repo.QuestStates.FirstOrDefault(q => q.Id == questState.ParentQuestStateId);
            output.ChildQuestStates = repo.QuestStates.Where(q => q.ParentQuestStateId == questState.Id);

            return PartialView(output);
        }

        public ActionResult QuestStateSend(QuestStateFormViewModel input)
        {

            QuestWriterProcedures.SaveQuestState(input.QuestState);

            TempData["Result"] = "Save succeeded.";
            return RedirectToAction("Index", "QuestWriter");
        }

    }
}