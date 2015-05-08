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
         public ActionResult Settings()
         {
             Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

             ViewBag.GameMode = me.GameMode;

             ViewBag.TimeUntilLogout = 60 - Math.Abs(Math.Floor(me.LastActionTimestamp.Subtract(DateTime.UtcNow).TotalMinutes));

             return View(me);
         }


         //[Authorize]
         //public ActionResult EnterProtection()
         //{
         //    Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

         //    double minutesAgo = Math.Abs(Math.Floor(me.GetLastCombatTimestamp().Subtract(DateTime.UtcNow).TotalMinutes));
         //    if (minutesAgo < 60 && me.Mobility == "full")
         //    {
         //        TempData["Error"] = "You must have not been in any combat in the past 60 minutes to do this if you are animate.";
         //        return RedirectToAction("Play", "PvP");
         //    }

         //    // assert player is animate
         //    if (me.Mobility != "full")
         //    {
         //        TempData["Error"] = "You must be animate in order to enter Protection mode.";
         //        return RedirectToAction("Play", "PvP");
         //    }

         //    PlayerProcedures.SetPvPFlag(me, 1);

         //    TempData["Result"] = "You are now in protection mode.  You cannot be hit by inanimate, pet, or mind control spells nor can you cast them except against those on your friends list.";
         //    return RedirectToAction("Play", "PvP");
         //}

         //[Authorize]
         //public ActionResult LeaveProtection()
         //{
         //    Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

         //    double minutesAgo = Math.Abs(Math.Floor(me.GetLastCombatTimestamp().Subtract(DateTime.UtcNow).TotalMinutes));
         //    if (minutesAgo < 60 && me.Mobility == "full")
         //    {
         //        TempData["Error"] = "You must have not been in any combat in the past 60 minutes to do this if you are animate.";
         //        return RedirectToAction("Play", "PvP");
         //    }

         //    // assert player is animate
         //    if (me.Mobility != "full")
         //    {
         //        TempData["Error"] = "You must be animate in order to enter PvP mode.";
         //        return RedirectToAction("Play", "PvP");
         //    }

         //    // assert that it is not too late in the round for the player to enter PvP mode
         //    //int turnNumber = PvPWorldStatProcedures.GetWorldTurnNumber();
         //    //if (turnNumber > PvPStatics.RoundDuration_LastPvPEntryTurn)
         //    //{
         //    //    TempData["Error"] = "You cannot enter PvP mode anymore this round.";
         //    //    TempData["SubError"] = "You cannot enter PvP mode later than turn " + PvPStatics.RoundDuration_LastPvPEntryTurn + ".";
         //    //    return RedirectToAction("Play", "PvP");
         //    //}

         //    PlayerProcedures.SetPvPFlag(me, 2);
         //    EffectProcedures.GivePerkToPlayer("help_entered_PvP", me);

         //    TempData["Result"] = "You are no longer in protection mode.";
         //    return RedirectToAction("Play", "PvP");
         //}

         [Authorize]
         public ActionResult EnterSuperProtection()
         {
             Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

             if (me.GameMode != 1)
             {
                 TempData["Error"] = "You must be in Protection mode in order to enter SuperProtection mode.";
                 return RedirectToAction("Play","PvP");
             }

             PlayerProcedures.SetPvPFlag(me, 0);

             TempData["Result"] = "You are now in superprotection mode.  All spells are disabled against you except those cast by players on your friends list and bots.";
             return RedirectToAction("Play", "PvP");
         }

         [Authorize]
         public ActionResult LeaveSuperProtection()
         {
             Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

             if (me.GameMode != 0)
             {
                 TempData["Error"] = "You must be in SuperProtection mode in order to enter Protection mode.";
                 return RedirectToAction("Play", "PvP");
             }

             PlayerProcedures.SetPvPFlag(me, 1);

             TempData["Result"] = "You are no longer in SuperProtection mode.";
             return RedirectToAction("Play", "PvP");
         }

         [Authorize]
         public ActionResult EnableRP()
         {
             Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);
             TempData["Result"] = PlayerProcedures.SetRPFlag(me, true);
             return RedirectToAction("Play","PvP");
         }

         [Authorize]
         public ActionResult DisableRP()
         {
             Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);
             TempData["Result"] = PlayerProcedures.SetRPFlag(me, false);
             return RedirectToAction("Play", "PvP");
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
                 input.Text = "";
             }

             if (input.Tags == null)
             {
                 input.Tags = "";
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

             if (input.Tags.Length > 1000)
             {
                 TempData["Error"] = "Too many RP tags input text.";
                 return RedirectToAction("Play", "PvP");
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
         public ActionResult SetBioDelete(PlayerBio input)
         {

             SettingsProcedures.DeletePlayerBio(WebSecurity.CurrentUserId);

             TempData["Result"] = "Your bio has been deleted.";
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
                 TempData["Error"] = "It seems that this player has not written a player biography yet.";
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

            if (me.GameMode == 2)
            {
                TempData["Error"] = "You must be fully animate and in Protection or SuperProtection mode in order to drop your willpower.";
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

            DonatorProcedures.SetNewPlayerDonationRank(me.Id);

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

        [Authorize]
        public ActionResult MyBlacklistEntries()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership();
            IEnumerable<BlacklistEntryViewModel> output = BlacklistProcedures.GetMyBlacklistEntries(me);

            return View(output);
        }

        [Authorize]
        public ActionResult ChangeBlacklistType(int id, int playerId, string type)
        {

            Player me = PlayerProcedures.GetPlayerFromMembership();
            Player target = PlayerProcedures.GetPlayer(playerId);


            // assert that this player is not a bot
            if (target.MembershipId < 0)
            {
                TempData["Error"] = "You cannot blacklist an AI character.";
                return RedirectToAction("Play", "PvP");
            }

            // assert that this player owns this blacklist entry


            // assert that this player has not been friended
            if (FriendProcedures.PlayerIsMyFriend(me, target) == true)
            {
                TempData["Error"] = "You cannot blacklist one of your friends.";
                TempData["SubError"] = "Cancel your friendship with this player first.";
                return RedirectToAction("Play", "PvP");
            }

            TempData["Result"] = BlacklistProcedures.TogglePlayerBlacklistType(id, type, me, target);
            return RedirectToAction("Play", "PvP");

        }

        public ActionResult ViewPolls()
        {
            return View();
        }

        [Authorize]
        public ActionResult ViewPoll(int id)
        {
            PollEntry output = SettingsProcedures.LoadPoll(id);
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

            SettingsProcedures.SavePoll(input, 14, input.PollId);
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
            IEnumerable<PollEntry> output = SettingsProcedures.GetAllPollResults(id);
            return View("Polls/Read/poll" + id, output);
        }

        public ActionResult PollResultsClosed(int id)
        {
           // IEnumerable<PollEntry> output = SettingsProcedures.GetAllPollResults(id);
            return View("Polls/Closed/poll" + id);
        }

        [Authorize]
        public ActionResult SetChatColor(string color)
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            string filename = System.Web.HttpContext.Current.Server.MapPath("~/XMLs/validChatColors.txt");
            string text = System.IO.File.ReadAllText(filename);

            if (text.Contains(color + ";") == false)
            {
                TempData["Error"] = "That is not a valid chat color.";
                return RedirectToAction("Play", "PvP");
            }

            PlayerProcedures.SetChatColor(me, color);
            TempData["Result"] = "Your chat color has been set to " + color + ".";
            return RedirectToAction("Play", "PvP");
        }


        [Authorize]
        public ActionResult WriteAuthorArtistBio()
        {
            // assert player has on the artist whitelist
            if (User.IsInRole(PvPStatics.Permissions_Artist) == false)
            {
                TempData["Error"] = "You are not eligible to do this at this time.";
                return RedirectToAction("Play", "PvP");
            }

            AuthorArtistBio output = SettingsProcedures.GetAuthorArtistBio(WebSecurity.CurrentUserId);
            return View(output);
        }


        [HttpPost]
        [Authorize]
        public ActionResult WriteAuthorArtistSend(AuthorArtistBio input)
        {
            // assert player has on the artist whitelist
            if (User.IsInRole(PvPStatics.Permissions_Artist) == false)
            {
                TempData["Error"] = "You are not eligible to do this at this time.";
                return RedirectToAction("Play", "PvP");
            }

            SettingsProcedures.SaveAuthorArtistBio(input);

            TempData["Result"] = "Your artist bio has been saved!";
            return RedirectToAction("Play", "PvP");
        }

        [Authorize]
        public ActionResult AuthorArtistBio(int id)
        {
            AuthorArtistBio output = SettingsProcedures.GetAuthorArtistBio(id);
            Player artistIngamePlayer = PlayerProcedures.GetPlayerFromMembership(id);
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);
            ViewBag.IngameCharacter = "This artist does not currently have a character ingame.";

            bool friends = false;
            try
            {
                friends = FriendProcedures.PlayerIsMyFriend(artistIngamePlayer, me);
                ViewBag.IAmFriendsWithArtist = friends;
            }
            catch
            {
                ViewBag.IAmFriendsWithArtist = false;
            }

            if (artistIngamePlayer != null)
            {
                ViewBag.IngameCharacter = "This artist current has a character under the name of " + artistIngamePlayer.GetFullName() + ".";
            }
            // assert visibility setting is okay
            if (output.PlayerNamePrivacyLevel == 1 && friends == false)
            {
                TempData["Error"] = "This artist bio is only visible to his or her friends.";
                return RedirectToAction("Play", "PvP");
            }

            if (output.PlayerNamePrivacyLevel == 2)
            {
                TempData["Error"] = "This artist's biography is currently entirely disabled.  Check again later.";
                return RedirectToAction("Play", "PvP");
            }

            if (output.Text != null) {
                output.Text = output.Text.Replace("[br]", "<br>").Replace("[p]", "<p>").Replace("[/p]", "</p>").Replace("[h1]", "<h1>").Replace("[/h1]", "</h1>").Replace("[h2]", "<h2>").Replace("[/h2]", "</h2>").Replace("[h3]", "<h3>").Replace("[/h3]", "</h3>");
            }

            return View(output);
        }

        [Authorize]
        public ActionResult UseMyCustomForm()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership();

            string filename = System.Web.HttpContext.Current.Server.MapPath("~/XMLs/custom_bases.xml");
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<CustomFormViewModel>));
            System.IO.StreamReader file = new System.IO.StreamReader(filename);
            List<CustomFormViewModel> output = (List<CustomFormViewModel>)reader.Deserialize(file);

            CustomFormViewModel newForm = output.FirstOrDefault(p => p.MembershipId == WebSecurity.CurrentUserId);

            if (newForm == null)
            {
                TempData["Error"] = "You do not have a custom base form.";
                TempData["SubError"] = "Read more about how to get one here:  http://luxianne.com/forum/viewtopic.php?f=9&t=400";
                return RedirectToAction("Play", "PvP");
            }

            

            // player is already in their original form so change them instantly.  Otherwise they'll have to find a way to be restored themselves
            if (me.Form == me.OriginalForm)
            {
                PlayerProcedures.SetCustomBase(me, newForm.Form);
                PlayerProcedures.InstantRestoreToBase(me);
            }
            else
            {
                PlayerProcedures.SetCustomBase(me, newForm.Form);
            }

            
            TempData["Result"] = "Your custom form has been set.";
            return RedirectToAction("Play", "PvP");
        }

        [Authorize]
        public ActionResult ArchiveSpell(string name)
        {
            Player me = PlayerProcedures.GetPlayerFromMembership();
            // assert that player does own this skill
            SkillViewModel2 skill = SkillProcedures.GetSkillViewModel(name, me.Id);

            if (skill == null)
            {
                TempData["Error"] = "You don't know this spell yet.";
                return RedirectToAction("Play", "PvP");
            }

            SkillProcedures.ArchiveSpell(skill.dbSkill.Id);

            if (skill.dbSkill.IsArchived == false) { 
                ViewBag.Message = "You have successfully archived " + skill.Skill.FriendlyName + ".";
            }
            else
            {
                ViewBag.Message = "You have successfully restored " + skill.Skill.FriendlyName + " from your spell archive.";
            }
            ViewBag.Number = skill.dbSkill.Id;
            return PartialView("partial/ArchiveNotice");
        }

        [Authorize]
        public ActionResult ArchiveAllMySpells(string archive)
        {
            Player me = PlayerProcedures.GetPlayerFromMembership();
            if (archive == "True")
            {
                TempData["Result"] = "You have archived all of your known spells.  They will not appear on the attack modal until you unarchive them.";
                SkillProcedures.ArchiveAllSpells(me.Id, true);
            }
            else
            {
                TempData["Result"] = "You have archived all of your known spells.  They will all now appear on the attack modal again.";
                SkillProcedures.ArchiveAllSpells(me.Id, false);
            }
            
           
            return RedirectToAction("Play", "PvP");
        }

        public ActionResult PlayerStats(int id)
        {
            Player player = PlayerProcedures.GetPlayerFromMembership(id);
            ViewBag.Name = player.GetFullName();
            ViewBag.PlayerId = player.Id;
            IEnumerable<Achievement> output = StatsProcedures.GetPlayerStats(id);
            return View(output);
        }

        public ActionResult PlayerStatsLeaders()
        {
            List<PlayerAchievementViewModel> output = StatsProcedures.GetPlayerMaxStats().ToList();
            return View(output);
        }

        [Authorize]
        public ActionResult SetFriendNickname(int id)
        {
            Player me = PlayerProcedures.GetPlayerFromMembership();
            Friend friend = FriendProcedures.GetFriend(id);

            Player pFriend;

            if (friend.OwnerMembershipId == me.MembershipId)
            {
                pFriend = PlayerProcedures.GetPlayerFromMembership(friend.FriendMembershipId);
            }
            else
            {
                pFriend = PlayerProcedures.GetPlayerFromMembership(friend.OwnerMembershipId);
            }

            if (friend.OwnerMembershipId != me.MembershipId && friend.OwnerMembershipId != pFriend.MembershipId)
            {
                TempData["Error"] = "This player is not a friend with you.";
                return RedirectToAction("MyFriends", "PvP");
            }

            SetFriendNicknameViewModel output = new SetFriendNicknameViewModel
            {
                Owner = me,
                OwnerMembershipId = me.MembershipId,
                Friend = pFriend,
                FriendMembershipId = pFriend.MembershipId,
                Nickname = "[SET NICKNAME]",
                FriendshipId = friend.Id,
            };

                return View(output);
        }

        [Authorize]
        public ActionResult SetFriendNicknameSend(SetFriendNicknameViewModel input)
        {

            if (input.Nickname == null)
            {
                input.Nickname = "";
            }
            input.Nickname = input.Nickname.Trim();

            // asset the nickname falls within an appropriate range
            if (input.Nickname.Length == 0)
            {
                TempData["Error"] = "You must provide a nickname.";
                return RedirectToAction("MyFriends", "PvP");
            }
            else if (input.Nickname.Length > 20)
            {
                TempData["Error"] = "Friend nicknames must be under 20 characters.";
                return RedirectToAction("MyFriends", "PvP");
            }

            Player me = PlayerProcedures.GetPlayerFromMembership();
            Friend friend = FriendProcedures.GetFriend(input.FriendshipId);

            Player pFriend;

            // this player is the owner of the friendship
            if (friend.OwnerMembershipId == me.MembershipId)
            {
                pFriend = PlayerProcedures.GetPlayerFromMembership(friend.FriendMembershipId);
            }

            // this player is the receiver of the friendship
            else
            {
                pFriend = PlayerProcedures.GetPlayerFromMembership(friend.OwnerMembershipId);
            }

            if (friend.OwnerMembershipId != me.MembershipId && friend.OwnerMembershipId != pFriend.MembershipId)
            {
                TempData["Error"] = "This player is not a friend with you.";
                return RedirectToAction("Play", "PvP");
            }

            // set the nickname based on whether the current player is the owner or the friend
            else if (friend.OwnerMembershipId == me.MembershipId)
            {
                TempData["Result"] = FriendProcedures.OwnerSetNicknameOfFriend(friend.Id, input.Nickname);
            }
            else if (friend.OwnerMembershipId == pFriend.MembershipId)
            {
                TempData["Result"] = FriendProcedures.FriendSetNicknameOfOwner(friend.Id, input.Nickname);
            }

            
            return RedirectToAction("MyFriends", "PvP");
        }
       

	}
}