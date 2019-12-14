using System;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Commands;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

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

            var output = new ReportViewModel
            {
                ReportedId = Id,
                Name = reported.GetFullName()
            };

            return View(MVC.Report.Views.Report, output);
        }

        // GET: /Messages
        [HttpGet]
        public virtual ActionResult Question()
        {
            var output = new ReportViewModel
            {
                ReportedId = User.Identity.GetUserId()
            };

            return View(MVC.Report.Views.Question, output);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult SubmitReport(ReportViewModel model)
{
            var myMembershipId = User.Identity.GetUserId();
            var round = Int32.Parse(PvPStatics.AlphaRound.Split(' ')[2]);

            try
            {
                DomainRegistry.Repository.Execute(new SubmitReport
                {
                    Reason = model.Reason,
                    ReportedId = model.ReportedId,
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