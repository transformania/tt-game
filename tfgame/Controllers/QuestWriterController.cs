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

namespace tfgame.Controllers
{
    [Authorize(Roles = PvPStatics.Permissions_QuestWriter)]
    public class QuestWriterController : Controller
    {
        // GET: QuestWriter
        public ActionResult Index()
        {

            return View();
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


            return View(questStart);
        }

        public ActionResult QuestStartSend(QuestStart input)
        {

            QuestWriterProcedures.SaveQuestStart(input);

            TempData["Result"] = "Save succeeded.";
            return RedirectToAction("Play", "PvP");
        }

    }
}