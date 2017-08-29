using System;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Commands;
using TT.Domain.Messages.Queries;
using TT.Domain.Players.Queries;
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
    }
}