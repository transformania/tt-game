using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TT.Domain;
using TT.Domain.Items.Queries;
using TT.Domain.Procedures;
using TT.Domain.ViewModels;

namespace TT.Web.Controllers
{
    [Authorize]
    public partial class LeaderboardController : Controller
    {

        public virtual ActionResult Leaderboard()
        {
            var myMembershipId = User.Identity.GetUserId();
            if (myMembershipId.IsNullOrEmpty())
            {
                ViewBag.MyName = "";
            }
            else
            {
                var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
                ViewBag.MyName = me == null ? "" : me.GetFullName();
            }
            return View(MVC.Leaderboard.Views.Leaderboard, PlayerProcedures.GetLeadingPlayers__XP(100));
        }

        public virtual ActionResult PvPLeaderboard()
        {
            var myMembershipId = User.Identity.GetUserId();
            if (myMembershipId.IsNullOrEmpty())
            {
                ViewBag.MyName = "";
            }
            else
            {
                var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
                ViewBag.MyName = me == null ? "" : me.GetFullName();
            }
            return View(PlayerProcedures.GetLeadingPlayers__PvP(100));
        }

        public virtual ActionResult ItemLeaderboard()
        {
            var myMembershipId = User.Identity.GetUserId();
            if (myMembershipId.IsNullOrEmpty())
            {
                ViewBag.MyName = "";
            }
            else
            {
                var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
                ViewBag.MyName = me == null ? "" : me.GetFullName();
            }

            var output = DomainRegistry.Repository.Find(new GetHighestLevelPlayerItems { Limit = 100 });

            return View(output);
        }

        public virtual ActionResult PlayerStatsLeaders()
        {
            List<PlayerAchievementViewModel> output = StatsProcedures.GetPlayerMaxStats().ToList();
            return View(output);
        }

        public virtual ActionResult OldLeaderboards(string round)
        {
            // TODO: T4ize
            return View("~/Views/Leaderboard/RoundLeaderboards/Alpha_" + round + ".cshtml");
        }

        public virtual ActionResult OldLeaderboards_Achievements(string round)
        {
            // TODO: T4ize
            return View("~/Views/Leaderboard/RoundLeaderboards/Statistics/Alpha_" + round + ".cshtml");
        }

        public virtual ActionResult OldLeaderboards_Item(string round)
        {
            // TODO: T4ize
            return View("~/Views/Leaderboard/RoundLeaderboards/Items/Alpha_" + round + ".cshtml");
        }

        public virtual ActionResult OldLeaderboards_XP(string round)
        {
            // TODO: T4ize
            return View("~/Views/Leaderboard/RoundLeaderboards/XP/Alpha_" + round + ".cshtml");
        }

    }
}