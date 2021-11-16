using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TT.Domain.Models;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Web.Controllers
{
    [Authorize]
    public partial class CovenantController : Controller
    {

        public virtual ActionResult MyCovenant()
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            ViewBag.Player = me;

            CovenantViewModel output;

            output = CovenantProcedures.GetCovenantViewModel(me);
            if (output == null || me.Covenant == null || me.Covenant <= 0)
            {
                TempData["Error"] = "You are not currently in a covenant.";
                return RedirectToAction(MVC.Covenant.CovenantList());
            }

            ViewBag.LocationsControlled = CovenantProcedures.GetLocationControlCount(output.dbCovenant);

            ViewBag.MyMoney = Math.Floor(me.Money);

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            ViewBag.IAmCaptain = CovenantProcedures.PlayerIsCaptain(output.dbCovenant, me);

            return View(MVC.Covenant.Views.MyCovenant, output);
        }

        public virtual ActionResult CovenantList()
        {
            var output = CovenantProcedures.GetCovenantsList();

            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            ViewBag.Player = me;

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            ViewBag.HasApplication = CovenantProcedures.PlayerHasPendingApplication(me);

            return View(MVC.Covenant.Views.CovenantList, output);
        }

        public virtual ActionResult ApplyToCovenant(int id)
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            // assert that the covenant actually exists...
            var cov = CovenantProcedures.GetDbCovenant(id);

            if (cov == null)
            {
                TempData["Error"] = "That covenant does not exist.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player is not currently in a covenant
            if (me.Covenant > 0)
            {
                TempData["Error"] = "You must leave your covenant before you can apply to any new ones.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player doesn't already have a pending application
            if (CovenantProcedures.PlayerHasPendingApplication(me))
            {
                TempData["Error"] = "You already have an application to a covenant.";
                TempData["SubError"] = "In order to apply to a different covenant you must withdraw your old one first.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            CovenantProcedures.AddCovenantApplication(me, cov);

            TempData["Result"] = "Your application has been sent.";
            return RedirectToAction(MVC.Covenant.MyCovenant());
        }

        public virtual ActionResult ReviewMyCovenantApplications()
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            // assert that player is in a covenant
            if (me.Covenant == null || me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant and cannot accept or deny applications.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player is a covenant leader
            var myCov = CovenantProcedures.GetDbCovenant((int)me.Covenant);
            if (myCov.LeaderId != me.Id)
            {
                TempData["Error"] = "You are not the leader of this covenant.";
                TempData["SubError"] = "Only covenant leaders can accept or reject applications.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // validations are okay:  return the application list
            var output = CovenantProcedures.GetCovenantApplications(myCov);

            return View(MVC.Covenant.Views.ReviewMyCovenantApplications, output);
        }

        public virtual ActionResult ApplicationResponse(int id, string response)
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            // assert that player is in a covenant
            if (me.Covenant == null || me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant and cannot accept or deny applications.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player is a covenant leader
            var myCov = CovenantProcedures.GetDbCovenant((int)me.Covenant);
            if (myCov.LeaderId != me.Id)
            {
                TempData["Error"] = "You are not the leader of this covenant.";
                TempData["SubError"] = "Only covenant leaders can accept or reject applications.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the last acceptance wasn't too soon if it's past the first full day of the turn
            var minutesAgo = Math.Abs(Math.Floor(myCov.LastMemberAcceptance.Subtract(DateTime.UtcNow).TotalMinutes));
            if ((PvPWorldStatProcedures.GetWorldTurnNumber() > 144) && minutesAgo < 120)
            {

            }

            // get the application from the database
            var app = CovenantProcedures.GetCovenantApplication(id);
            var submitter = PlayerProcedures.GetPlayer(app.OwnerId);

            // assert that the application is for the same covenant
            if (app.CovenantId != myCov.Id)
            {
                TempData["Error"] = "This application is not for your covenant.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the covenant doesn't already have too many animate players
            if (response == "yes" && submitter.Mobility == PvPStatics.MobilityFull &&
                CovenantProcedures.GetPlayerCountInCovenant(myCov, true) >= PvPStatics.Covenant_MaximumAnimatePlayerCount)
            {
                TempData["Error"] = "The maximum number of animate players in your covenant has already been reached.";
                TempData["SubError"] = "You will not be able to accept this player's covenant request until there is room in the covent or they are no longer animate.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the applicant is not currently in a different covenant (e.g. of their own creation)
            if (submitter.Covenant != null || submitter.Covenant >0)
            {
                PlayerLogProcedures.AddPlayerLog(app.OwnerId, "<b style='color: red;'>Your application for " + myCov.Name + " covenant has been revoked as you are currently a member of a different covenant. Please leave your existing covenant before applying to a new one.</b>", true);
                TempData["Error"] = "This player is already in another covenant, thus the application was revoked.";

                CovenantProcedures.RevokeApplication(submitter);
                
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            if (response == "yes")
            {

                CovenantProcedures.AddPlayerToCovenant(app.OwnerId, myCov.Id);
                PlayerLogProcedures.AddPlayerLog(app.OwnerId, "<b style='color: blue;'>Congratulations, you have been accepted into the " + myCov.Name + " covenant.</b>", true);
                TempData["Result"] = "You have accepted the application.";

                CovenantProcedures.SetLastMemberJoinTimestamp(myCov);

            }
            else
            {
                PlayerLogProcedures.AddPlayerLog(app.OwnerId, "<b style='color: red;'>Unfortunately, you have not been accepted into the " + myCov.Name + " covenant.</b>", true);
                TempData["Result"] = "You have rejected the application.";
            }

            CovenantProcedures.RevokeApplication(submitter);

            return RedirectToAction(MVC.Covenant.MyCovenant());
        }

        public virtual ActionResult LeaveCovenant()
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            // assert player is indeed in a covenant
            if (me.Covenant <= 0)
            {
                TempData["Error"] = "You can't leave a covenant because you are not currently in one.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            CovenantProcedures.RemovePlayerFromCovenant(me);
            TempData["Result"] = "You have left your covenant.";

            return RedirectToAction(MVC.Covenant.MyCovenant());
        }


        public virtual ActionResult ChangeNoticeboardMessage()
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            // assert that player is in a covenant
            if (me.Covenant == null || me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant and cannot change the covenant's noticeboard.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player is a covenant leader or captain
            var myCov = CovenantProcedures.GetDbCovenant((int)me.Covenant);
            if (myCov.LeaderId != me.Id && !CovenantProcedures.PlayerIsCaptain(myCov, me))
            {                
                TempData["Error"] = "You are not authorized to change the notice.";
                TempData["SubError"] = "Only covenant leaders and captains can change a covenant's noticeboard.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            return View(MVC.Covenant.Views.ChangeNoticeboardMessage, myCov);
        }

        public virtual ActionResult ChangeNoticeboardMessageSubmit(Covenant input)
        {

            // assert model is okay
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Noticeboard was not updated";
                TempData["SubError"] = "Maximum message length is 200 characters.";
                return ChangeNoticeboardMessage();
            }

            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            // assert that player is in a covenant
            if (me.Covenant == null || me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant and cannot change the covenant's noticeboard.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player is a covenant leader or captain
            var myCov = CovenantProcedures.GetDbCovenant((int)me.Covenant);
            if (myCov.LeaderId != me.Id && !CovenantProcedures.PlayerIsCaptain(myCov, me))
            {
                TempData["Error"] = "You are not authorized to change the notice.";
                TempData["SubError"] = "Only covenant leaders and captains can change a covenant's noticeboard.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // finally update the covenant
            CovenantProcedures.ChangeNoticeboardMessage(myCov.Id, input.NoticeboardMessage);
            TempData["Result"] = "Noticeboard updated.";


            return RedirectToAction(MVC.Covenant.MyCovenant());
        }


        public virtual ActionResult ChangeCovenantDescription()
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            // assert that player is in a covenant
            if (me.Covenant == null || me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant and cannot change the covenant's self description.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player is a covenant leader
            var myCov = CovenantProcedures.GetDbCovenant((int)me.Covenant);
            if (myCov.LeaderId != me.Id)
            {
                TempData["Error"] = "You are not the leader of this covenant.";
                TempData["SubError"] = "Only covenant leaders can change a covenant's self description.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            var flagURLs = new List<string>();

            var path = $"{AppDomain.CurrentDomain.BaseDirectory}{PvPStatics.ImageFolder}CovenantFlags/";
            var d = new DirectoryInfo(path);//Assuming Test is your Folder
            var Files = d.GetFiles("*.png"); //Getting Text files
            foreach (var file in Files)
            {
                flagURLs.Add(file.Name);
            }

            flagURLs = CovenantProcedures.FilterAvailableFlags(flagURLs);

            ViewBag.FlagURLS = flagURLs;

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            return View(MVC.Covenant.Views.ChangeCovenantDescription, myCov);
        }

        public virtual ActionResult ChangeCovenantDescriptionSubmit(Covenant input)
        {

            // assert model is okay
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Your description was not saved";
                TempData["SubError"] = "Covenant description must be between 25 and 200 characters long.";
                return ChangeCovenantDescription();
            }

            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            // assert that player is in a covenant
            if (me.Covenant == null || me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant and cannot change the covenant's self description.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player is a covenant leader
            var myCov = CovenantProcedures.GetDbCovenant((int)me.Covenant);
            if (myCov.LeaderId != me.Id)
            {
                TempData["Error"] = "You are not the leader of this covenant.";
                TempData["SubError"] = "Only covenant leaders can change a covenant's self description.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the flag is not taken
            if (myCov.FlagUrl != input.FlagUrl && CovenantProcedures.FlagIsInUse(input.FlagUrl))
            {
                TempData["Error"] = "That flag is already in use.";
                TempData["SubError"] = "Select a different one.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            var path = $"{AppDomain.CurrentDomain.BaseDirectory}{PvPStatics.ImageFolder}CovenantFlags/{input.FlagUrl}";
            if (!System.IO.File.Exists(path))
            {
                TempData["Error"] = "Flag not found.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // finally update the covenant
            CovenantProcedures.UpdateCovenantDescription(myCov.Id, input.SelfDescription, input.FlagUrl);
            TempData["Result"] = "Description updated.";


            return RedirectToAction(MVC.Covenant.MyCovenant());
        }

        public virtual ActionResult StartNewCovenant()
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            // assert that this player is not currently in a covenant
            if (me.Covenant > 0)
            {
                TempData["Error"] = "You must leave your current covenant before you can found a new one.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player does not have pending coven applications
            if (CovenantProcedures.PlayerHasPendingApplication(me))
            {
                TempData["Error"] = "You have a pending application to a covenant.";
                TempData["SubError"] = "Please withdraw your covenant application before creating a new covenant.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            var output = new Covenant();

            ViewBag.Player = me;

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            return View(MVC.Covenant.Views.StartNewCovenant, output);
        }

        public virtual ActionResult StartNewCovenantSubmit(Covenant input)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            if (!ModelState.IsValid)
            {
                ViewBag.Player = me;

                TempData["Error"] = "Your covenant was not created.";
                TempData["SubError"] = "Covenant name must be between 8 and 50 characters long and description must be between 25 and 200 characters long.";
                return View(MVC.Covenant.Views.StartNewCovenant);
            }

            // assert that a covenant of this name does not already exists
            if (CovenantProcedures.CovenantOfNameExists(input.Name))
            {
                TempData["Error"] = "A covenant of that name already exists.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that this player is not currently in a covenant
            if (me.Covenant > 0)
            {
                TempData["Error"] = "You must leave your current covenant before you can found a new one.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            //input.FlagUrl = input.FlagUrl;
            input.FlagUrl = "generic.jpg";
            input.LeaderId = me.Id;
            //input.IsPvP = me.InPvP;
            CovenantProcedures.StartNewCovenant(input);

            // retrieve the recently made covenant's id so the player can be added to it
            var newcov = CovenantProcedures.GetDbCovenant(input.Name);
            CovenantProcedures.AddPlayerToCovenant(me, newcov.Id);
            CovenantProcedures.LoadCovenantDictionary();

            return RedirectToAction(MVC.Covenant.MyCovenant());
        }

        public virtual ActionResult LookAtCovenant(int id)
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            if (me.Covenant == id)
            {
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            var output = CovenantProcedures.GetCovenantViewModel(id);
            ViewBag.Player = me;
            ViewBag.LocationsControlled = CovenantProcedures.GetLocationControlCount(output.dbCovenant);
            return View(MVC.Covenant.Views.LookAtCovenant, output);
        }

        public virtual ActionResult WithdrawApplication()
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            TempData["Result"] = CovenantProcedures.RevokeApplication(me);
            return RedirectToAction(MVC.Covenant.MyCovenant());
        }

        public virtual ActionResult CovenantLeaderAdmin()
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            // assert that player is in a covenant
            if (me.Covenant == null || me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant and cannot accept or deny applications.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player is a covenant leader
            var myCov = CovenantProcedures.GetDbCovenant((int)me.Covenant);
            if (myCov.LeaderId != me.Id && !CovenantProcedures.PlayerIsCaptain(myCov, me))
            {
                TempData["Error"] = "You are not the leader of this covenant.";
                TempData["SubError"] = "Only covenant leaders can accept or reject applications.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            ViewBag.UpgradeCost = CovenantProcedures.GetUpgradeCost(myCov).ToString();


            if (myCov.LeaderId == me.Id)
            {
                ViewBag.LeaderOrCaptain = "leader";
            }
            else if (CovenantProcedures.PlayerIsCaptain(myCov, me))
            {
                ViewBag.LeaderOrCaptain = "captain";
            }


            return View(MVC.Covenant.Views.CovenantLeaderAdmin);
        }

        public virtual ActionResult KickList()
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            // assert that player is in a covenant
            if (me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant and cannot kick members.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player is a covenant leader
            var output = CovenantProcedures.GetCovenantViewModel(me);
            if (output.dbCovenant.LeaderId != me.Id)
            {
                TempData["Error"] = "You are not the leader of this covenant and cannot kick members.";
                TempData["SubError"] = "Only covenant leaders can kick out players.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // remove the player's own name from the list
            output.Members = output.Members.Where(p => p.Player.Id != me.Id);

            return View(MVC.Covenant.Views.KickList, output);

        }

        public virtual ActionResult KickMember(int id)
        {

            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            // assert that player is in a covenant
            if (me.Covenant == null || me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant and cannot accept or deny applications.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player is a covenant leader
            var myCov = CovenantProcedures.GetDbCovenant((int)me.Covenant);
            if (myCov.LeaderId != me.Id)
            {
                TempData["Error"] = "You are not the leader of this covenant.";
                TempData["SubError"] = "Only covenant leaders can accept or reject applications.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player being kicked does belong to the covenant
            var beingKicked = PlayerProcedures.GetPlayer(id);

            if (beingKicked.Covenant != myCov.Id)
            {
                TempData["Error"] = "This player is not in your covenant.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player is not kicking themself out
            if (beingKicked.Id == me.Id)
            {
                TempData["Error"] = "You cannot kick yourself out of a covenant since you are its leader.";
                TempData["SubError"] = "You must leave the covenant by your own will.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            CovenantProcedures.RemovePlayerFromCovenant(beingKicked);
            var message = "<b>" + me.GetFullName() + " has kicked you out of " + myCov.Name + ".</b>";
            PlayerLogProcedures.AddPlayerLog(beingKicked.Id, message, true);

            TempData["Result"] = "You have kicked " + beingKicked.GetFullName() + " out of your covenant.";
            return RedirectToAction(MVC.Covenant.MyCovenant());
        }

        public virtual ActionResult AddToCovenantChest(int amount)
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            // assert that player is in a covenant
            if (me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant and cannot add money to the covenant's chest.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player has enough money for this
            if (me.Money < amount)
            {
                TempData["Error"] = "You do not have this many Arpeyjis to send to your covenant.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player has not been in recent combat
            var minutesAgo = Math.Abs(Math.Floor(me.GetLastCombatTimestamp().Subtract(DateTime.UtcNow).TotalMinutes));
            if (minutesAgo < TurnTimesStatics.GetMinutesSinceLastCombatBeforeQuestingOrDuelling())
            {
                TempData["Error"] = "You must wait another " + (TurnTimesStatics.GetMinutesSinceLastCombatBeforeQuestingOrDuelling() - minutesAgo) + " minutes without being in combat in order to do this.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the amount is valid
            if (amount != 20 && amount != 100 && amount != 500)
            {
                TempData["Error"] = "That is not a valid amount of Arpeyjis to send.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            CovenantProcedures.SendPlayerMoneyToCovenant(me, amount);

            TempData["Result"] = "You have successfully sent " + amount + " Arpeyjis to your covenant.";

            StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__CovenantNetDonation, amount);

            return RedirectToAction(MVC.Covenant.MyCovenant());

        }

        public virtual ActionResult GiveMoneyFromCovenantChest(int id, decimal amount)
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            // assert that player is in a covenant
            if (me.Covenant == null || me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player is a covenant leader or captain
            var myCov = CovenantProcedures.GetDbCovenant((int)me.Covenant);
            if (myCov.LeaderId != me.Id && !CovenantProcedures.PlayerIsCaptain(myCov, me))
            {
                TempData["Error"] = "You are not the leader or a captain of your covenant.";
                TempData["SubError"] = "Only covenant leaders and captains can gift out money from the covenant's Arpeyjis chest.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the covenant does have this much to send
            if (myCov.Money < amount)
            {
                TempData["Error"] = "Your covenant does not have that many Arpeyjis to send.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the amount is valid
            if (amount != 20 && amount != 100 && amount != 500)
            {
                TempData["Error"] = "That is not a valid amount of Arpeyjis to send.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the giftee is in the same covenant
            var giftee = PlayerProcedures.GetPlayer(id);
            if (giftee.Covenant != me.Covenant)
            {
                TempData["Error"] = "This player is not in your covenant.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the covenant is sufficiently large
            if (PlayerProcedures.GetAnimatePlayerCountInCovenant((int)me.Covenant) < 3)
            {
                TempData["Error"] = "In order to gift out Arpeyjis to members of your covenant, you must have at least 3 animate members.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player will not go over the money limit
            if (giftee.WillGoOverMoneyLimitIfPaid(amount))
            {
                TempData["Error"] = "This player has too much money, you cannot give them any more.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }
            CovenantProcedures.SendCovenantMoneyToPlayer((int)me.Covenant, giftee, amount);


            StatsProcedures.AddStat(giftee.MembershipId, StatsProcedures.Stat__CovenantNetDonation, (float) -amount);

            TempData["Result"] = "You have successfully sent " + amount + " Arpeyjis to " + giftee.GetFullName() + ".";
            return RedirectToAction(MVC.Covenant.MyCovenant());

        }

        public virtual ActionResult ClaimLocation()
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());


            // assert that player is in a covenant
            if (me.Covenant == null || me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player is a covenant leader
            var myCov = CovenantProcedures.GetDbCovenant((int)me.Covenant);
            if (myCov.LeaderId != me.Id)
            {
                TempData["Error"] = "You are not the leader of your covenant.";
                TempData["SubError"] = "Only covenant leaders can gift out money from the covenant's Arpeyjis chest.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player is animate
            if (me.Mobility != PvPStatics.MobilityFull)
            {
                TempData["Error"] = "You must be animate in order to do this.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            ViewBag.MyLocation = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == me.dbLocationName)?.Name ?? "unknown";
            ViewBag.CovenantMoney = myCov.Money;

            return View(MVC.Covenant.Views.ClaimLocation);

        }

        public virtual ActionResult ClaimLocationSend()
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            // assert that the player is animate
            if (me.Mobility != PvPStatics.MobilityFull)
            {
                TempData["Error"] = "You must be animate in order to do this.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that player is in a covenant
            if (me.Covenant == null || me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player is a covenant leader
            var myCov = CovenantProcedures.GetDbCovenant((int)me.Covenant);

            if (myCov.LeaderId != me.Id)
            {
                TempData["Error"] = "You are not the leader of your covenant.";
                TempData["SubError"] = "Only covenant leaders can gift out money from the covenant's Arpeyjis chest.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the location is not in the dungeon
            if (me.dbLocationName.Contains("dungeon_"))
            {
                TempData["Error"] = "You cannot establish a safeground in the dungeon.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            if (me.dbLocationName == LocationsStatics.JOKE_SHOP)
            {
                TempData["Error"] = "This otherworldly realm is immune to your enchantments and you find yourself unable to establish a safeground in the Joke Shop.";
                TempData["SubError"] = "Try establishing your safeground elsewhere.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the covenant does not already have a safeground set
            if (CovenantProcedures.CovenantHasSafeground(myCov))
            {
                TempData["Error"] =
                    $"Your covenant already has a safeground at {LocationsStatics.LocationList.GetLocation.FirstOrDefault(f => f.dbName == myCov.HomeLocation)?.Name ?? "unknown"}.";
                TempData["SubError"] = "Covenants can only establish a safeground once per round.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }


            // assert that the covenant has enough money
            if (myCov.Money < 2500)
            {
                TempData["Error"] = "Your covenant cannot afford that right now.";
                TempData["SubError"] = "Try asking your members to donate more to the Covenant Chest.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that this location is not in the streets
            if (me.dbLocationName.Contains("street_"))
            {
                TempData["Error"] = "You cannot establish a covenant safeground on a path or street.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // asset that this location is not already in use by another covenant
            if (CovenantProcedures.ACovenantHasASafegroundHere(me.dbLocationName))
            {
                TempData["Error"] = "Your covenant cannot establish a safeground here.";
                TempData["SubError"] = "Another covenant has already established a safeground at your current location.  You must find somewhere else.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player is animate
            if (me.Mobility != PvPStatics.MobilityFull)
            {
                TempData["Result"] = "You have succesfully claimed this location for your covenant safeground!";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the covenant has sufficient player count
            if (CovenantProcedures.GetPlayerCountInCovenant_Animate_Lvl3(myCov) < PvPStatics.Covenant_MinimumUpgradeAnimateLvl3PlayerCount)
            {
                TempData["Error"] = "Your covenant needs at least five animate players at level three or greater in order to do this.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // all checks are okay, so allow this covenant to establish a safeground here

            TempData["Result"] = "You have succesfully claimed this location for your covenant safeground!";
            CovenantProcedures.SetCovenantSafeground(myCov, me.dbLocationName);
            return RedirectToAction(MVC.Covenant.MyCovenant());

        }

        public virtual ActionResult UpgradeSafeground()
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            // assert that the player is animate
            if (me.Mobility != PvPStatics.MobilityFull)
            {
                TempData["Error"] = "You must be animate in order to do this.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that player is in a covenant
            if (me.Covenant == null || me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player is a covenant leader
            var myCov = CovenantProcedures.GetDbCovenant((int)me.Covenant);
            if (myCov.LeaderId != me.Id)
            {
                TempData["Error"] = "You are not the leader of your covenant.";
                TempData["SubError"] = "Only covenant leaders can gift out money from the covenant's Arpeyjis chest.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the covenant has a safeground set
            if (!CovenantProcedures.CovenantHasSafeground(myCov))
            {
                TempData["Error"] = "Your covenant must establish a safeground before it can upgrade it.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player is at the safeground
            if (me.dbLocationName != myCov.HomeLocation)
            {
                TempData["Error"] = "You must be at your covenant safeground in order to upgrade it.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the covenant has enough money
            if (myCov.Money < CovenantProcedures.GetUpgradeCost(myCov))
            {
                TempData["Error"] = "Your covenant cannot afford that right now.";
                TempData["SubError"] = "Try asking your members to donate more to the Covenant Treasury.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the covenant has sufficient player count
            if (CovenantProcedures.GetPlayerCountInCovenant_Animate_Lvl3(myCov) < PvPStatics.Covenant_MinimumUpgradeAnimateLvl3PlayerCount)
            {
                TempData["Error"] = "Your covenant needs at least five animate players at level three or greater in order to do this.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // all checks pass; upgrade the covenant
            CovenantProcedures.UpgradeCovenant(myCov);

            TempData["Result"] = "You have successfully upgraded your covenant safeground to lvl " + (myCov.Level + 1) + ".";
            return RedirectToAction(MVC.Covenant.MyCovenant());
        }

        public virtual ActionResult ViewAvailableFurniture()
        {

            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            // assert that player is in a covenant
            if (me.Covenant == null || me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            var output = FurnitureProcedures.GetAvailableFurnitureViewModels();

            var myCov = CovenantProcedures.GetDbCovenant((int)me.Covenant);
            if (myCov == null)
            {
                TempData["Error"] = "Error loading covenant information.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }
            var playerIsCovenantLeader = myCov.LeaderId == me.Id && me.Covenant > 0;
            ViewBag.playerIsCovenantLeader = playerIsCovenantLeader;
            ViewBag.CovenantMoney = (int)myCov.Money;

            ViewBag.FurnitureLimit = CovenantProcedures.GetCovenantFurnitureLimit(myCov);

            ViewBag.IAmCaptain = CovenantProcedures.PlayerIsCaptain(myCov, me);

            return View(MVC.Covenant.Views.ViewAvailableFurniture, output);
        }

        public virtual ActionResult PurchaseFurniture(int id)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert that player is in a covenant
            if (me.Covenant == null || me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player is a covenant leader or captain
            var myCov = CovenantProcedures.GetDbCovenant((int)me.Covenant);
            if (myCov.LeaderId != me.Id && !CovenantProcedures.PlayerIsCaptain(myCov, me))
            {
                TempData["Error"] = "You are not the leader of your covenant.";
                TempData["SubError"] = "Only covenant leaders can gift out money from the covenant's Arpeyjis chest.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            var furniture = FurnitureProcedures.GetdbFurniture(id);

            // assert that the covenant has enough money
            if (myCov.Money < furniture.Price)
            {
                TempData["Error"] = "Your covenant does not have enough Arpeyjis to purchase this contract.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the covenant has a safeground
            if (myCov.HomeLocation.IsNullOrEmpty())
            {
                TempData["Error"] = "Your covenant needs a safeground before it can purchase any furniture.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the furniture is on the market
            if (furniture.CovenantId != -1)
            {
                TempData["Error"] = "This piece of furniture is not currently on the market.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the covenant has room for this new furniture
            if (CovenantProcedures.GetCovenantFurnitureLimit(myCov) <= CovenantProcedures.GetCurrentFurnitureOwnedByCovenant(myCov))
            {
                TempData["Error"] = "Your safeground already has too much furniture and cannot fit any more.";
                TempData["SubError"] = "Your covenant leader must upgrade the safeground in order to fit more furniture or else wait for some furnitures' contracts to expire.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // all checks have passed; give the furniture to the covenant

            FurnitureProcedures.GiveFurnitureToCovenant(furniture, myCov);

            var result = "Congratulations, your covenant, " + myCov.Name + ", has successfully purchased the contract for " + furniture.HumanName + ".";
            TempData["Result"] = result;

            ViewBag.FurnitureLimit = CovenantProcedures.GetCovenantFurnitureLimit(myCov);


            return RedirectToAction(MVC.Covenant.MyCovenant());

        }

        public virtual ActionResult MyCovenantFurniture()
        {

            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            // assert that player is in a covenant
            if (me.Covenant == null || me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }
            var myCov = CovenantProcedures.GetDbCovenant((int)me.Covenant);

            var furniture = FurnitureProcedures.GetCovenantFurnitureViewModels((int)me.Covenant);

            ViewBag.MyLocation = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == me.dbLocationName)?.Name ?? "unknown";

            var covSafegroundLocation = myCov.HomeLocation.IsNullOrEmpty()
                ? "Your covenant has not yet established a safeground and cannot yet lease any furniture."
                : $"Your covenant\'s safeground is at {LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == myCov.HomeLocation)?.Name ?? "unknown"}";

            var playerIsAtSafeground = !myCov.HomeLocation.IsNullOrEmpty() && me.dbLocationName == myCov.HomeLocation;

            var output = new UseFurnitureViewModel
            {
                CovenantSafeground = covSafegroundLocation,
                FurnitureLimit = CovenantProcedures.GetCovenantFurnitureLimit(myCov),
                Furniture = furniture.ToList(),
                AtCovenantSafeground = playerIsAtSafeground
            };

            output.Furniture.ForEach(f => f.MyUserId = me.Id);

            return View(MVC.Covenant.Views.MyCovenantFurniture, output);

        }

        public virtual ActionResult UseFurniture(int id)
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            if (me.Covenant == null || me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            var myCov = CovenantProcedures.GetDbCovenant((int)me.Covenant);
            var furniture = FurnitureProcedures.GetdbFurniture(id);

            // assert that the player is animate
            if (me.Mobility != PvPStatics.MobilityFull)
            {
                TempData["Error"] = "You must be animate in order to do this.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that this player is not in a duel
            if (me.InDuel > 0)
            {
                TempData["Error"] = "You must finish your duel before you use covenant furniture.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that this player is not in a quest
            if (me.InQuest > 0)
            {
                TempData["Error"] = "You must finish your quest before you use covenant furniture.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that player is in a covenant
            if (me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player is in the covenant that actually owns this
            if (furniture.CovenantId != me.Covenant)
            {
                TempData["Error"] = "Your covenant does not own this furniture.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player is at the covenant safegrounds
            if (myCov.HomeLocation != me.dbLocationName)
            {
                TempData["Error"] = "You must be at your covenant safeground to use your covenant's furniture.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the item is not on recharge
            if (FurnitureProcedures.GetMinutesUntilReuse(furniture) > 0)
            {
                TempData["Error"] = "This item of furniture needs more time to regather its energy before it can be used for any bonuses.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that this player was not the last person to use this furniture
            if (furniture.LastUsersIds == me.Id.ToString())
            {
                TempData["Error"] = "You were the last person to use this furniture!  Let someone else have a chance.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            TempData["Result"] = FurnitureProcedures.UseFurniture(id, me);

            StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__CovenantFurnitureUsed, 1);

            return RedirectToAction(MVC.Covenant.MyCovenant());

        }

        public virtual ActionResult MyCovenantLog()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert that player is in a covenant
            if (me.Covenant == null || me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            var output = CovenantProcedures.GetCovenantLogs((int)me.Covenant);

            return View(MVC.Covenant.Views.MyCovenantLog, output);
        }

        public virtual ActionResult InviteCaptainList()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert that player is in a covenant
            if (me.Covenant == null || me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player is a covenant leader
            var myCov = CovenantProcedures.GetDbCovenant((int)me.Covenant);
            if (myCov.LeaderId != me.Id)
            {
                TempData["Error"] = "You are not the leader of your covenant.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            var output = CovenantProcedures.GetCovenantViewModel((int)me.Covenant);

            ViewBag.CurrentCaptains = myCov.Captains;

            return View(MVC.Covenant.Views.InviteCaptainList, output);
        }

        public virtual ActionResult InviteCaptainSend(int id)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert that player is in a covenant
            if (me.Covenant == null || me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player is a covenant leader
            var myCov = CovenantProcedures.GetDbCovenant((int)me.Covenant);
            if (myCov.LeaderId != me.Id)
            {
                TempData["Error"] = "You are not the leader of your covenant.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            var newCaptain = PlayerProcedures.GetPlayer(id);

            // assert that the target player is in the same covenant
            if (newCaptain.Covenant != myCov.Id)
            {
                TempData["Error"] = "This player is not a member of your covenant and cannot be made a captain.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the target is not the covenant leader
            if (newCaptain.Id == myCov.LeaderId)
            {
                TempData["Error"] = "This player is the leader of the covenant.";
                TempData["SubError"] = "There is nothing a covenant captain can do that a leader cannot.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            var result = CovenantProcedures.ChangeCovenantCaptain(myCov, newCaptain);

            TempData["Result"] = result;
            return RedirectToAction(MVC.Covenant.MyCovenant());
        }

        public virtual ActionResult InviteLeaderList()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert that player is in a covenant
            if (me.Covenant == null || me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player is a covenant leader
            var myCov = CovenantProcedures.GetDbCovenant((int)me.Covenant);
            if (myCov.LeaderId != me.Id)
            {
                TempData["Error"] = "You are not the leader of your covenant.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            var output = CovenantProcedures.GetCovenantViewModel((int)me.Covenant);
            return View(MVC.Covenant.Views.InviteLeaderList, output);
        }


        public virtual ActionResult InviteLeaderSend(int id)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert that player is in a covenant
            if (me.Covenant == null || me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the player is a covenant leader
            var myCov = CovenantProcedures.GetDbCovenant((int)me.Covenant);
            if (myCov.LeaderId != me.Id)
            {
                TempData["Error"] = "You are not the leader of your covenant.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            var newLeader = PlayerProcedures.GetPlayer(id);

            // assert that the target player is in the same covenant
            if (newLeader.Covenant != myCov.Id)
            {
                TempData["Error"] = "This player is not a member of your covenant and cannot be made the leader.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }

            // assert that the target is not the covenant leader
            if (newLeader.Id == myCov.LeaderId)
            {
                TempData["Error"] = "This player is already the leader of the covenant.";
                return RedirectToAction(MVC.Covenant.MyCovenant());
            }
            var result = CovenantProcedures.ChangeCovenantLeader(myCov, newLeader);

            TempData["Result"] = result;
            return RedirectToAction(MVC.Covenant.MyCovenant());

        }
    }
}