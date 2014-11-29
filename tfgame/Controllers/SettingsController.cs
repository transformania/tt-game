using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tfgame.dbModels.Models;
using WebMatrix.WebData;
using tfgame.Procedures;
using tfgame.Statics;
using tfgame.ViewModels;
using tfgame.Filters;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;

namespace tfgame.Controllers
{
    [InitializeSimpleMembership]
    public class SettingsController : Controller
    {
         [Authorize]
        public ActionResult Index()
        {
            return View();
        }

         [Authorize]
        public ActionResult SetBio()
        {

            PlayerBio output = SettingsProcedures.GetPlayerBioFromMembershipId(WebSecurity.CurrentUserId);

            if (output == null)
            {
                output = new PlayerBio{
                    OwnerMembershipId = WebSecurity.CurrentUserId,
                    Timestamp = DateTime.UtcNow,
                    WebsiteURL = "",
                    Text = "",
                };
            }
            else
            {
                if (output.OwnerMembershipId != WebSecurity.CurrentUserId)
                {
                    TempData["Error"] = "This is not your biography.";
                    return RedirectToAction("Play", "PvP");
                }
            }

            return View(output);
        }

         [Authorize]
         public ActionResult SetBioSend(PlayerBio input)
         {

             if (input.Text == null)
             {
                 TempData["Error"] = "You must have some text in order to save your bio.";
                 return RedirectToAction("Play", "PvP");
             }

             Player me = PlayerProcedures.GetPlayerFromMembership();
             if (input.Text.Length > 2500 && DonatorProcedures.DonatorGetsMessagesRewards(me) == false)
             {
                 TempData["Error"] = "The text of your bio is too long (more than 2500 characters).";
                 return RedirectToAction("Play", "PvP");
             }

             if (input.Text.Length > 10000 && DonatorProcedures.DonatorGetsMessagesRewards(me) == true)
             {
                 TempData["Error"] = "The text of your bio is too long (more than 10,000 characters).";
                 return RedirectToAction("Play", "PvP");
             }

             if (input.WebsiteURL == null)
             {
                 input.WebsiteURL = "";
             }

             if (input.WebsiteURL.Length > 1500)
             {
                 TempData["Error"] = "The text of your website URL is too long (more than 250 characters).";
                 return RedirectToAction("Play", "PvP");
             }

             SettingsProcedures.SavePlayerBio(input);

             TempData["Result"] = "Your bio has been saved.";
             return RedirectToAction("Play", "PvP");
         }

        [Authorize]
         public ActionResult ViewBio(int id)
         {
             Player player = PlayerProcedures.GetPlayerFromMembership(id);
             ViewBag.Name = player.FirstName + " " + player.LastName;
             PlayerBio output = SettingsProcedures.GetPlayerBioFromMembershipId(id);

             if (output == null)
             {
                 TempData["Error"] = "It seems that " + player.FirstName + " " + player.LastName + " has not written a player biography yet.";
                 return RedirectToAction("Play", "PvP");
             }

            IContributionRepository contributionRepo = new EFContributionRepository();

            IEnumerable<BioPageContributionViewModel> mySpells = from c in contributionRepo.Contributions
                                                                 where c.OwnerMembershipId == player.MembershipId && c.ProofreadingCopy == true && c.IsLive == true
                                                                 select new BioPageContributionViewModel
                                                                 {
                                                                     SpellName = c.Skill_FriendlyName,
                                                                     FormName = c.Form_FriendlyName,
                                                                 };

            IEffectContributionRepository effectContribtionRepo = new EFEffectContributionRepository();

            IEnumerable<BioPageEffectContributionViewModel> myEffects = from c in effectContribtionRepo.EffectContributions
                                                                 where c.OwnerMemberhipId == player.MembershipId && c.ProofreadingCopy == true && c.IsLive == true
                                                                 select new BioPageEffectContributionViewModel
                                                                 {
                                                                     EffectName = c.Effect_FriendlyName,
                                                                     SpellName = c.Skill_FriendlyName
                                                                 };
            myEffects = myEffects.ToList();

            ViewBag.MyContributions = mySpells.ToList();
            ViewBag.MyEffectContributions = myEffects.ToList();

             return View(output);
         }

        [Authorize]
        public ActionResult DumpWillpower(string amount)
        {

            Player me = PlayerProcedures.GetPlayerFromMembership();

            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be fully animate and in protection mode in order to drop your willpower.";
                return RedirectToAction("Play", "PvP");
            }

            if (me.InPvP == false)
            {
                TempData["Error"] = "You must be fully animate and in protection mode in order to drop your willpower.";
                return RedirectToAction("Play", "PvP");
            }

            decimal drop = 0;
            if (amount == "half")
            {
                decimal halfHealth = me.MaxHealth / 2;
                if (halfHealth < me.Health)
                {
                   drop = me.Health - halfHealth;
                    PlayerProcedures.ChangePlayerActionManaNoTimestamp(0, -drop, 0, me.Id);
                    TempData["Result"] = "You voluntarily lower your willpower down to half of its maximum, making yourself completely vulnerable to animate transformations.";
                    return RedirectToAction("Play", "PvP");
                }
                else
                {
                    TempData["Error"] = "Your willpower is already lower than half of its maximum.";
                    return RedirectToAction("Play", "PvP");
                }
            }
            else if (amount == "full")
            {
               
                if (me.Health > 0)
                {
                   // drop = me.MaxHealth - me.Health;
                    PlayerProcedures.ChangePlayerActionManaNoTimestamp(0, -me.Health, 0, me.Id);
                    TempData["Result"] = "You voluntarily decrease your willpower to nothing, making yourself vulnerable to any type of transformation.";
                    return RedirectToAction("Play", "PvP");
                }
            }

            TempData["Error"] = "That is not a valid amount to decrease your willpower to.";
            return RedirectToAction("Play", "PvP");

            
        }

        public ActionResult Donate()
        {
            return View();
        }

        [Authorize]
        public ActionResult SetNickname()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            if (DonatorProcedures.DonatorGetsNickname(me) == false)
            {
                TempData["Error"] = "You are not marked as being a donator.";
                TempData["SubError"] = "This feature is reserved for players who pledge $7 monthly to support Transformania Time on Patreon.";
                return RedirectToAction("Play", "PvP");
            }

            Message output = new Message();
            ViewBag.OldNickname = me.Nickname;
            return View(output);
        }

        [Authorize]
        public ActionResult VerifyDonatorStatus()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            DonatorProcedures.SetNewPlayerDonationRank(me.FirstName + " " + me.LastName);

            me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            TempData["Result"] = "Your donation tier has been verified and set to tier " + me.DonatorLevel + ".";
            return RedirectToAction("Play", "PvP");

        }

        [Authorize]
        public ActionResult SetNicknameSend(Message input)
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            if (me.DonatorLevel < 2)
            {
                TempData["Error"] = "You are not marked as a tier 2 or above donator.";
                TempData["SubError"] = "This feature is reserved for players who pledge $7 monthly to support Transformania Time on Patreon.";
                return RedirectToAction("Play", "PvP");
            }


            if (input.MessageText.Length > 20) {
                TempData["Error"] = "That nickname is too long. ";
                TempData["SubError"] = "Nicknames must be no longer than 20 characters.";
                return RedirectToAction("Play", "PvP");
            }

            PlayerProcedures.SetNickname(input.MessageText);

            TempData["Result"] = "Your new nickname has been set.";
            return RedirectToAction("Play", "PvP");
        }

        [Authorize]
        public ActionResult ToggleBlacklistOnPlayer(int id)
        {
            Player me = PlayerProcedures.GetPlayerFromMembership();
            Player target = PlayerProcedures.GetPlayer(id);

            // assert that this player is not a bot
            if (target.MembershipId < 0)
            {
                TempData["Error"] = "You cannot blacklist an AI character.";
                return RedirectToAction("Play", "PvP");
            }

            // assert that this player has not been friended
            if (FriendProcedures.PlayerIsMyFriend(me, target) == true)
            {
                TempData["Error"] = "You cannot blacklist one of your friends.";
                TempData["SubError"] = "Cancel your friendship with this player first.";
                return RedirectToAction("Play", "PvP");
            }


            TempData["Result"] =  BlacklistProcedures.TogglePlayerBlacklist(me, target);


            return RedirectToAction("Play", "PvP");
        }

        public ActionResult ViewPolls()
        {
            return View();
        }

        [Authorize]
        public ActionResult ViewPoll(int id)
        {
            PollEntry output = SettingsProcedures.LoadPoll(1);
            return View("Polls/Open/poll" + id, output);
        }

        [Authorize]
        public ActionResult ReplyToPoll(PollEntry input)
        {

            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Invalid input.";
                return View("Polls/Open/poll" + input.PollId, input);
            }

            SettingsProcedures.SavePoll(input, 13, 1);
            TempData["Result"] = "Your response has been recorded.  Thanks for your participation!";
            return RedirectToAction("Play", "PvP");
        }

        [Authorize]
        public ActionResult PollResultsList()
        {
            return View();
        }


        public ActionResult PollResults(int id)
        {
            return View();
        }

	}
}