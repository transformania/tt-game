using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TT.Domain;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Commands;
using TT.Domain.Identity.DTOs;
using TT.Domain.Identity.Queries;
using TT.Domain.Messages.DTOs;
using TT.Domain.Messages.Queries;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Queries;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Web.Controllers
{
    [Authorize(Roles = PvPStatics.Permissions_Moderator + "," + PvPStatics.Permissions_Admin)]
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

        public virtual ActionResult LockPvP(SuspendTimeoutViewModel suspendTimeoutViewModel)
        {

            try
            {
                DomainRegistry.Repository.Execute(new SetPvPLock
                {
                    UserId = suspendTimeoutViewModel.UserId,
                    LockoutMessage = suspendTimeoutViewModel.lockoutMessage,
                    PvPLock = suspendTimeoutViewModel.isPvPLocked
                });
                TempData["Result"] = $"PvP lock has been successfully set to {suspendTimeoutViewModel.isPvPLocked}.";
            }
            catch (DomainException)
            {
                TempData["Error"] = "Failed to change PvP lock enabled/disabled.";
            }

            return RedirectToAction(MVC.PvP.Play());

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
            if (userId == null)
            {
                TempData["Error"] = "You cannot set a lock on this user";
                return RedirectToAction(MVC.PvP.Play());
            }

            var player = PlayerProcedures.GetPlayerFromMembership(userId);
            var user = DomainRegistry.Repository.FindSingle(new GetUserFromMembershipId { UserId = userId });
            bool isPvPLocked = DomainRegistry.Repository.FindSingle(new IsPvPLocked { UserId = userId });
            bool isAccountLocked = DomainRegistry.Repository.FindSingle(new IsAccountLockedOut { userId = userId });

            return View(MVC.Moderator.Views.SetAccountLockoutDate, new SuspendTimeoutViewModel{User = user, Player = player, UserId = player.MembershipId, PlayerId = player.Id, isPvPLocked = isPvPLocked, isAccountLocked = isAccountLocked, date = DateTime.UtcNow.AddYears(99)});
        }

        [ValidateAntiForgeryToken]
        public virtual ActionResult SetAccountLockoutDateSend(SuspendTimeoutViewModel suspendTimeoutViewModel)
        {
            try
            {
                TempData["Result"] = DomainRegistry.Repository.Execute(new SetLockoutTimestamp { UserId = suspendTimeoutViewModel.UserId, date = suspendTimeoutViewModel.date, message = suspendTimeoutViewModel.lockoutMessage });
                return RedirectToAction(MVC.PvP.Play());
            }
            catch (DomainException e)
            {
                TempData["Error"] = "Error: " + e.Message;
                return RedirectToAction(MVC.PvP.Play());
            }

        }

        /// <summary>
        /// List all of the News posts available for admins to edit
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult ListNewsPosts()
        {
            INewsPostRepository repo = new EFNewsPostRepository();
            var output = repo.NewsPosts.OrderByDescending(a => a.Timestamp);

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            return View(MVC.Moderator.Views.ListNewsPosts, output);
        }

        /// <summary>
        /// Make changes to an existing or new NewsPost
        /// </summary>
        /// <param name="Id">Id of the news post to make changes to.  -1 indicates a new post.</param>
        /// <returns></returns>
        public virtual ActionResult EditNewsPost(int id = -1)
        {
            INewsPostRepository repo = new EFNewsPostRepository();
            var output = repo.NewsPosts.FirstOrDefault(f => f.Id == id) ?? new NewsPost();

            return View(MVC.Moderator.Views.EditNewsPost, output);

        }

        /// <summary>
        /// Submit a NewsPost for revisions.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public virtual ActionResult EditNewsPostSend(NewsPost input)
        {
            INewsPostRepository repo = new EFNewsPostRepository();
            var saveMe = repo.NewsPosts.FirstOrDefault(f => f.Id == input.Id) ?? new NewsPost();

            saveMe.Timestamp = input.Timestamp;
            saveMe.Text = input.Text;
            saveMe.ViewState = input.ViewState;
            repo.SaveNewsPost(saveMe);

            TempData["Result"] = "News Post " + input.Id + " saved successfully!";
            return RedirectToAction(MVC.Moderator.ListNewsPosts());

        }

        /// <summary>
        /// Deletes a news post
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public virtual ActionResult DeleteNewsPost(int Id)
        {
            INewsPostRepository repo = new EFNewsPostRepository();
            repo.DeleteNewsPost(Id);

            TempData["Result"] = "News Post " + Id + " deleted successfully!";
            return RedirectToAction(MVC.Moderator.ListNewsPosts());
        }

        [Authorize]
        public virtual ActionResult RevertToBase(int Id)
        {
            // Get Moderator info.
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            var checkModerator = User.IsInRole(PvPStatics.Permissions_Moderator);
            if (!checkModerator)
            {
                TempData["Result"] = "You do not have permission to do that.";
                return RedirectToAction(MVC.PvP.LookAtPlayer(Id));
            }

            // Get the player info needed
            var playerMembershipId = PlayerProcedures.GetPlayer(Id);

            IPlayerRepository playerRepo = new EFPlayerRepository();

            var player = playerRepo.Players.FirstOrDefault(p => p.MembershipId == playerMembershipId.MembershipId);

            if (player.Mobility != PvPStatics.MobilityFull)
            {
                TempData["Result"] = "Only animate players can be force reverted to base form.";
                return RedirectToAction(MVC.PvP.LookAtPlayer(Id));
            }

            // Check for empty field
            if (player.OriginalFirstName == null || player.OriginalLastName == null)
            {
                TempData["Result"] = "The player is missing their original first or last name. Please contact an admin to get this fixed.";
                return RedirectToAction(MVC.PvP.LookAtPlayer(Id));
            }

            // Revert the player to their original self.
            player.FirstName = player.OriginalFirstName;
            player.LastName = player.OriginalLastName;

            playerRepo.SavePlayer(player);

            DomainRegistry.Repository.Execute(new ChangeForm
            {
                PlayerId = Id,
                FormSourceId = player.OriginalFormSourceId
            });

            PlayerLogProcedures.AddPlayerLog(me.Id,
                $"<b>You have reverted {player.OriginalFirstName} {player.OriginalLastName} back to their starting identity.</b>", true);
            return RedirectToAction(MVC.PvP.Play());

        }

        public virtual ActionResult ModReadConversation(int messageId, int reporterId)
        {

            //Assert the message was actually reported
            var reportedMessageList = DomainRegistry.Repository.Find(new GetMessagesReportedAbusive());
            var reportedMessage = reportedMessageList.FirstOrDefault(rm => rm.MessageId == messageId);

            if (reportedMessage == null)
            {
                TempData["Error"] = "This message was not reported and cannot be viewed.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var message = DomainRegistry.Repository.FindSingle(new GetMessageModerator { MessageId = messageId, OwnerId = reporterId });

            IEnumerable<MessageDetail> messages;

            try
            {
                messages =
                    DomainRegistry.Repository.Find(new GetMessagesInConversationModerator
                    {
                        conversationId = message.ConversationId
                    });

                return View(MVC.Messages.Views.ModReadConversation, messages);
            }
            catch
            {
                TempData["Error"] = "No conversation found";
                return RedirectToAction(MVC.PvP.Play());
            }


        }
    }
}
