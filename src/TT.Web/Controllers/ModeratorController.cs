using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TT.Domain;
using TT.Domain.Commands.Identity;
using TT.Domain.Queries.Messages;
using TT.Domain.Queries.Players;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Web.Controllers
{
    [Authorize(Roles=PvPStatics.Permissions_Moderator)]
    public class ModeratorController : Controller
    {
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewAbusiveMessages()
        {
            if (!User.IsInRole(PvPStatics.Permissions_Admin) && !User.IsInRole(PvPStatics.Permissions_Moderator))
            {
                return RedirectToAction("Play", "PvP");
            }
            var output = DomainRegistry.Repository.Find(new GetMessagesReportedAbusive());
            return View(output);
        }

        public ActionResult ViewStrikes(string id)
        {
            var output = new AddStrikeViewModel
            {
                UserId = id,
                PlayerUserStrikesDetail = DomainRegistry.Repository.FindSingle(new GetPlayerUserStrikes {UserId = id })
            };
            return View(output);
        }

        [ValidateAntiForgeryToken]
        public ActionResult AddStrike(AddStrikeViewModel input)
        {

            // TODO:  get rid of this crap once all links to round number are done via integer, not string
            var round = Int32.Parse(PvPStatics.AlphaRound.Split(' ')[2]); // 'Alpha Round 42' gets split up, take the 3rd position which is the number... hack, I know

            try
            {
                DomainRegistry.Repository.Execute(new AddStrike { UserId = input.UserId, ModeratorId = User.Identity.GetUserId(), Reason = input.Reason, Round = round });
                var player = DomainRegistry.Repository.FindSingle(new GetPlayerByUserId {UserId = input.UserId});
                TempData["Result"] = $"Strike given to player <b>{player.FullName}</b> with account name <b>{player.User.UserName}</b>.";
                return RedirectToAction("Play", "PvP");
            }
            catch (DomainException e)
            {
                TempData["Error"] = "Error: " + e.Message;
                return RedirectToAction("Play", "PvP");
            }
            
        }
    }
}