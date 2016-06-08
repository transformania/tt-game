using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Web.Controllers
{
    public class SettingsController : Controller
    {
         [Authorize]
         public ActionResult Settings()
         {
             string myMembershipId = User.Identity.GetUserId();
             Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

             ViewBag.GameMode = me.GameMode;
             ViewBag.Mobility = me.Mobility;
             ViewBag.TimeUntilReroll = Math.Round(RerollProcedures.GetTimeUntilReroll(me).TotalMinutes);
             ViewBag.TimeUntilLogout = PvPStatics.OfflineAfterXMinutes - Math.Abs(Math.Floor(me.LastActionTimestamp.Subtract(DateTime.UtcNow).TotalMinutes));

             return View(me);
         }

         [Authorize]
         public ActionResult EnterSuperProtection()
         {
             string myMembershipId = User.Identity.GetUserId();
             Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

             if (me.GameMode != GameModeStatics.Protection)
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
             string myMembershipId = User.Identity.GetUserId();
             Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

             if (me.GameMode != GameModeStatics.SuperProtection)
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
             string myMembershipId = User.Identity.GetUserId();
             Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
             TempData["Result"] = PlayerProcedures.SetRPFlag(me, true);
             return RedirectToAction("Play","PvP");
         }

         [Authorize]
         public ActionResult DisableRP()
         {
             string myMembershipId = User.Identity.GetUserId();
             Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
             TempData["Result"] = PlayerProcedures.SetRPFlag(me, false);
             return RedirectToAction("Play", "PvP");
         }

         [Authorize]
        public ActionResult SetBio()
        {
            string myMembershipId = User.Identity.GetUserId();
            PlayerBio output = SettingsProcedures.GetPlayerBioFromMembershipId(myMembershipId);

            if (output == null)
            {
                output = new PlayerBio{
                    OwnerMembershipId = myMembershipId,
                    Timestamp = DateTime.UtcNow,
                    WebsiteURL = "",
                    Text = "",
                };
            }
            else
            {
                if (output.OwnerMembershipId != myMembershipId)
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
             string myMembershipId = User.Identity.GetUserId();
             if (input.Text == null)
             {
                 input.Text = "";
             }

             if (input.Tags == null)
             {
                 input.Tags = "";
             }

             Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
             if (input.Text.Length > 2500 && !DonatorProcedures.DonatorGetsMessagesRewards(me))
             {
                 TempData["Error"] = "The text of your bio is too long (more than 2500 characters).";
                 return RedirectToAction("Play", "PvP");
             }

             if (input.Text.Length > 10000 && DonatorProcedures.DonatorGetsMessagesRewards(me))
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

             SettingsProcedures.SavePlayerBio(input, myMembershipId);

             TempData["Result"] = "Your bio has been saved.";
             return RedirectToAction("Play", "PvP");
         }

        [Authorize]
         public ActionResult SetBioDelete(PlayerBio input)
         {
             string myMembershipId = User.Identity.GetUserId();
             SettingsProcedures.DeletePlayerBio(myMembershipId);

             TempData["Result"] = "Your bio has been deleted.";
             return RedirectToAction("Play", "PvP");
         }

        [Authorize]
         public ActionResult ViewBio(string id)
         {
             Player player = PlayerProcedures.GetPlayerFromMembership(id);
             ViewBag.Name = player.GetFullName();

             BioPageViewModel output = new BioPageViewModel();
             output.PlayerBio = SettingsProcedures.GetPlayerBioFromMembershipId(id);
             if (output.PlayerBio == null)
             {
                 TempData["Error"] = "It seems that this player has not written a player biography yet.";
                 return RedirectToAction("Play", "PvP");
             }

             output.Badges = StatsProcedures.GetPlayerBadges(player.MembershipId);


            IContributionRepository contributionRepo = new EFContributionRepository();

            IEnumerable<BioPageContributionViewModel> mySpells = from c in contributionRepo.Contributions
                                                                 where c.OwnerMembershipId == player.MembershipId && c.ProofreadingCopy && c.IsLive
                                                                 select new BioPageContributionViewModel
                                                                 {
                                                                     SpellName = c.Skill_FriendlyName,
                                                                     FormName = c.Form_FriendlyName,
                                                                 };

            IEffectContributionRepository effectContribtionRepo = new EFEffectContributionRepository();

            IEnumerable<BioPageEffectContributionViewModel> myEffects = from c in effectContribtionRepo.EffectContributions
                                                                 where c.OwnerMemberhipId == player.MembershipId && c.ProofreadingCopy && c.IsLive
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
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            if (me.Mobility != PvPStatics.MobilityFull)
            {
                TempData["Error"] = "You must be fully animate and in protection mode in order to drop your willpower.";
                return RedirectToAction("Play", "PvP");
            }

            // assert player is not a duel
            if (me.InDuel > 0)
            {
                TempData["Error"] = "You must finish your duel before you can drop your willpower.";
                return RedirectToAction("Play", "PvP");
            }

            // assert player is not a quest
            if (me.InQuest > 0)
            {
                TempData["Error"] = "You must finish your quest before you can drop your willpower.";
                return RedirectToAction("Play", "PvP");
            }

            if (me.GameMode == GameModeStatics.PvP)
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
            else if (amount == PvPStatics.MobilityFull)
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
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            if (!DonatorProcedures.DonatorGetsNickname(me))
            {
                TempData["Error"] = "You are not marked as being a donator.";
                TempData["SubError"] = "This feature is reserved for players who pledge $7 monthly to support Transformania Time on Patreon.";
                return RedirectToAction("Play", "PvP");
            }

            Message output = new Message();
            output.MessageText = me.Nickname;

            return View(output);
        }

        [Authorize]
        public ActionResult VerifyDonatorStatus()
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            DonatorProcedures.SetNewPlayerDonationRank(me.Id);

            me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            TempData["Result"] = "Your donation tier has been verified and set to tier " + me.DonatorLevel + ".";
            return RedirectToAction("Play", "PvP");

        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetNicknameSend(Message input)
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            if (me.DonatorLevel < 2)
            {
                TempData["Error"] = "You are not marked as a tier 2 or above donator.";
                TempData["SubError"] = "This feature is reserved for players who pledge $7 monthly to support Transformania Time on Patreon.";
                return RedirectToAction("Play", "PvP");
            }


            if (input.MessageText != null && input.MessageText.Length > 20) {
                TempData["Error"] = "That nickname is too long. ";
                TempData["SubError"] = "Nicknames must be no longer than 20 characters.";
                return RedirectToAction("Play", "PvP");
            }

            PlayerProcedures.SetNickname(input.MessageText, myMembershipId);

            if (me.Mobility == PvPStatics.MobilityInanimate || me.Mobility == PvPStatics.MobilityPet)
            {
                ItemProcedures.SetNickname(me, input.MessageText);
            }

            TempData["Result"] = "Your new nickname has been set.";
            return RedirectToAction("Play", "PvP");
        }

        [Authorize]
        public ActionResult ToggleBlacklistOnPlayer(int id)
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            Player target = PlayerProcedures.GetPlayer(id);

            // assert that this player is not a bot
            if (target.BotId < AIStatics.ActivePlayerBotId)
            {
                TempData["Error"] = "You cannot blacklist an AI character.";
                return RedirectToAction("Play", "PvP");
            }

            // assert that this player has not been friended
            if (FriendProcedures.PlayerIsMyFriend(me, target))
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
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            IEnumerable<BlacklistEntryViewModel> output = BlacklistProcedures.GetMyBlacklistEntries(me);

            return View(output);
        }

        [Authorize]
        public ActionResult ChangeBlacklistType(int id, int playerId, string type)
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            Player target = PlayerProcedures.GetPlayer(playerId);


            // assert that this player is not a bot
            if (target.BotId < AIStatics.ActivePlayerBotId)
            {
                TempData["Error"] = "You cannot blacklist an AI character.";
                return RedirectToAction("Play", "PvP");
            }

            // assert that this player owns this blacklist entry


            // assert that this player has not been friended
            if (FriendProcedures.PlayerIsMyFriend(me, target))
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
            string myMembershipId = User.Identity.GetUserId();
            PollEntry output = SettingsProcedures.LoadPoll(id, myMembershipId);
            return View("Polls/Open/poll" + id, output);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReplyToPoll(PollEntry input)
        {
            string myMembershipId = User.Identity.GetUserId();
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Invalid input.";
                return View("Polls/Open/poll" + input.PollId, input);
            }

            SettingsProcedures.SavePoll(input, 14, input.PollId, myMembershipId);
            TempData["Result"] = "Your response has been recorded.  Thanks for your participation!";
            return RedirectToAction("Play", "PvP");
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
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            string filename = AppDomain.CurrentDomain.BaseDirectory + "XMLs/validChatColors.txt";
            string text = System.IO.File.ReadAllText(filename);

            if (!text.Contains(color + ";"))
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
            string myMembershipId = User.Identity.GetUserId();
            // assert player has on the artist whitelist
            if (!User.IsInRole(PvPStatics.Permissions_Artist))
            {
                TempData["Error"] = "You are not eligible to do this at this time.";
                return RedirectToAction("Play", "PvP");
            }

            AuthorArtistBio output = SettingsProcedures.GetAuthorArtistBio(myMembershipId);
            return View(output);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult WriteAuthorArtistSend(AuthorArtistBio input)
        {
            string myMembershipId = User.Identity.GetUserId();
            // assert player has on the artist whitelist
            if (!User.IsInRole(PvPStatics.Permissions_Artist))
            {
                TempData["Error"] = "You are not eligible to do this at this time.";
                return RedirectToAction("Play", "PvP");
            }

            SettingsProcedures.SaveAuthorArtistBio(input, myMembershipId);

            TempData["Result"] = "Your artist bio has been saved!";
            return RedirectToAction("Play", "PvP");
        }

        [Authorize]
        public ActionResult AuthorArtistBio(string id)
        {
            string myMembershipId = User.Identity.GetUserId();
            AuthorArtistBio output = SettingsProcedures.GetAuthorArtistBio(id);
            Player artistIngamePlayer = PlayerProcedures.GetPlayerFromMembership(id);
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            ViewBag.IngameCharacter = "This artist does not currently have a character ingame.";

            bool friends = FriendProcedures.MemberIsMyFriend(id, myMembershipId);
            ViewBag.IAmFriendsWithArtist = friends;

            if (artistIngamePlayer != null)
            {
                ViewBag.IngameCharacter = "This artist current has a character under the name of " + artistIngamePlayer.GetFullName() + ".";
            }
            // assert visibility setting is okay
            if (output.PlayerNamePrivacyLevel == 1 && !friends)
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

        /// <summary>
        /// Allows a player to claim a new base form if they have earned one through being a contributor or artist.  Multiple custom forms can be toggled through by clicking the link mulitple times; each click will advance to the next available form and upon reaching the final form loop back to the first one.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult UseMyCustomForm()
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            IContributorCustomFormRepository repo = new EFContributorCustomFormRepository();
            var customForms = repo.ContributorCustomForms.Where(c => c.OwnerMembershipId == myMembershipId).ToList();

            if (!customForms.Any())
            {
                TempData["Error"] = "You do not have any custom base forms.";
                TempData["SubError"] = "Read more about how to get one here:  http://luxianne.com/forum/viewtopic.php?f=9&t=400";
                return RedirectToAction("Play", "PvP");
            }

            ContributorCustomForm newForm = customForms.First();

            int index = 0;
            foreach (ContributorCustomForm c in customForms)
            {
                if (me.OriginalForm == c.CustomForm.dbName)
                {
                    if (index + 1 < customForms.Count())
                    {
                        newForm = customForms.ElementAt(index + 1);
                    }
                    break;
                }
                index++;
            }

            // player is already in their original form so change them instantly.  Otherwise they'll have to find a way to be restored themselves
            if (me.Form == me.OriginalForm)
            {
                PlayerProcedures.SetCustomBase(me, newForm.CustomForm.dbName);
                PlayerProcedures.InstantRestoreToBase(me);
            }
            else
            {
                PlayerProcedures.SetCustomBase(me, newForm.CustomForm.dbName);
            }

            
            TempData["Result"] = "Your custom form has been set.";
            return RedirectToAction("Play", "PvP");
        }

        [Authorize]
        public ActionResult ArchiveSpell(string name)
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            // assert that player does own this skill
            SkillViewModel skill = SkillProcedures.GetSkillViewModel(name, me.Id);

            if (skill == null)
            {
                TempData["Error"] = "You don't know this spell yet.";
                return RedirectToAction("Play", "PvP");
            }

            SkillProcedures.ArchiveSpell(skill.dbSkill.Id);

            if (!skill.dbSkill.IsArchived) {
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
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            if (archive == "True")
            {
                SkillProcedures.ArchiveAllSpells(me.Id, true);
                TempData["Result"] = "You have archived all of your known spells.  They will not appear on the attack modal until you unarchive them.";
            }
            else
            {
                SkillProcedures.ArchiveAllSpells(me.Id, false);
                TempData["Result"] = "You have unarchived all of your known spells.  They will all now appear on the attack modal again.";
            }
            
           
            return RedirectToAction("Play", "PvP");
        }

        public ActionResult PlayerStats(string id)
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

        public ActionResult PlayerStatsTopOfType(string type)
        {
            IEnumerable<PlayerAchievementViewModel> output = StatsProcedures.GetLeaderPlayersInStat(type);
            return PartialView("partial/PlayerStatsTopOfType", output);
        }

        [Authorize]
        public ActionResult SetFriendNickname(int id)
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetFriendNicknameSend(SetFriendNicknameViewModel input)
        {
            string myMembershipId = User.Identity.GetUserId();
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

            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
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

        [Authorize]
        public ActionResult MyRPClassifiedAds()
        {
            string myMembershipId = User.Identity.GetUserId();
             Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
             IEnumerable<RPClassifiedAd> output = RPClassifiedAdsProcedures.GetPlayersClassifiedAds(me);

             ViewBag.ErrorMessage = TempData["Error"];
             ViewBag.SubErrorMessage = TempData["SubError"];
             ViewBag.Result = TempData["Result"];

             return View(output);
        }

        [Authorize]
        public ActionResult EditRPClassifiedAd(int id)
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            RPClassifiedAd ad = RPClassifiedAdsProcedures.GetClassifiedAd(id);

            // assert player is owner of the RP add or else it is new
            if (me.MembershipId != ad.OwnerMembershipId && ad.Id > 0)
            {
                TempData["Error"] = "You do not own this RP Classified Ad.";
                return RedirectToAction("MyRPClassifiedAds", "Settings");
            }

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];


            return View(ad);
        }

        [Authorize]
        public ActionResult RefreshRPClassifiedAd(int id)
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            RPClassifiedAd ad = RPClassifiedAdsProcedures.GetClassifiedAd(id);

            // assert player is owner of the RP add or else it is new
            if (me.MembershipId != ad.OwnerMembershipId && ad.Id > 0)
            {
                TempData["Error"] = "You do not own this RP Classified Ad.";
                return RedirectToAction("MyRPClassifiedAds", "Settings");
            }

            RPClassifiedAdsProcedures.RefreshAd(id);

            TempData["Result"] = "RP classified ad successfully refreshed.";
            return RedirectToAction("MyRPClassifiedAds", "Settings");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRPClassifiedAdSend(RPClassifiedAd input)
        {

            input.SetNullsToEmptyStrings();

            string myMembershipId = User.Identity.GetUserId();

            // assert the title field is not too long
            if (input.Title.Length > 35)
            {
                ViewBag.ErrorMessage = "The ad title is too long.";
                return View("EditRPClassifiedAd", input);
            }

            // assert the title field is  not too long
            if (input.Title.Length < 5)
            {
                ViewBag.ErrorMessage = "The ad title is too short.";
                return View("EditRPClassifiedAd", input);
            }

            // assert the text fields are not too long
            if (input.Text.Length > 300)
            {
                ViewBag.ErrorMessage = "The ad description is too long.";
                return View("EditRPClassifiedAd", input);
            }

            // assert the text fields are not too short
            if (input.Text == null || input.Text.Length < 50)
            {
                ViewBag.ErrorMessage = "The ad description is too short.";
                return View("EditRPClassifiedAd", input);
            }

            // assert the yes field is not too long
            if (input.YesThemes.Length > 200)
            {
                ViewBag.ErrorMessage = "The ad description is too long.";
                return View("EditRPClassifiedAd", input);
            }

            // assert the no field is not too long
            if (input.NoThemes.Length > 200)
            {
                ViewBag.ErrorMessage = "The ad description is too long.";
                return View("EditRPClassifiedAd", input);
            }

            // assert the timezone fields is not too long
            if (input.PreferredTimezones.Length > 70)
            {
                ViewBag.ErrorMessage = "The ad title is too long.";
                return View("EditRPClassifiedAd", input);
            }

            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            RPClassifiedAd ad = RPClassifiedAdsProcedures.GetClassifiedAd(input.Id);

            // assert player is owner of the RP add or else it is new
            if (me.MembershipId != ad.OwnerMembershipId && ad.Id > 0)
            {
                TempData["Error"] = "You do not own this RP Classified Ad.";
                return RedirectToAction("MyRPClassifiedAds", "Settings");
            }

            // assert player does not have too many ads out already
            if (RPClassifiedAdsProcedures.GetPlayerClassifiedAdCount(me) >= 3)
            {
                TempData["Error"] = "You already have the maximum number of RP Classified Ads posted per player.";
                TempData["SubError"] = "Wait a while for old postings to get automatically deleted or delete some of your own yourself.";
                return RedirectToAction("MyRPClassifiedAds", "Settings");
            }

            RPClassifiedAdsProcedures.SaveAd(input, me);

            TempData["Result"] = "RP classified ad successfully saved.";
            return RedirectToAction("MyRPClassifiedAds", "Settings");
        }


        [Authorize]
        public ActionResult DeleteRPClassifiedAd(int id)
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            RPClassifiedAd ad = RPClassifiedAdsProcedures.GetClassifiedAd(id);

            // assert player is owner of the RP add
            if (me.MembershipId != ad.OwnerMembershipId)
            {
                TempData["Error"] = "You do not own this RP Classified Ad.";
                return RedirectToAction("MyRPClassifiedAds", "Settings");
            }

            RPClassifiedAdsProcedures.DeleteAd(ad.Id);
            TempData["Result"] = "RP classified ad successfully deleted.";
            return RedirectToAction("MyRPClassifiedAds", "Settings");
        }

        [Authorize]
        public ActionResult ChaosChangeGameMode(int id)
        {

            if (!PvPStatics.ChaosMode)
            {
                TempData["Error"] = "You can only do this in chaos mode.";
                return RedirectToAction("Play", "PvP");
            }

            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);


            if (id < 0 || id > 2)
            {
                TempData["Error"] = "That is not a valid game mode.";
                return RedirectToAction("Play", "PvP");
            }

       
            PlayerProcedures.SetPvPFlag(me, id);

            TempData["Result"] = "Set to game mode " + id;
            return RedirectToAction("Play", "PvP");

        }

        [Authorize]
        public ActionResult ChaosRestoreBase()
        {

            if (!PvPStatics.ChaosMode)
            {
                TempData["Error"] = "You can only do this in chaos mode.";
                return RedirectToAction("Play", "PvP");
            }

            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            if (me.Form == me.OriginalForm)
            {
                TempData["Error"] = "You are already in your original form.";
                return RedirectToAction("Play", "PvP");
            }

            PlayerProcedures.InstantRestoreToBase(me);

            TempData["Result"] = "You have been restored to your base form.";
            return RedirectToAction("Play", "PvP");

        }

    }
}