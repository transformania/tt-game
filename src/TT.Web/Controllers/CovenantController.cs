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
    public class CovenantController : Controller
    {
        //
        // GET: /Covenant/

        [Authorize]
        public ActionResult MyCovenant()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            ViewBag.Player = me;

            CovenantViewModel output;

            output = CovenantProcedures.GetCovenantViewModel(me);
            if (output == null)
            {
                output = new CovenantViewModel();
                ViewBag.LocationsControlled = 0;
            }
            else
            {
                ViewBag.LocationsControlled = CovenantProcedures.GetLocationControlCount(output.dbCovenant);
            }

            ViewBag.MyMoney = Math.Floor(me.Money);

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            ViewBag.IAmCaptain = CovenantProcedures.PlayerIsCaptain(output.dbCovenant, me);
           
            ViewBag.HasApplication = CovenantProcedures.PlayerHasPendingApplication(me);

            

            return View(output);
        }

        [Authorize]
        public ActionResult CovenantList()
        {
            IEnumerable<CovenantListItemViewModel> output = CovenantProcedures.GetCovenantsList();
            return View(output);
        }

        [Authorize]
        public ActionResult ApplyToCovenant(int id)
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            // assert that the covenant actually exists...
            Covenant cov = CovenantProcedures.GetDbCovenant(id);

            if (cov == null)
            {
                TempData["Error"] = "That covenant does not exist.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the player is not currently in a covenant
            if (me.Covenant > 0)
            {
                TempData["Error"] = "You must leave your covenant before you can apply to any new ones.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the covenant matches the player's PvP mode
            //if ((me.InPvP && !cov.IsPvP) || (!me.InPvP && cov.IsPvP)) {
            //    TempData["Error"] = "Only PvP players may join PvP covenants and only non-PvP players can join non-PvP covenants.";
            //    return RedirectToAction("MyCovenant");
            //}

            // assert that the player doesn't already have a pending application
            if (CovenantProcedures.PlayerHasPendingApplication(me))
            {
                TempData["Error"] = "You already have an application to a covenant.";
                TempData["SubError"] = "In order to apply to a different covenant you must withdraw your old one first.";
                return RedirectToAction("MyCovenant");
            }

            CovenantProcedures.AddCovenantApplication(me, cov);

            TempData["Result"] = "Your application has been sent.";
            return RedirectToAction("MyCovenant");
        }

        [Authorize]
        public ActionResult ReviewMyCovenantApplications()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            // assert that player is in a covenant
            if (me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant and cannot accept or deny applications.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the player is a covenant leader
            Covenant myCov = CovenantProcedures.GetDbCovenant(me.Covenant);
            if (myCov.LeaderId != me.Id)
            {
                TempData["Error"] = "You are not the leader of this covenant.";
                TempData["SubError"] = "Only covenant leaders can accept or reject applications.";
                return RedirectToAction("MyCovenant");
            }

            // validations are okay:  return the application list
            IEnumerable<CovenantApplicationViewModel> output = CovenantProcedures.GetCovenantApplications(myCov);

            return View(output);
        }

        [Authorize]
        public ActionResult ApplicationResponse(int id, string response)
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            // assert that player is in a covenant
            if (me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant and cannot accept or deny applications.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the player is a covenant leader
            Covenant myCov = CovenantProcedures.GetDbCovenant(me.Covenant);
            if (myCov.LeaderId != me.Id)
            {
                TempData["Error"] = "You are not the leader of this covenant.";
                TempData["SubError"] = "Only covenant leaders can accept or reject applications.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the covenant doesn't already have too many animate players
            if (response == "yes" && CovenantProcedures.GetPlayerCountInCovenant(myCov, true) >= PvPStatics.Covenant_MaximumAnimatePlayerCount)
            {
                TempData["Error"] = "The maximum number of animate players in your covenant has already been reached.";
                TempData["SubError"] = "You will not be able to accept this player's covenant request until there is room in the covent or they are no longer animate.";
                return RedirectToAction("MyCovenant");
            }

           // assert that the last acceptance wasn't too soon if it's past the first full day of the turn
            double minutesAgo = Math.Abs(Math.Floor(myCov.LastMemberAcceptance.Subtract(DateTime.UtcNow).TotalMinutes));
            if ((PvPWorldStatProcedures.GetWorldTurnNumber() > 144) && minutesAgo < 120)
            {

            }

            // get the application from the database
            CovenantApplication app = CovenantProcedures.GetCovenantApplication(id);

            // assert that the application is for the same covenant
            if (app.CovenantId!= myCov.Id) {
                TempData["Error"] = "This application is not for your covenant.";
                return RedirectToAction("MyCovenant");
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


            Player submitter = PlayerProcedures.GetPlayer(app.OwnerId);
            CovenantProcedures.RevokeApplication(submitter);

            return RedirectToAction("MyCovenant");
        }

        [Authorize]
        public ActionResult LeaveCovenant()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            // assert player is indeed in a covenant
            if (me.Covenant <= 0)
            {
                TempData["Error"] = "You can't leave a covenant because you are not currently in one.";
                return RedirectToAction("MyCovenant");
            }

            CovenantProcedures.RemovePlayerFromCovenant(me);
            TempData["Result"] = "You have left your covenant.";

            return RedirectToAction("MyCovenant");
        }

        [Authorize]
        public ActionResult DisbandCovenant()
        {
            return View();
        }

        [Authorize]
        public ActionResult ChangeCovenantDescription()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            // assert that player is in a covenant
            if (me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant and change the covenant's self description.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the player is a covenant leader
            Covenant myCov = CovenantProcedures.GetDbCovenant(me.Covenant);
            if (myCov.LeaderId != me.Id)
            {
                TempData["Error"] = "You are not the leader of this covenant.";
                TempData["SubError"] = "Only covenant leaders can change a covenant's self description.";
                return RedirectToAction("MyCovenant");
            }

            List<string> flagURLs = new List<string>();

            string path = Server.MapPath("~/Images/PvP/CovenantFlags/");
            DirectoryInfo d = new DirectoryInfo(path);//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.jpg"); //Getting Text files
            foreach (FileInfo file in Files)
            {
                flagURLs.Add(file.Name);
            }

            flagURLs = CovenantProcedures.FilterAvailableFlags(flagURLs);

            ViewBag.FlagURLS = flagURLs;

            return View(myCov);
        }

        [Authorize]
        public ActionResult ChangeCovenantDescriptionSubmit(Covenant input)
        {

            // assert model is okay
            if (!ModelState.IsValid)
            {
                ViewBag.ValidationMessage = "Your description was not saved.  Covenant description must be between 25 and 200 characters long.";
                return View("ChangeCovenantDescription");
            }

            Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            // assert that player is in a covenant
            if (me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant and change the covenant's self description.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the player is a covenant leader
            Covenant myCov = CovenantProcedures.GetDbCovenant(me.Covenant);
            if (myCov.LeaderId != me.Id)
            {
                TempData["Error"] = "You are not the leader of this covenant.";
                TempData["SubError"] = "Only covenant leaders can change a covenant's self description.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the flag is not taken
            if (CovenantProcedures.FlagIsInUse(input.FlagUrl))
            {
                TempData["Error"] = "That flag is already in use.";
                TempData["SubError"] = "Select a different one.";
                return RedirectToAction("MyCovenant");
            }

            string path = Server.MapPath("~/Images/PvP/CovenantFlags/" + input.FlagUrl);
            if (!System.IO.File.Exists(path))
            {
                TempData["Error"] = "Flag not found.";
                return RedirectToAction("MyCovenant");
            }

            // finally update the covenant
            CovenantProcedures.UpdateCovenantDescription(myCov.Id, input.SelfDescription, input.FlagUrl);
            TempData["Result"] = "Description updated.";


            return RedirectToAction("MyCovenant");
        }

        [Authorize]
        public ActionResult StartNewCovenant()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            // assert that this player is not currently in a covenant
            if (me.Covenant > 0)
            {
                TempData["Error"] = "You must leave your current covenant before you can found a new one.";
                return RedirectToAction("MyCovenant");
            }

            Covenant output = new Covenant();
            return View(output);
        }

        [Authorize]
        public ActionResult StartNewCovenantSubmit(Covenant input)
        {
            string myMembershipId = User.Identity.GetUserId();
            if (!ModelState.IsValid)
            {
                ViewBag.ValidationMessage = "Your covenant was not created.  Covenant name must be between 8 and 50 characters long and description must be between 25 and 200 characters long.";
                return View("StartNewCovenant");
            }

            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert that a covenant of this name does not already exists
            if (CovenantProcedures.CovenantOfNameExists(input.Name))
            {
                TempData["Error"] = "A covenant of that name already exists.";
                return RedirectToAction("MyCovenant");
            }

            // assert that this player is not currently in a covenant
            if (me.Covenant > 0)
            {
                TempData["Error"] = "You must leave your current covenant before you can found a new one.";
                return RedirectToAction("MyCovenant");
            }

            //input.FlagUrl = input.FlagUrl;
            input.FlagUrl = "generic.jpg";
            input.FounderMembershipId = myMembershipId;
            input.LeaderId = me.Id;
            //input.IsPvP = me.InPvP;
            CovenantProcedures.StartNewCovenant(input);

            // retrieve the recently made covenant's id so the player can be added to it
            Covenant newcov = CovenantProcedures.GetDbCovenant(input.Name);
            CovenantProcedures.AddPlayerToCovenant(me, newcov.Id);
            CovenantProcedures.LoadCovenantDictionary();

            return RedirectToAction("MyCovenant");
        }

        [Authorize]
        public ActionResult LookAtCovenant(int id)
        {
            CovenantViewModel output = CovenantProcedures.GetCovenantViewModel(id);
            ViewBag.LocationsControlled = CovenantProcedures.GetLocationControlCount(output.dbCovenant);
            return View(output);
        }

        [Authorize]
        public ActionResult WithdrawApplication()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            TempData["Result"] = CovenantProcedures.RevokeApplication(me);
            return RedirectToAction("MyCovenant");
        }

        [Authorize]
        public ActionResult CovenantLeaderAdmin()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            // assert that player is in a covenant
            if (me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant and cannot accept or deny applications.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the player is a covenant leader
            Covenant myCov = CovenantProcedures.GetDbCovenant(me.Covenant);
            if (myCov.LeaderId != me.Id && !CovenantProcedures.PlayerIsCaptain(myCov, me))
            {
                TempData["Error"] = "You are not the leader of this covenant.";
                TempData["SubError"] = "Only covenant leaders can accept or reject applications.";
                return RedirectToAction("MyCovenant");
            }

            ViewBag.UpgradeCost = CovenantProcedures.GetUpgradeCost(myCov).ToString();


            if (myCov.LeaderId == me.Id) {
                ViewBag.LeaderOrCaptain = "leader";
            } else if (CovenantProcedures.PlayerIsCaptain(myCov, me)) {
                ViewBag.LeaderOrCaptain = "captain";
            }


            return View();
        }

        [Authorize]
        public ActionResult KickList()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            // assert that player is in a covenant
            if (me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant and cannot kick members.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the player is a covenant leader
            CovenantViewModel output = CovenantProcedures.GetCovenantViewModel(me);
            if (output.dbCovenant.LeaderId != me.Id)
            {
                TempData["Error"] = "You are not the leader of this covenant and cannot kick members.";
                TempData["SubError"] = "Only covenant leaders can kick out players.";
                return RedirectToAction("MyCovenant");
            }

            // remove the player's own name from the list
            output.Members = output.Members.Where(p => p.Player.Id != me.Id);

            return View(output);

        }

        [Authorize]
        public ActionResult KickMember(int id)
        {

            Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            // assert that player is in a covenant
            if (me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant and cannot accept or deny applications.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the player is a covenant leader
            Covenant myCov = CovenantProcedures.GetDbCovenant(me.Covenant);
            if (myCov.LeaderId != me.Id)
            {
                TempData["Error"] = "You are not the leader of this covenant.";
                TempData["SubError"] = "Only covenant leaders can accept or reject applications.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the player being kicked does belong to the covenant
            Player beingKicked = PlayerProcedures.GetPlayer(id);

            if (beingKicked.Covenant != myCov.Id)
            {
                TempData["Error"] = "This player is not in your covenant.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the player is not kicking themself out
            if (beingKicked.Id == me.Id)
            {
                TempData["Error"] = "You cannot kick yourself out of a covenant since you are its leader.";
                TempData["SubError"] = "You must leave the covenant by your own will.";
                return RedirectToAction("MyCovenant");
            }

            CovenantProcedures.RemovePlayerFromCovenant(beingKicked);
            string message = "<b>" + me.GetFullName() + " has kicked you out of " + myCov.Name + ".</b>";
            PlayerLogProcedures.AddPlayerLog(beingKicked.Id, message, true);

            TempData["Result"] = "You have kicked " + beingKicked.GetFullName() + " out of your covenant.";
            return RedirectToAction("MyCovenant");
        }

         [Authorize]
        public ActionResult AddToCovenantChest(int amount)
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            // assert that player is in a covenant
            if (me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant and cannot add money to the covenant's chest.";
                return RedirectToAction("MyCovenant");
            }

             // assert that the player has enough money for this
            if (me.Money < amount)
            {
                TempData["Error"] = "You do not have this many Arpeyjis to send to your covenant.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the player has not been in recent combat
            double minutesAgo = Math.Abs(Math.Floor(me.GetLastCombatTimestamp().Subtract(DateTime.UtcNow).TotalMinutes));
            if (minutesAgo < 30)
            {
                TempData["Error"] = "You must wait another " + (30 - minutesAgo) + " minutes without being in combat in order to do this.";
                return RedirectToAction("MyCovenant");
            }

             // assert that the amount is valid
             if (amount != 20 && amount != 100 && amount!= 500) {
                 TempData["Error"] = "That is not a valid amount of Arpeyjis to send.";
                 return RedirectToAction("MyCovenant");
             }

            CovenantProcedures.SendPlayerMoneyToCovenant(me, amount);

            TempData["Result"] = "You have successfully sent " + amount + " Arpeyjis to your covenant.";

            new Thread(() =>
                 StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__CovenantNetDonation, amount)
             ).Start();

            return RedirectToAction("MyCovenant");

        }

         [Authorize]
         public ActionResult GiveMoneyFromCovenantChest(int id, decimal amount)
         {
             Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
             
             // assert that player is in a covenant
             if (me.Covenant <= 0)
             {
                 TempData["Error"] = "You are not in a covenant.";
                 return RedirectToAction("MyCovenant");
             }

             // assert that the player is a covenant leader or captain
             Covenant myCov = CovenantProcedures.GetDbCovenant(me.Covenant);
             if (myCov.LeaderId != me.Id && !CovenantProcedures.PlayerIsCaptain(myCov, me))
             {
                 TempData["Error"] = "You are not the leader or a captain of your covenant.";
                 TempData["SubError"] = "Only covenant leaders and captains can gift out money from the covenant's Arpeyjis chest.";
                 return RedirectToAction("MyCovenant");
             }

             // assert that the covenant does have this much to send
             if (myCov.Money < amount)
             {
                 TempData["Error"] = "Your covenant does not have that many Arpeyjis to send.";
                 return RedirectToAction("MyCovenant");
             }

             // assert that the amount is valid
             if (amount != 20 && amount != 100 && amount != 500)
             {
                 TempData["Error"] = "That is not a valid amount of Arpeyjis to send.";
                 return RedirectToAction("MyCovenant");
             }

             // assert that the giftee is in the same covenant
             Player giftee = PlayerProcedures.GetPlayer(id);
             if (giftee.Covenant != me.Covenant)
             {
                 TempData["Error"] = "This player is not in your covenant.";
                 return RedirectToAction("MyCovenant");
             }

             // assert that the covenant is sufficiently large
             if (PlayerProcedures.GetAnimatePlayerCountInCovenant(me.Covenant) < 3)
             {
                 TempData["Error"] = "In order to gift out Arpeyjis to members of your covenant, you must have at least 3 animate members.";
                 return RedirectToAction("MyCovenant");
             }


             CovenantProcedures.SendCovenantMoneyToPlayer(me.Covenant, giftee, amount);


            new Thread(() =>
                StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__CovenantNetDonation, (float)-amount)
            ).Start();

            TempData["Result"] = "You have successfully sent " + amount + " Arpeyjis to " + giftee.GetFullName() + ".";
             return RedirectToAction("MyCovenant");

         }

        [Authorize]
         public ActionResult ClaimLocation()
         {
             Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            

             // assert that player is in a covenant
             if (me.Covenant <= 0)
             {
                 TempData["Error"] = "You are not in a covenant.";
                 return RedirectToAction("MyCovenant");
             }

             // assert that the player is a covenant leader
             Covenant myCov = CovenantProcedures.GetDbCovenant(me.Covenant);
             if (myCov.LeaderId != me.Id)
             {
                 TempData["Error"] = "You are not the leader of your covenant.";
                 TempData["SubError"] = "Only covenant leaders can gift out money from the covenant's Arpeyjis chest.";
                 return RedirectToAction("MyCovenant");
             }

            // assert that the player is animate
             if (me.Mobility != "full")
             {
                 TempData["Error"] = "You must be animate in order to do this.";
                 return RedirectToAction("MyCovenant");
             }

             ViewBag.MyLocation = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == me.dbLocationName).Name;
             ViewBag.CovenantMoney = myCov.Money;

             return View();

         }

        [Authorize]
        public ActionResult ClaimLocationSend()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            // assert that the player is animate
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to do this.";
                return RedirectToAction("MyCovenant");
            }

            // assert that player is in a covenant
            if (me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the player is a covenant leader
            Covenant myCov = CovenantProcedures.GetDbCovenant(me.Covenant);
            if (myCov.LeaderId != me.Id)
            {
                TempData["Error"] = "You are not the leader of your covenant.";
                TempData["SubError"] = "Only covenant leaders can gift out money from the covenant's Arpeyjis chest.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the location is not in the dungeon
            if (me.dbLocationName.Contains("dungeon_"))
            {
                TempData["Error"] = "You cannot establish a safeground in the dungeon.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the covenant does not already have a safeground set
            if (CovenantProcedures.CovenantHasSafeground(myCov))
            {
                TempData["Error"] = "Your covenant already has a safeground at " + LocationsStatics.LocationList.GetLocation.FirstOrDefault(f => f.dbName == myCov.HomeLocation).Name + ".";
                TempData["SubError"] = "Covenants can only establish a safeground once per round.";
                return RedirectToAction("MyCovenant");
            }


            // assert that the covenant has enough money
            if (myCov.Money < 2500)
            {
                TempData["Error"] = "Your covenant cannot afford that right now.";
                TempData["SubError"] = "Try asking your members to donate more to the Covenant Chest.";
                return RedirectToAction("MyCovenant");
            }

            // assert that this location is not in the streets
            if (me.dbLocationName.Contains("street_"))
            {
                TempData["Error"] = "You cannot establish a covenant safeground on a path or street.";
                return RedirectToAction("MyCovenant");
            }

            // asset that this location is not already in use by another covenant
            if (CovenantProcedures.ACovenantHasASafegroundHere(me.dbLocationName))
            {
                TempData["Error"] = "Your covenant cannot establish a safeground here.";
                TempData["SubError"] = "Another covenant has already established a safeground at your current location.  You must find somewhere else.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the player is animate
            if (me.Mobility != "full")
            {
                TempData["Result"] = "You have succesfully claimed this location for your covenant safeground!";
                return RedirectToAction("MyCovenant");
            }

            // assert that the covenant has sufficient player count
            if (CovenantProcedures.GetPlayerCountInCovenant_Animate_Lvl3(myCov) < PvPStatics.Covenant_MinimumUpgradeAnimateLvl3PlayerCount)
            {
                TempData["Error"] = "Your covenant needs at least five animate players at level three or greater in order to do this.";
                return RedirectToAction("MyCovenant");
            }

            // all checks are okay, so allow this covenant to establish a safeground here

            TempData["Result"] = "You have succesfully claimed this location for your covenant safeground!";
            CovenantProcedures.SetCovenantSafeground(myCov, me.dbLocationName);
            return RedirectToAction("MyCovenant");

        }

        [Authorize]
        public ActionResult UpgradeSafeground()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            // assert that the player is animate
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to do this.";
                return RedirectToAction("MyCovenant");
            }

            // assert that player is in a covenant
            if (me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the player is a covenant leader
            Covenant myCov = CovenantProcedures.GetDbCovenant(me.Covenant);
            if (myCov.LeaderId != me.Id)
            {
                TempData["Error"] = "You are not the leader of your covenant.";
                TempData["SubError"] = "Only covenant leaders can gift out money from the covenant's Arpeyjis chest.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the covenant has a safeground set
            if (!CovenantProcedures.CovenantHasSafeground(myCov))
            {
                TempData["Error"] = "Your covenant must establish a safeground before it can upgrade it.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the player is at the safeground
            if (me.dbLocationName != myCov.HomeLocation)
            {
                TempData["Error"] = "You must be at your covenant safeground in order to upgrade it.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the covenant has enough money
            if (myCov.Money < CovenantProcedures.GetUpgradeCost(myCov))
            {
                TempData["Error"] = "Your covenant cannot afford that right now.";
                TempData["SubError"] = "Try asking your members to donate more to the Covenant Treasury.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the covenant has sufficient player count
            if (CovenantProcedures.GetPlayerCountInCovenant_Animate_Lvl3(myCov) < PvPStatics.Covenant_MinimumUpgradeAnimateLvl3PlayerCount)
            {
                TempData["Error"] = "Your covenant needs at least five animate players at level three or greater in order to do this.";
                return RedirectToAction("MyCovenant");
            }

            // all checks pass; upgrade the covenant
            CovenantProcedures.UpgradeCovenant(myCov);

            TempData["Result"] = "You have successfully upgraded your covenant safeground to lvl " + (myCov.Level + 1) + ".";
            return RedirectToAction("MyCovenant");
        }

         [Authorize]
        public ActionResult ViewAvailableFurniture()
        {
           
             Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

             // assert that player is in a covenant
             if (me.Covenant <= 0)
             {
                 TempData["Error"] = "You are not in a covenant.";
                 return RedirectToAction("MyCovenant");
             }

           //  FurnitureProcedures.MoveExpiredFurnitureBackToMarket();
             IEnumerable<FurnitureViewModel> output = FurnitureProcedures.GetAvailableFurnitureViewModels();

             Covenant myCov = CovenantProcedures.GetDbCovenant(me.Covenant);
             bool playerIsCovenantLeader = (myCov != null && myCov.LeaderId == me.Id && me.Covenant > 0);
             ViewBag.playerIsCovenantLeader = playerIsCovenantLeader;
             ViewBag.CovenantMoney = (int)myCov.Money;

             ViewBag.FurnitureLimit = CovenantProcedures.GetCovenantFurnitureLimit(myCov);

             ViewBag.IAmCaptain = CovenantProcedures.PlayerIsCaptain(myCov, me);
             
            return View(output);
        }

        [Authorize]
         public ActionResult PurchaseFurniture(int id)
         {
             string myMembershipId = User.Identity.GetUserId();
             Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

             // assert that player is in a covenant
             if (me.Covenant <= 0)
             {
                 TempData["Error"] = "You are not in a covenant.";
                 return RedirectToAction("MyCovenant");
             }

             // assert that the player is a covenant leader or captain
             Covenant myCov = CovenantProcedures.GetDbCovenant(me.Covenant);
             if (myCov.LeaderId != me.Id && !CovenantProcedures.PlayerIsCaptain(myCov, me))
             {
                 TempData["Error"] = "You are not the leader of your covenant.";
                 TempData["SubError"] = "Only covenant leaders can gift out money from the covenant's Arpeyjis chest.";
                 return RedirectToAction("MyCovenant");
             }

             Furniture furniture = FurnitureProcedures.GetdbFurniture(id);

            // assert that the covenant has enough money
             if (myCov.Money < furniture.Price)
             {
                 TempData["Error"] = "Your covenant does not have enough Arpeyjis to purchase this contract.";
                 return RedirectToAction("MyCovenant");
             }

            // assert that the covenant has a safeground
             if (myCov.HomeLocation == null || myCov.HomeLocation == "")
             {
                 TempData["Error"] = "Your covenant needs a safeground before it can purchase any furniture.";
                 return RedirectToAction("MyCovenant");
             }

            // assert that the furniture is on the market
             if (furniture.CovenantId != -1)
             {
                 TempData["Error"] = "This piece of furniture is not currently on the market.";
                 return RedirectToAction("MyCovenant");
             }

            // assert that the covenant has room for this new furniture
             if (CovenantProcedures.GetCovenantFurnitureLimit(myCov) <= CovenantProcedures.GetCurrentFurnitureOwnedByCovenant(myCov))
             {
                 TempData["Error"] = "Your safeground already has too much furniture and cannot fit any more.";
                 TempData["SubError"] = "Your covenant leader must upgrade the safeground in order to fit more furniture or else wait for some furnitures' contracts to expire.";
                 return RedirectToAction("MyCovenant");
             }

            // all checks have passed; give the furniture to the covenant
             
             FurnitureProcedures.GiveFurnitureToCovenant(furniture, myCov);

            string result = "Congratulations, your covenant, " + myCov.Name + ", has successfully purchased the contract for " + furniture.HumanName + ".";
            TempData["Result"] = result;

            ViewBag.FurnitureLimit = CovenantProcedures.GetCovenantFurnitureLimit(myCov);
            

             return RedirectToAction("MyCovenant");

         }

        [Authorize]
        public ActionResult MyCovenantFurniture()
        {
            
            Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            Covenant myCov = CovenantProcedures.GetDbCovenant(me.Covenant);

            // assert that player is in a covenant
            if (me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant.";
                return RedirectToAction("MyCovenant");
            }

           // FurnitureProcedures.MoveExpiredFurnitureBackToMarket();
            IEnumerable<FurnitureViewModel> output = FurnitureProcedures.GetCovenantFurnitureViewModels(me.Covenant);
            ViewBag.MyLocation = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == me.dbLocationName).Name;

            string covSafegroundLocation = "";
            if (myCov.HomeLocation != null && myCov.HomeLocation != "") {
                covSafegroundLocation = "Your covenant's safeground is at " + LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == myCov.HomeLocation).Name;
            }
            else
            {
                covSafegroundLocation = "Your covenant has not yet established a safeground and cannot yet lease any furniture.";
            }
            ViewBag.CovLocation = covSafegroundLocation;

            bool playerIsAtSafeground = false;

            if (myCov.HomeLocation != null && myCov.HomeLocation != "" && me.dbLocationName == myCov.HomeLocation) {
                playerIsAtSafeground = true;
            }

            ViewBag.AtCovenantSafeground = playerIsAtSafeground;

            ViewBag.FurnitureLimit = CovenantProcedures.GetCovenantFurnitureLimit(myCov);

            return View(output);

        }

        [Authorize]
        public ActionResult UseFurniture(int id)
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            Covenant myCov = CovenantProcedures.GetDbCovenant(me.Covenant);
            Furniture furniture = FurnitureProcedures.GetdbFurniture(id);

            // assert that the player is animate
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to do this.";
                return RedirectToAction("MyCovenant");
            }

            // assert that this player is not in a duel
            if (me.InDuel > 0)
            {
                TempData["Error"] = "You must finish your duel before you use covenant furniture.";
                return RedirectToAction("Play", "PvP");
            }

            // assert that this player is not in a quest
            if (me.InQuest > 0)
            {
                TempData["Error"] = "You must finish your quest before you use covenant furniture.";
                return RedirectToAction("Play", "PvP");
            }

            // assert that player is in a covenant
            if (me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the player is in the covenant that actually owns this
            if (furniture.CovenantId != me.Covenant)
            {
                TempData["Error"] = "Your covenant does not own this furniture.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the player is at the covenant safegrounds
            if (myCov.HomeLocation != me.dbLocationName)
            {
                TempData["Error"] = "You must be at your covenant safeground to use your covenant's furniture.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the contract has not expired nor has not begun yet
            //int turnNumber = PvPWorldStatProcedures.GetWorldTurnNumber();
            //if (furniture.ContractStartTurn > turnNumber || furniture.ContractEndTurn < turnNumber)
            //{
            //    TempData["Error"] = "This contract for this piece of furniture has either expired or not begun yet.";
            //    return RedirectToAction("MyCovenant");
            //}

            // assert that the item is not on recharge
            if (FurnitureProcedures.GetMinutesUntilReuse(furniture) > 0)
            {
                TempData["Error"] = "This item of furniture needs more time to regather its energy before it can be used for any bonuses.";
                return RedirectToAction("MyCovenant");
            }

            TempData["Result"] = FurnitureProcedures.UseFurniture(id, me);

            new Thread(() =>
                 StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__CovenantFurnitureUsed, 1)
             ).Start();

            return RedirectToAction("MyCovenant");

        }

        [Authorize]
        public ActionResult MyCovenantLog()
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert that player is in a covenant
            if (me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant.";
                return RedirectToAction("MyCovenant");
            }

            IEnumerable<CovenantLog> output = CovenantProcedures.GetCovenantLogs(me.Covenant);

            return View(output);
        }

        [Authorize]
        public ActionResult InviteCaptainList()
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert that player is in a covenant
            if (me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the player is a covenant leader
            Covenant myCov = CovenantProcedures.GetDbCovenant(me.Covenant);
            if (myCov.LeaderId != me.Id)
            {
                TempData["Error"] = "You are not the leader of your covenant.";
                return RedirectToAction("MyCovenant");
            }

            CovenantViewModel output = CovenantProcedures.GetCovenantViewModel(me.Covenant);

            ViewBag.CurrentCaptains = myCov.Captains;

            return View(output);
        }

        [Authorize]
        public ActionResult InviteCaptainSend(int id)
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert that player is in a covenant
            if (me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the player is a covenant leader
            Covenant myCov = CovenantProcedures.GetDbCovenant(me.Covenant);
            if (myCov.LeaderId != me.Id)
            {
                TempData["Error"] = "You are not the leader of your covenant.";
                return RedirectToAction("MyCovenant");
            }

            Player newCaptain = PlayerProcedures.GetPlayer(id);

            // assert that the target player is in the same covenant
            if (newCaptain.Covenant != myCov.Id)
            {
                TempData["Error"] = "This player is not a member of your covenant and cannot be made a captain.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the target is not the covenant leader
            if (newCaptain.Id == myCov.LeaderId)
            {
                TempData["Error"] = "This player is the leader of the covenant.";
                TempData["SubError"] = "There is nothing a covenant captain can do that a leader cannot.";
                return RedirectToAction("MyCovenant");
            }

            string result = CovenantProcedures.ChangeCovenantCaptain(myCov, newCaptain);

            TempData["Result"] = result;
            return RedirectToAction("MyCovenant");
        }

        [Authorize]
        public ActionResult InviteLeaderList()
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert that player is in a covenant
            if (me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the player is a covenant leader
            Covenant myCov = CovenantProcedures.GetDbCovenant(me.Covenant);
            if (myCov.LeaderId != me.Id)
            {
                TempData["Error"] = "You are not the leader of your covenant.";
                return RedirectToAction("MyCovenant");
            }

            CovenantViewModel output = CovenantProcedures.GetCovenantViewModel(me.Covenant);
            return View(output);
        }


        [Authorize]
        public ActionResult InviteLeaderSend(int id)
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert that player is in a covenant
            if (me.Covenant <= 0)
            {
                TempData["Error"] = "You are not in a covenant.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the player is a covenant leader
            Covenant myCov = CovenantProcedures.GetDbCovenant(me.Covenant);
            if (myCov.LeaderId != me.Id)
            {
                TempData["Error"] = "You are not the leader of your covenant.";
                return RedirectToAction("MyCovenant");
            }

            Player newLeader = PlayerProcedures.GetPlayer(id);

            // assert that the target player is in the same covenant
            if (newLeader.Covenant != myCov.Id)
            {
                TempData["Error"] = "This player is not a member of your covenant and cannot be made the leader.";
                return RedirectToAction("MyCovenant");
            }

            // assert that the target is not the covenant leader
            if (newLeader.Id == myCov.LeaderId)
            {
                TempData["Error"] = "This player is already the leader of the covenant.";
                return RedirectToAction("MyCovenant");
            }
            string result = CovenantProcedures.ChangeCovenantLeader(myCov, newLeader);

            TempData["Result"] = result;
            return RedirectToAction("MyCovenant");

        }
    }
}