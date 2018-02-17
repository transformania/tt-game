using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TT.Domain;
using TT.Domain.Items.Queries;
using TT.Domain.Procedures;
using TT.Domain.ViewModels.World;
using TT.Domain.World.Queries;

namespace TT.Web.Controllers
{
    [Authorize]
    public partial class LeaderboardController : Controller
    {
        private const int LastRoundWithDatabaseLeaderboardData = 43;

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
            return View(MVC.Leaderboard.Views.PvPLeaderboard, PlayerProcedures.GetLeadingPlayers__PvP(100));
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

            return View(MVC.Leaderboard.Views.ItemLeaderboard, output);
        }

        public virtual ActionResult PlayerStatsLeaders()
        {
            var output = StatsProcedures.GetPlayerMaxStats().ToList();
            return View(MVC.Leaderboard.Views.PlayerStatsLeaders, output);
        }

        public virtual ActionResult OldLeaderboards(int round)
        {
            if (round >= LastRoundWithDatabaseLeaderboardData)
            {
                return RedirectToAction(MVC.Leaderboard.OldPvPLeaderboard(round));
            }
            return View("~/Views/Leaderboard/RoundLeaderboards/Alpha_" + round + ".cshtml");
        }

        public virtual ActionResult OldLeaderboards_Achievements(string round)
        {
            // TODO: T4ize
            return View("~/Views/Leaderboard/RoundLeaderboards/Statistics/Alpha_" + round + ".cshtml");
        }

        public virtual ActionResult OldLeaderboards_Item(int round)
        {
            if (round >= LastRoundWithDatabaseLeaderboardData)
            {
                return RedirectToAction(MVC.Leaderboard.OldItemLeaderboard(round));
            }
            // TODO: T4ize
            return View("~/Views/Leaderboard/RoundLeaderboards/Items/Alpha_" + round + ".cshtml");
        }

        public virtual ActionResult OldLeaderboards_XP(int round)
        {
            if (round >= LastRoundWithDatabaseLeaderboardData)
            {
                return RedirectToAction(MVC.Leaderboard.OldXpLeaderboard(round));
            }
            // TODO: T4ize
            return View("~/Views/Leaderboard/RoundLeaderboards/XP/Alpha_" + round + ".cshtml");
        }

        public virtual ActionResult OldPvPLeaderboard(int round)
        {
            var output = new OldPvPLeaderboardViewModel
            {
                Entries = DomainRegistry.Repository.Find(new GetOldPvPLeaderboardEntries { Round = round }),
                Round = round
            };
            return View(MVC.Leaderboard.Views.OldPvPLeaderboard, output);
        }

        public virtual ActionResult OldXpLeaderboard(int round)
        {
            var output = new OldXpLeaderboardViewModel
            {
                Entries = DomainRegistry.Repository.Find(new GetOldXpLeaderboardEntries { Round = round }),
                Round = round
            };
            return View(MVC.Leaderboard.Views.OldXpLeaderboard, output);
        }

        public virtual ActionResult OldItemLeaderboard(int round)
        {
            var output = new OldItemLeaderboardViewModel
            {
                Entries = DomainRegistry.Repository.Find(new GetOldItemLeaderboardEntries { Round = round }),
                Round = round
            };
            return View(MVC.Leaderboard.Views.OldItemLeaderboard, output);
        }

    }
}