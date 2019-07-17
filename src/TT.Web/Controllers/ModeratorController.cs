using System;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Commands;
using TT.Domain.Identity.DTOs;
using TT.Domain.Identity.Queries;
using TT.Domain.Messages.Queries;
using TT.Domain.Players.Queries;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Web.Controllers
{
    [Authorize(Roles = PvPStatics.Permissions_Moderator)]
    public partial class ModeratorController : Controller
    {

        public virtual ActionResult Index()
        {
            return View(MVC.Moderator.Views.Index);
        }

        public virtual ActionResult ViewAbusiveMessages()
        {
            if (!User.IsInRole(PvPStatics.Permissions_Admin) && !User.IsInRole(PvPStatics.Permissions_Moderator))
            {
                return RedirectToAction(MVC.PvP.Play());
            }
            var output = DomainRegistry.Repository.Find(new GetMessagesReportedAbusive());
            return View(MVC.Moderator.Views.ViewAbusiveMessages, output);
        }

        public virtual ActionResult ViewStrikes(string id)
        {
            var output = new AddStrikeViewModel
            {
                UserId = id,
                PlayerUserStrikesDetail = DomainRegistry.Repository.FindSingle(new GetPlayerUserStrikes { UserId = id })
            };
            return View(MVC.Moderator.Views.ViewStrikes, output);
        }

        [ValidateAntiForgeryToken]
        public virtual ActionResult AddStrike(AddStrikeViewModel input)
        {

            // TODO:  get rid of this crap once all links to round number are done via integer, not string
            var round = Int32.Parse(PvPStatics.AlphaRound.Split(' ')[2]); // 'Alpha Round 42' gets split up, take the 3rd position which is the number... hack, I know

            try
            {
                DomainRegistry.Repository.Execute(new AddStrike { UserId = input.UserId, ModeratorId = User.Identity.GetUserId(), Reason = input.Reason, Round = round });
                var player = DomainRegistry.Repository.FindSingle(new GetPlayerByUserId { UserId = input.UserId });
                TempData["Result"] = $"Strike given to player <b>{player.FullName}</b> with account name <b>{player.User.UserName}</b>.";
                return RedirectToAction(MVC.PvP.Play());
            }
            catch (DomainException e)
            {
                TempData["Error"] = "Error: " + e.Message;
                return RedirectToAction(MVC.PvP.Play());
            }

        }

        public virtual ActionResult ViewReports()
        {
            var output = DomainRegistry.Repository.Find(new GetAllReports());

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            return View(MVC.Moderator.Views.ViewReports, output);
        }

        public virtual ActionResult HandleReport(int reportId)
        {
            var output = DomainRegistry.Repository.FindSingle(new GetReport {ReportId = reportId});
            return View(MVC.Moderator.Views.HandleReport, output);
        }

        [ValidateAntiForgeryToken]
        public virtual ActionResult HandleReportSend(ReportDetail input)
        {
            try
            {
                DomainRegistry.Repository.Execute(new HandleReport
                {
                    ModeratorResponse = input.ModeratorResponse,
                    ReportId = input.Id
                });
            }
            catch (DomainException e)
            {
                TempData["Error"] = "Error: " + e.Message;
                return RedirectToAction(MVC.PvP.Play());
            }
            TempData["Result"] = "You have replied to this report.";
            return RedirectToAction(MVC.Moderator.ViewReports());
        }

        public virtual ActionResult SetAccountLockoutDate(string userId)
        {
            var player = PlayerProcedures.GetPlayerFromMembership(userId);
            return View(MVC.Moderator.Views.SetAccountLockoutDate, new SuspendTimeoutViewModel{Player = player, UserId = player.MembershipId, date = DateTime.UtcNow.AddYears(99)});
        }

        [ValidateAntiForgeryToken]
        public virtual ActionResult SetAccountLockoutDateSend(SuspendTimeoutViewModel suspendTimeoutViewModel)
        {
            try
            {
                TempData["Result"] = DomainRegistry.Repository.Execute(new SetLockoutTimestamp { UserId = suspendTimeoutViewModel.UserId, date = suspendTimeoutViewModel.date });
                return RedirectToAction(MVC.PvP.Play());
            }
            catch (DomainException e)
            {
                TempData["Error"] = "Error: " + e.Message;
                return RedirectToAction(MVC.PvP.Play());
            }

        }
    }
}