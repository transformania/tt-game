using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels.Quest;

namespace TT.Web.Controllers
{
    [Authorize(Roles = PvPStatics.Permissions_QuestWriter)]
    public partial class QuestWriterController : Controller
    {
        // GET: QuestWriter
        public virtual ActionResult Index()
        {
            List<QuestStart> output = QuestProcedures.GetAllQuestStarts().ToList();

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            return View(output);
        }

        public virtual ActionResult QuestStart(int Id)
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
                    PrerequisiteQuest = 0,
                };
            }

            IEnumerable<QuestStart> allStarts = repo.QuestStarts;
            ViewBag.OtherQuests = allStarts;

            QuestState firstState = repo.QuestStates.FirstOrDefault(f => f.Id == questStart.StartState);
            ViewBag.firstState = firstState;

            return PartialView(questStart);
        }

        public virtual ActionResult QuestStartSend(QuestStart input)
        {

            int newId = QuestWriterProcedures.SaveQuestStart(input);

            QuestWriterProcedures.LogQuestWriterAction(User.Identity.Name, newId, " began new quest with Id <b>" + newId + "</b>.");

            return RedirectToAction(MVC.QuestWriter.QuestStart(newId));
        }

        public virtual ActionResult MarkQuestAsLive(int Id, bool live)
        {
            // assert only admins can view this
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.QuestWriter.QuestStart(Id));
            }

            QuestWriterProcedures.MarkQuestAsLive(Id, live);

            QuestWriterProcedures.LogQuestWriterAction(User.Identity.Name, Id, " marked quest Id <b>" + Id + "</b> as live.");

            return RedirectToAction(MVC.QuestWriter.QuestStart(Id));

        }

        public virtual ActionResult QuestState(int Id, int QuestId, int ParentStateId)
        {
            IQuestRepository repo = new EFQuestRepository();

            QuestState questState = repo.QuestStates.FirstOrDefault(q => q.Id == Id);

            if (questState == null)
            {
                questState = new QuestState
                {
                    QuestEndId = -1,
                    Text = "",
                    QuestId = QuestId,
                    QuestStateName = "[ state name ]",

                    QuestEnds = new List<QuestEnd>(),
                    QuestConnectionRequirements = new List<QuestConnectionRequirement>(),
                    QuestStatePreactions = new List<QuestStatePreaction>(),
                    X = 0,
                    Y = 0,
                    PinToDiagram = true
                };
            }
            else
            {

            }

            QuestStateFormViewModel output = new QuestStateFormViewModel();
            output.QuestState = questState;
            output.QuestConnectionsTo = repo.QuestConnections.Where(q => q.QuestStateToId == output.QuestState.Id && q.QuestStateToId > 0);
            output.QuestConnectionsFailTo = repo.QuestConnections.Where(q => q.QuestStateFailToId == output.QuestState.Id && q.QuestStateFailToId > 0);

            output.QuestConnectionsFrom = repo.QuestConnections.Where(q => q.QuestStateFromId == output.QuestState.Id && q.QuestStateFromId > 0);

            return PartialView(output);
        }

        public virtual ActionResult QuestStateSend(QuestStateFormViewModel input)
        {

            int id = QuestWriterProcedures.SaveQuestState(input.QuestState);

            QuestWriterProcedures.LogQuestWriterAction(User.Identity.Name, input.QuestState.QuestId, " saved quest state Id <b>" + input.QuestState.Id + "</b>.");

            return RedirectToAction(MVC.QuestWriter.QuestState(id, input.QuestState.QuestId, -1));
        }

        public virtual ActionResult QuestStateDelete(int Id)
        {
            IQuestRepository repo = new EFQuestRepository();

            QuestState questState = repo.QuestStates.FirstOrDefault(q => q.Id == Id);

            QuestWriterProcedures.DeleteQuestState(Id);

            QuestWriterProcedures.LogQuestWriterAction(User.Identity.Name, questState.QuestId, " deleted quest state Id <b>" + questState.Id + "</b>.");

            return RedirectToAction(MVC.QuestWriter.ShowAllQuestStates(questState.QuestId));
        }

        public virtual ActionResult QuestConnection(int Id, int QuestId, int FromQuestId, int ToQuestId)
        {

            IQuestRepository repo = new EFQuestRepository();

            QuestConnection questConnection = repo.QuestConnections.FirstOrDefault(q => q.Id == Id);

            if (questConnection == null)
            {
                questConnection = new QuestConnection
                {
                    QuestId = QuestId,
                    QuestConnectionRequirements = new List<QuestConnectionRequirement>(),
                    QuestStateFromId = FromQuestId,
                    QuestStateToId = ToQuestId,
                    QuestStateFailToId = 0,
                };
            }

            QuestConnectionFormViewModel output = new QuestConnectionFormViewModel
            {
                QuestConnection = questConnection,

            };

            if (FromQuestId > 0)
            {
                output.FromQuestState = QuestProcedures.GetQuestState(FromQuestId);
            }
            if (ToQuestId > 0)
            {
                output.ToQuestState = QuestProcedures.GetQuestState(ToQuestId);
            }

            if (questConnection.QuestStateFailToId > 0)
            {
                output.FailToQuestState = QuestProcedures.GetQuestState(questConnection.QuestStateFailToId);
            }

            return PartialView(output);
        }

        public virtual ActionResult QuestConnectionSend(QuestConnectionFormViewModel input)
        {

            int id = QuestWriterProcedures.SaveQuestConnection(input.QuestConnection);

            QuestWriterProcedures.LogQuestWriterAction(User.Identity.Name, input.QuestConnection.QuestId, " saved connection Id <b>" + id + "</b>.");

            return RedirectToAction(MVC.QuestWriter.QuestConnection(id, input.QuestConnection.QuestId, input.QuestConnection.QuestStateFromId, input.QuestConnection.QuestStateToId));
        }

        public virtual ActionResult DeleteQuestConnection(int Id, int QuestId)
        {

            QuestWriterProcedures.DeleteQuestConnection(Id);
            QuestWriterProcedures.LogQuestWriterAction(User.Identity.Name, QuestId, " deleted connection Id <b>" + Id + "</b>.");

            return RedirectToAction(MVC.QuestWriter.ShowAllQuestConnections(QuestId));
        }

        public virtual ActionResult QuestConnectionRequirement(int Id, int QuestId, int QuestConnectionId)
        {
            IQuestRepository repo = new EFQuestRepository();

            QuestConnectionRequirement QuestConnectionRequirement = repo.QuestConnectionRequirements.FirstOrDefault(q => q.Id == Id);
            QuestConnection connection = repo.QuestConnections.FirstOrDefault(q => q.Id == QuestConnectionId);

            if (QuestConnectionRequirement == null)
            {
                QuestConnectionRequirement = new QuestConnectionRequirement
                {
                    QuestId = QuestId,
                    QuestConnectionRequirementName = "[UNNAMED REQUIREMENT]",
                };

                QuestConnectionRequirement.QuestConnectionId = connection;
            }
            else
            {

            }

            QuestConnectionRequirementFormViewModel output = new QuestConnectionRequirementFormViewModel();
            output.QuestConnectionRequirement = QuestConnectionRequirement;
            output.QuestConnection = connection;

            return PartialView(output);
        }

        public virtual ActionResult QuestConnectionRequirementSend(QuestConnectionRequirementFormViewModel input)
        {

            IQuestRepository repo = new EFQuestRepository();
            QuestConnection connection = repo.QuestConnections.FirstOrDefault(q => q.Id == input.QuestConnection.Id);

            int savedId = QuestWriterProcedures.SaveQuestConnectionRequirement(input.QuestConnectionRequirement, connection);

            QuestWriterProcedures.LogQuestWriterAction(User.Identity.Name, connection.QuestId, " saved connection requirement Id <b>" + savedId + "</b>.");

            return RedirectToAction(MVC.QuestWriter.QuestConnectionRequirement(savedId, input.QuestConnectionRequirement.QuestId, connection.Id));
        }

        public virtual ActionResult QuestConnectionRequirementDelete(int Id)
        {

            IQuestRepository repo = new EFQuestRepository();

            QuestConnectionRequirement questConnectionRequirement = repo.QuestConnectionRequirements.FirstOrDefault(q => q.Id == Id);
            QuestConnection connection = repo.QuestConnections.FirstOrDefault(q => q.Id == questConnectionRequirement.QuestConnectionId.Id);
            QuestWriterProcedures.DeleteQuestConnectionRequirement(Id);

            QuestWriterProcedures.LogQuestWriterAction(User.Identity.Name, connection.QuestId, " deleted connection requirement Id <b>" + Id + "</b>.");

            return RedirectToAction(MVC.QuestWriter.QuestConnection(connection.Id, connection.QuestId, connection.QuestStateFromId, connection.QuestStateToId));
        }

        public virtual ActionResult QuestStatePreaction(int Id, int QuestStateId, int QuestId)
        {
            IQuestRepository repo = new EFQuestRepository();

            QuestStatePreaction questStatePreaction = repo.QuestStatePreactions.FirstOrDefault(q => q.Id == Id);
            QuestState state = repo.QuestStates.FirstOrDefault(q => q.Id == QuestStateId);

            if (questStatePreaction == null)
            {
                questStatePreaction = new QuestStatePreaction
                {
                    QuestId = QuestId,
                    QuestStatePreactionName = "[UNNAMED PREACTION]",

                };

                questStatePreaction.QuestStateId = state;
            }
            else
            {

            }

            QuestStatePreactionFormViewModel output = new QuestStatePreactionFormViewModel();
            output.QuestStatePreaction = questStatePreaction;
            output.ParentQuestState = state;

            return PartialView(output);
        }

        public virtual ActionResult QuestStatePreactionSend(QuestStatePreactionFormViewModel input)
        {
            IQuestRepository repo = new EFQuestRepository();
            QuestState state = repo.QuestStates.FirstOrDefault(q => q.Id == input.ParentQuestState.Id);

            int savedId = QuestWriterProcedures.SaveQuestStatePreaction(input.QuestStatePreaction, state);

            QuestWriterProcedures.LogQuestWriterAction(User.Identity.Name, state.QuestId, " saved quest state preaction Id <b>" + savedId + "</b>.");

            return RedirectToAction(MVC.QuestWriter.QuestStatePreaction(savedId, state.Id, input.QuestStatePreaction.QuestId));
        }

        public virtual ActionResult QuestStatePreactionDelete(int Id)
        {

            IQuestRepository repo = new EFQuestRepository();

            QuestStatePreaction questStatePreaction = repo.QuestStatePreactions.FirstOrDefault(q => q.Id == Id);
            QuestState state = repo.QuestStates.FirstOrDefault(q => q.Id == questStatePreaction.QuestStateId.Id);

            QuestWriterProcedures.DeleteQuestStatePreaction(Id);

            QuestWriterProcedures.LogQuestWriterAction(User.Identity.Name, state.QuestId, " deleted quest state preaction Id <b>" + Id + "</b>.");

            return RedirectToAction(MVC.QuestWriter.QuestState(questStatePreaction.QuestStateId.Id, questStatePreaction.QuestId, -1));
        }

        public virtual ActionResult QuestEnd(int Id, int QuestStateId, int QuestId)
        {
            IQuestRepository repo = new EFQuestRepository();

            QuestEnd questEnd = repo.QuestEnds.FirstOrDefault(q => q.Id == Id);
            QuestState state = repo.QuestStates.FirstOrDefault(q => q.Id == QuestStateId);

            if (questEnd == null)
            {
                questEnd = new QuestEnd
                {
                    QuestEndName = "[NAME NOT SET]",
                    QuestId = QuestId,
                    QuestStateId = state,
                };


            }
            else
            {

            }

            QuestEndFormViewModel output = new QuestEndFormViewModel();
            output.QuestEnd = questEnd;
            output.ParentQuestState = repo.QuestStates.FirstOrDefault(q => q.Id == QuestStateId);

            return PartialView(output);
        }

        public virtual ActionResult QuestEndSend(QuestEndFormViewModel input)
        {

            IQuestRepository repo = new EFQuestRepository();
            QuestState state = repo.QuestStates.FirstOrDefault(q => q.Id == input.ParentQuestState.Id);

            int savedId = QuestWriterProcedures.SaveQuestEnd(input.QuestEnd, state);

            QuestWriterProcedures.LogQuestWriterAction(User.Identity.Name, state.QuestId, " saved quest end Id <b>" + savedId + "</b>.");

            return RedirectToAction(MVC.QuestWriter.QuestEnd(savedId, state.Id, input.QuestEnd.QuestId));
        }

        public virtual ActionResult QuestEndDelete(int Id)
        {
            IQuestRepository repo = new EFQuestRepository();

            QuestEnd questEnd = repo.QuestEnds.FirstOrDefault(q => q.Id == Id);
            QuestState state = repo.QuestStates.FirstOrDefault(s => s.Id == questEnd.QuestStateId.Id);

            QuestWriterProcedures.LogQuestWriterAction(User.Identity.Name, state.QuestId, " deleted quest state preaction Id <b>" + Id + "</b>.");

            QuestWriterProcedures.DeleteQuestEnd(Id);

            return RedirectToAction(MVC.QuestWriter.QuestState(questEnd.QuestStateId.Id, state.Id, -1));
        }

        public virtual ActionResult Help()
        {
            return PartialView();
        }

        public virtual ActionResult Diagram(int Id)
        {

            ViewBag.QuestId = Id;

            return PartialView();
        }

        public virtual JsonResult DiagramStatesJSON(int Id)
        {
            IQuestRepository repo = new EFQuestRepository();
            QuestStart start = repo.QuestStarts.FirstOrDefault(q => q.Id == Id);

            //  IEnumerable<PopulationTurnTuple> output = from q in repo.ServerLogs select new PopulationTurnTuple { Turn = q.TurnNumber, Population = q.Population };

            IEnumerable<QuestStateJSONObject> output = from s in repo.QuestStates.Where(q => q.QuestId == Id)
                                                       select new QuestStateJSONObject
                                                       {
                                                           Id = s.Id,
                                                           StateName = s.QuestStateName,
                                                           EndCount = s.QuestEnds.Count(),
                                                           Pin = s.PinToDiagram,
                                                           X = s.X,
                                                           Y = s.Y
                                                       };

            output = output.ToList();

            foreach (QuestStateJSONObject q in output)
            {
                if (q.Id == start.StartState)
                {
                    q.IsStart = true;
                }
            }

            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public virtual JsonResult DiagramConnectionsJSON(int Id)
        {
            IQuestRepository repo = new EFQuestRepository();
            var output = from c in repo.QuestConnections.Where(q => q.QuestId == Id)
                         select new
                         {
                             Id = c.Id,
                             Name = c.ConnectionName,
                             From = c.QuestStateFromId,
                             To = c.QuestStateToId,
                             FailTo = c.QuestStateFailToId,
                             Reqs = c.QuestConnectionRequirements.Count(),
                         };


            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public virtual JsonResult DiagramSaveNodePosition(int Id, string X, string Y)
        {

            IQuestRepository repo = new EFQuestRepository();

            QuestState state = repo.QuestStates.FirstOrDefault(q => q.Id == Id);
            state.PinToDiagram = true;
            state.X = float.Parse(X);
            state.Y = float.Parse(Y);
            repo.SaveQuestState(state);

            var output = "Pinned state " + Id;

            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public virtual JsonResult QuestStatesInQuestJSON(int Id)
        {
            IEnumerable<QuestState> states = QuestWriterProcedures.GetAllQuestsStatesInQuest(Id);
            var output = from s in states
                         select new
                         {
                             Id = s.Id,
                             StateName = s.QuestStateName
                         };

            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult ShowAllQuestStarts()
        {
            IEnumerable<QuestStart> output = QuestProcedures.GetAllQuestStarts();
            return PartialView(output);
        }

        public virtual ActionResult ShowAllQuestStates(int Id)
        {
            IEnumerable<QuestState> output = QuestWriterProcedures.GetAllQuestsStatesInQuest(Id);
            return PartialView(output);
        }

        public virtual ActionResult ShowAllQuestConnections(int Id)
        {
            IEnumerable<QuestConnection> output = QuestWriterProcedures.GetAllQuestsConnectionsInQuest(Id);
            return PartialView(output);
        }

        public virtual JsonResult ShowAllUsedQuestVariables(int Id)
        {
            IQuestRepository repo = new EFQuestRepository();

            List<string> output = QuestProcedures.GetAllPossibleVariablesNamesInQuest(Id);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public virtual JsonResult ShowAllActivePlayerVariables(int Id)
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            IEnumerable<QuestPlayerVariable> variables = QuestProcedures.GetAllQuestPlayerVariablesFromQuest(Id, me.Id);

            var output = from v in variables
                         select new
                         {
                             Name = v.VariableName,
                             Value = v.VariableValue
                         };

            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult ShowQuestWriterLogs(int Id)
        {
            IEnumerable<QuestWriterLog> output = QuestWriterProcedures.GetAllQuestWriterLogs(Id);
            return PartialView(output);
        }

    }
}