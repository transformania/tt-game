using System;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Commands;
using TT.Domain.Procedures;
using TT.Domain.Statics;

namespace TT.Web.Controllers
{
    [Authorize]
    public partial class ReportController : Controller
    {

        // GET: /Messages
        [HttpGet]
        public virtual ActionResult Report(string Id)
        {
            var reported = PlayerProcedures.GetPlayerFromMembership(Id);
            return View(MVC.Report.Views.Report, reported);
        }

        [HttpPost]
        public virtual ActionResult SubmitReport(string reportedId, string reason)
        {
            var myMembershipId = User.Identity.GetUserId();
            var round = Int32.Parse(PvPStatics.AlphaRound.Split(' ')[2]);

            try
            {
                DomainRegistry.Repository.Execute(new SubmitReport
                {
                    Reason = reason,
                    ReportedId = reportedId,
                    ReporterId = myMembershipId,
                    Round = round
                });
                TempData["Result"] = "Your report has been submitted.  Please wait for a moderator to act upon it.";
            }
            catch (DomainException e)
            {
                TempData["Error"] = $"Failed to submit report.  Reason: {e.Message}";
                return RedirectToAction(MVC.PvP.Play());
            }

            return RedirectToAction(MVC.PvP.Play());
        }
    }
}