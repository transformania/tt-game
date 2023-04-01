using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using Microsoft.AspNet.Identity;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Items.Queries;
using TT.Domain.Models;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;
using TT.Domain;
using TT.Domain.ClassifiedAds.Commands;
using TT.Domain.ClassifiedAds.DTOs;
using TT.Domain.ClassifiedAds.Queries;
using TT.Domain.Exceptions;
using TT.Domain.Exceptions.RPClassifiedAds;
using TT.Domain.Exceptions.Identity;
using TT.Domain.Identity.Commands;
using TT.Domain.Identity.Queries;
using TT.Domain.Players.Commands;
using TT.Web.ViewModels;
using TT.Domain.Skills.Queries;
using TT.Domain.Procedures.BossProcedures;

namespace TT.Web.Controllers
{
    [Authorize]
    public partial class SettingsController : Controller
    {

        public virtual ActionResult Settings()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            var output = new SettingsPageViewModel
            {
                TimeUntilReroll = Math.Round(RerollProcedures.GetTimeUntilReroll(me).TotalMinutes),
                TimeUntilLogout = TurnTimesStatics.GetOfflineAfterXMinutes() - Math.Floor(DateTime.UtcNow.Subtract(me.LastActionTimestamp).TotalMinutes),
                Player = me,
                PlayerItem = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer { PlayerId = me.Id }),
                Strikes = DomainRegistry.Repository.Find(new GetUserStrikes { UserId = myMembershipId }),
                ChaosChangesEnabled = DomainRegistry.Repository.FindSingle(new IsChaosChangesEnabled { UserId = myMembershipId }),
                OwnershipVisibilityEnabled = DomainRegistry.Repository.FindSingle(new IsOwnershipVisibilityEnabled { UserId = myMembershipId })
            };

            return View(MVC.Settings.Views.Settings, output);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult ChangeGameMode(int mode)
        {
            var myMembershipId = User.Identity.GetUserId();
            try
            {
                DomainRegistry.Repository.Execute(new ChangeGameMode
                {
                    MembershipId = myMembershipId,
                    GameMode = mode,
                    Force = PvPStatics.ChaosMode
                });

                var modeName = "";
                if (mode == (int)GameModeStatics.GameModes.Superprotection)
                    modeName = "SuperProtection";
                else if (mode == (int)GameModeStatics.GameModes.Protection)
                    modeName = "Protection";
                if (mode == (int)GameModeStatics.GameModes.PvP)
                    modeName = "PvP";

                TempData["Result"] = $"You have successfully changed your game to {modeName} mode.";
                return RedirectToAction(MVC.PvP.Play());
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(MVC.PvP.Play());
            }
        }

        public virtual ActionResult ChangeRPMode(bool inRP)
        {
            var myMembershipId = User.Identity.GetUserId();

            try
            {
                DomainRegistry.Repository.Execute(new ChangeRPMode { MembershipId = myMembershipId, InRPMode = inRP });
                TempData["Result"] = $"You have changed your game mode to RP mode: {inRP}";
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
            }

            return RedirectToAction(MVC.PvP.Play());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult ChangeHardmode()
        {
            var myMembershipId = User.Identity.GetUserId();

            try
            {
                DomainRegistry.Repository.Execute(new ChangeHardmode { MembershipId = myMembershipId, InHardmode = true });
                TempData["Result"] = $"You have changed your game mode to HARD MODE";
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
            }

            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult ChangeGender(int changeGender)
        {
            var myMembershipId = User.Identity.GetUserId();

            var Gender = "";

            if (changeGender == 0)
            {
                Gender = "male";
            }
            else if (changeGender == 1)
            {
                Gender = "female";
            }
            else
            {
                Gender = "other";
            }

            try
            {
                DomainRegistry.Repository.Execute(new ChangeGender { MembershipId = myMembershipId, changeGender = Gender });
                TempData["Result"] = $"You have changed your gender to {Gender}.";
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
            }

            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult SetBio()
        {
            var myMembershipId = User.Identity.GetUserId();
            var output = SettingsProcedures.GetSetBioViewModelFromMembershipId(myMembershipId);

            if (output.OwnerMembershipId != myMembershipId)
            {
                TempData["Error"] = "This is not your biography.";
                return RedirectToAction(MVC.PvP.Play());
            }

            return View(MVC.Settings.Views.SetBio, output);
        }

        public virtual ActionResult SetBioSend(SetBioViewModel input)
        {
            var myMembershipId = User.Identity.GetUserId();
            if (input.Text == null)
            {
                input.Text = "";
            }

            if (input.Tags == null)
            {
                input.Tags = "";
            }

            if (input.PublicVisibility > 1 || input.PublicVisibility < 0)
            {
                input.PublicVisibility = 0;
            }

            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            if (input.Text.Length > 2500 && !me.DonatorGetsMessagesRewards())
            {
                TempData["Error"] = "The text of your bio is too long (more than 2,500 characters).";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (input.Text.Length > 10000 && me.DonatorGetsMessagesRewards())
            {
                TempData["Error"] = "The text of your bio is too long (more than 10,000 characters).";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (input.WebsiteURL == null)
            {
                input.WebsiteURL = "";
            }

            if (input.Tags.Length > 1000)
            {
                TempData["Error"] = "Too many RP tags input text.";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (input.WebsiteURL.Length > 1500)
            {
                TempData["Error"] = "The text of your website URL is too long (more than 1,500 characters).";
                return RedirectToAction(MVC.PvP.Play());
            }

            SettingsProcedures.SavePlayerBio(input, myMembershipId);

            TempData["Result"] = "Your bio has been saved.";
            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult SetBioDelete(PlayerBio input)
        {
            var myMembershipId = User.Identity.GetUserId();
            SettingsProcedures.DeletePlayerBio(myMembershipId);

            TempData["Result"] = "Your bio has been deleted.";
            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult ViewBio(string id)
        {
            var player = PlayerProcedures.GetPlayerFromMembership(id);
            ViewBag.Name = player.GetFullName();

            var output = new BioPageViewModel();
            output.Player = player;
            output.PlayerBio = SettingsProcedures.GetPlayerBioFromMembershipId(id);
            
            if (output.PlayerBio == null)
            {
                TempData["Error"] = "It seems that this player has not written a player biography yet.";
                return RedirectToAction(MVC.PvP.Play());
            }
            else if (output.PlayerBio.PublicVisibility == 1 && !(User.IsInRole(PvPStatics.Permissions_Moderator) || User.IsInRole(PvPStatics.Permissions_Admin)))
            {
                TempData["Error"] = "It seems that this player is still working on their bio.";
                return RedirectToAction(MVC.PvP.Play());
            }

            output.Badges = StatsProcedures.GetPlayerBadges(player.MembershipId);
            output.IsMyBio = User.Identity.GetUserId() == player.MembershipId;

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

            return View(MVC.Settings.Views.ViewBio, output);
        }

        public virtual ActionResult DumpWillpower(string amount)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            if (me.Mobility != PvPStatics.MobilityFull)
            {
                TempData["Error"] = "You must be fully animate and in protection mode in order to drop your willpower.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player is not a duel
            if (me.InDuel > 0)
            {
                TempData["Error"] = "You must finish your duel before you can drop your willpower.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player is not a quest
            if (me.InQuest > 0)
            {
                TempData["Error"] = "You must finish your quest before you can drop your willpower.";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (me.GameMode == (int)GameModeStatics.GameModes.PvP)
            {
                TempData["Error"] = "You must be fully animate and in Protection or SuperProtection mode in order to drop your willpower.";
                return RedirectToAction(MVC.PvP.Play());
            }

            decimal drop = 0;
            if (amount == "half")
            {
                var halfHealth = me.MaxHealth / 2;
                if (halfHealth < me.Health)
                {
                    drop = me.Health - halfHealth;
                    PlayerProcedures.ChangePlayerActionMana(0, -drop, 0, me.Id, false);
                    TempData["Result"] = "You voluntarily lower your willpower down to half of its maximum, making yourself completely vulnerable to animate transformations.";
                    return RedirectToAction(MVC.PvP.Play());
                }
                else
                {
                    TempData["Error"] = "Your willpower is already lower than half of its maximum.";
                    return RedirectToAction(MVC.PvP.Play());
                }
            }
            else if (amount == PvPStatics.MobilityFull)
            {

                if (me.Health > 0)
                {
                    // drop = me.MaxHealth - me.Health;
                    PlayerProcedures.ChangePlayerActionMana(0, -me.Health, 0, me.Id, false);
                    TempData["Result"] = "You voluntarily decrease your willpower to nothing, making yourself vulnerable to any type of transformation.";
                    return RedirectToAction(MVC.PvP.Play());
                }
            }

            TempData["Error"] = "That is not a valid amount to decrease your willpower to.";
            return RedirectToAction(MVC.PvP.Play());


        }

        public virtual ActionResult SetNickname()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            if (!me.DonatorGetsNickname())
            {
                TempData["Error"] = "You are not marked as being a donator.";
                TempData["SubError"] = "This feature is reserved for players who pledge $7 monthly to support Transformania Time on Patreon.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var output = new SetNicknameViewModel
            {
                Nickname = me.Nickname
            };

            return View(MVC.Settings.Views.SetNickname, output);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult SetNicknameSend(SetNicknameViewModel input)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            if (me.DonatorLevel < 2)
            {
                TempData["Error"] = "You are not marked as a tier 2 or above donator.";
                TempData["SubError"] = "This feature is reserved for players who pledge $7 monthly to support Transformania Time on Patreon.";
                return RedirectToAction(MVC.PvP.Play());
            }


            if (input.Nickname != null && input.Nickname.Length > 20)
            {
                TempData["Error"] = "That nickname is too long. ";
                TempData["SubError"] = "Nicknames must be no longer than 20 characters.";
                return RedirectToAction(MVC.PvP.Play());
            }

            PlayerProcedures.SetNickname(input.Nickname, myMembershipId);

            TempData["Result"] = "Your new nickname has been set.";
            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult ToggleBlacklistOnPlayer(int id, int type)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var target = PlayerProcedures.GetPlayer(id);
            var getDate = DateTime.UtcNow;
            var note = "Added to blacklist on " + getDate + ".";

            // assert that this player is not a bot
            if (target.BotId < AIStatics.ActivePlayerBotId)
            {
                TempData["Error"] = "You cannot blacklist an AI character.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that this player has not been friended
            if (FriendProcedures.PlayerIsMyFriend(me, target))
            {
                TempData["Error"] = "You cannot blacklist one of your friends.";
                TempData["SubError"] = "Cancel your friendship with this player first.";
                return RedirectToAction(MVC.PvP.Play());
            }


            TempData["Result"] = BlacklistProcedures.TogglePlayerBlacklist(me, target, type, note);


            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult MyBlacklistEntries()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var output = BlacklistProcedures.GetMyBlacklistEntries(me);

            return View(MVC.Settings.Views.MyBlacklistEntries, output);
        }

        public virtual ActionResult ChangeBlacklistType(int id, int playerId, string type)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var target = PlayerProcedures.GetPlayer(playerId);


            // assert that this player is not a bot
            if (target.BotId < AIStatics.ActivePlayerBotId)
            {
                TempData["Error"] = "You cannot blacklist an AI character.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that this player owns this blacklist entry


            // assert that this player has not been friended
            if (FriendProcedures.PlayerIsMyFriend(me, target))
            {
                TempData["Error"] = "You cannot blacklist one of your friends.";
                TempData["SubError"] = "Cancel your friendship with this player first.";
                return RedirectToAction(MVC.PvP.Play());
            }

            TempData["Result"] = BlacklistProcedures.TogglePlayerBlacklistType(id, type, me, target);
            return RedirectToAction(MVC.PvP.Play());

        }

        public virtual ActionResult ChangeBlacklistNote(int blacklistId, int playerId)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var target = PlayerProcedures.GetPlayer(playerId);

            if (target == null)
            {
                TempData["Result"] = "This is not an active player.";
                return RedirectToAction(MVC.PvP.Play());
            }

            IBlacklistEntryRepository blacklistRepo = new EFBlacklistEntryRepository();
            var blacklist = blacklistRepo.BlacklistEntries.FirstOrDefault(b => b.Id == blacklistId && b.TargetMembershipId == target.MembershipId);

            var output = new BlacklistEntryViewModel
            {
                PlayerId = playerId,
                PlayerName = target.FirstName + " " + target.LastName,
                MembershipId = target.MembershipId,
                BlacklistId = blacklistId,
                Note = blacklist.Note,
            };


            return View(MVC.Settings.Views.ChangeBlacklistNote, output);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult ChangeBlacklistNoteSend(BlacklistEntryViewModel input)
        {

            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var target = PlayerProcedures.GetPlayer(input.PlayerId);

            IBlacklistEntryRepository blacklistRepo = new EFBlacklistEntryRepository();
            var blacklist = blacklistRepo.BlacklistEntries.FirstOrDefault(b => b.Id == input.BlacklistId && b.TargetMembershipId == input.MembershipId);

            if (target == null)
            {
                TempData["Result"] = "This is not an active player.";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (blacklist == null)
            {
                TempData["Result"] = "There is no blacklist entry with that ID." + input.dbBlacklistEntry.Id;
                return RedirectToAction(MVC.PvP.Play());
            }

            if (!ModelState.IsValid)
            {
                return View(MVC.Settings.Views.ChangeBlacklistNote, input);
            }

            blacklist.Note = input.Note;
            blacklistRepo.SaveBlacklistEntry(blacklist);

            PlayerLogProcedures.AddPlayerLog(me.Id, "<b>You have updated the blacklist note.</b>", true);

            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult ViewPolls()
        {
            return View(MVC.Settings.Views.ViewPolls);
        }

        public virtual ActionResult ViewPoll(int id)
        {
            var myMembershipId = User.Identity.GetUserId();
            var output = SettingsProcedures.LoadPoll(id, myMembershipId);
            // TODO: T4ize
            return View("Polls/Open/poll" + id, output);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult ReplyToPoll(PollEntry input)
        {
            var myMembershipId = User.Identity.GetUserId();
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Invalid input.";
                // TODO: T4ize
                return View("Polls/Open/poll" + input.PollId, input);
            }

            SettingsProcedures.SavePoll(input, 14, input.PollId, myMembershipId);
            TempData["Result"] = "Your response has been recorded.  Thanks for your participation!";
            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult PollResults(int id)
        {
            var output = SettingsProcedures.GetAllPollResults(id);
            // TODO: T4ize
            return View("Polls/Read/poll" + id, output);
        }

        public virtual ActionResult PollResultsClosed(int id)
        {
            // TODO: T4ize
            return View("Polls/Closed/poll" + id);
        }

        public virtual ActionResult SetChatColor(string color)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            var filename = AppDomain.CurrentDomain.BaseDirectory + "XMLs/validChatColors.txt";
            var text = System.IO.File.ReadAllText(filename);

            if (!text.Contains(color + ";"))
            {
                TempData["Error"] = "That is not a valid chat color.";
                return RedirectToAction(MVC.PvP.Play());
            }

            PlayerProcedures.SetChatColor(me, color);
            TempData["Result"] = "Your chat color has been set to " + color + ".";
            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult WriteAuthorArtistBio()
        {
            var myMembershipId = User.Identity.GetUserId();
            // assert player has on the artist whitelist
            if (!User.IsInRole(PvPStatics.Permissions_Artist))
            {
                TempData["Error"] = "You are not eligible to do this at this time.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var output = SettingsProcedures.GetAuthorArtistBio(myMembershipId);
            return View(MVC.Settings.Views.WriteAuthorArtistBio, output);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult WriteAuthorArtistSend(AuthorArtistBio input)
        {
            var myMembershipId = User.Identity.GetUserId();
            // assert player has on the artist whitelist
            if (!User.IsInRole(PvPStatics.Permissions_Artist))
            {
                TempData["Error"] = "You are not eligible to do this at this time.";
                return RedirectToAction(MVC.PvP.Play());
            }

            SettingsProcedures.SaveAuthorArtistBio(input, myMembershipId);

            TempData["Result"] = "Your artist bio has been saved!";
            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult AuthorArtistBio(string id)
        {
            var myMembershipId = User.Identity.GetUserId();
            var output = SettingsProcedures.GetAuthorArtistBio(id);
            var artistIngamePlayer = PlayerProcedures.GetPlayerFromMembership(id);
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            ViewBag.IngameCharacter = "This artist does not currently have a character ingame.";

            var friends = FriendProcedures.MemberIsMyFriend(id, myMembershipId);
            ViewBag.IAmFriendsWithArtist = friends;

            if (artistIngamePlayer != null)
            {
                ViewBag.IngameCharacter = "This artist current has a character under the name of " + artistIngamePlayer.GetFullName() + ".";
            }
            // assert visibility setting is okay
            if (output.PlayerNamePrivacyLevel == 1 && !friends)
            {
                TempData["Error"] = "This artist bio is only visible to his or her friends.";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (output.PlayerNamePrivacyLevel == 2)
            {
                TempData["Error"] = "This artist's biography is currently entirely disabled.  Check again later.";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (output.Text != null)
            {
                output.Text = output.Text.Replace("[br]", "<br>").Replace("[p]", "<p>").Replace("[/p]", "</p>").Replace("[h1]", "<h1>").Replace("[/h1]", "</h1>").Replace("[h2]", "<h2>").Replace("[/h2]", "</h2>").Replace("[h3]", "<h3>").Replace("[/h3]", "</h3>");
            }

            return View(MVC.Settings.Views.AuthorArtistBio, output);
        }

        /// <summary>
        /// Allows a player to claim a new base form if they have earned one.
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult SetBaseForm(int baseId)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            if (baseId == me.OriginalFormSourceId)
            {
                TempData["Error"] = "You already have this form selected as your base.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // Prevent switching to existing form as a means of quickly escaping a TG orb's effects
            if (baseId == me.FormSourceId && me.Mobility == PvPStatics.MobilityFull)
            {
                TempData["Error"] = "You cannot select your current form as your base form.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // Look for a match among custom forms
            IContributorCustomFormRepository repo = new EFContributorCustomFormRepository();
            var match = repo.ContributorCustomForms.Where(c => c.OwnerMembershipId == myMembershipId &&
                                                                c.CustomForm.Id == baseId)
                                                   .Select(c => c.CustomForm).FirstOrDefault();

            if (match == null)
            {
                // Look for a match among standard starter forms
                var formRepo = new EFDbStaticFormRepository();
                match = formRepo.DbStaticForms.Where(f => f.Id == baseId &&
                        (f.FriendlyName == "Regular Guy" || f.FriendlyName == "Regular Girl")).FirstOrDefault();

                if (match == null)
                {
                    TempData["Error"] = "You do not own that custom base form.";
                    return RedirectToAction(MVC.PvP.Play());
                }
            }

            var instant = false;

            // When player is already in their base form change them instantly.
            if (me.FormSourceId == me.OriginalFormSourceId)
            {
                PlayerProcedures.SetCustomBase(me, baseId);
                me.OriginalFormSourceId = baseId;
                PlayerProcedures.InstantRestoreToBase(me);
                instant = true;
            }
            else
            {
                PlayerProcedures.SetCustomBase(me, baseId);
            }

            if (me.Mobility == PvPStatics.MobilityFull)
            {
                if (instant)
                {
                    TempData["Result"] = $"You are suddenly overwhelmed as you spontaneously transform into a {match.FriendlyName}!";
                }
                else
                {
                    TempData["Result"] = $"Your inner {match.FriendlyName} is begging to be let out, if only you could return yourself to base form...";
                }
            }
            else
            {
                TempData["Result"] = $"Your dreams of becoming a {match.FriendlyName} could come true, if only you could return to animacy...";
            }

            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult MyBaseForms()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            var customRepo = new EFContributorCustomFormRepository();
            var customForms = customRepo.ContributorCustomForms.Where(c => c.OwnerMembershipId == myMembershipId)
                                                               .Select(c => c.CustomForm).ToList();

            var formRepo = new EFDbStaticFormRepository();
            var baseForms = formRepo.DbStaticForms.Where(f => f.FriendlyName == "Regular Guy" || f.FriendlyName == "Regular Girl").ToArray();

            // Shuffle non-custom base forms so players don't always pick the first one
            var rand = new Random();
            for(var backstop = baseForms.Length; backstop > 1; backstop--)
            {
                var dest = backstop - 1;
                var src = rand.Next(0, backstop);
                var temp = baseForms[dest];
                baseForms[dest] = baseForms[src];
                baseForms[src] = temp;
            }

            if (me.Mobility == PvPStatics.MobilityFull)
            {
                ViewBag.CurrentForm = me.FormSourceId;
            }
            ViewBag.CurrentBaseForm = me.OriginalFormSourceId;

            if (!customForms.Any(f => f.Id == me.OriginalFormSourceId))
            {
                var currentBase = formRepo.DbStaticForms.FirstOrDefault(f => f.Id == me.OriginalFormSourceId);

                if (currentBase != null)
                {
                    customForms.Add(currentBase);
                }
            }

            customForms.AddRange(baseForms);

            return View(MVC.Settings.Views.MyBaseForms, customForms);
        }

        public virtual ActionResult ArchiveSpell(int skillSourceId)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            // assert that player does own this skill
            var skill = SkillProcedures.GetSkillViewModel(skillSourceId, me.Id);

            if (skill == null)
            {
                TempData["Error"] = "You don't know this spell yet.";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (skill.dbSkill.SkillSourceId == PvPStatics.Spell_WeakenId)
            {
                ViewBag.Message = $"You can't archive the {skill.StaticSkill.FriendlyName} skill.";
                return PartialView(MVC.Settings.Views.partial.ArchiveNotice);
            }

            if (skill.dbSkill.Bookmarked)
            {
                ViewBag.Message = $"You cannot archive a bookmarked spell.";
                return PartialView(MVC.Settings.Views.partial.ArchiveNotice);
            }

            SkillProcedures.ArchiveSpell(skill.dbSkill.Id);

            if (!skill.dbSkill.IsArchived)
            {
                ViewBag.Message = "You have successfully archived " + skill.StaticSkill.FriendlyName + ".";
            }
            else
            {
                ViewBag.Message = "You have successfully restored " + skill.StaticSkill.FriendlyName + " from your spell archive.";
            }
            ViewBag.Number = skill.dbSkill.Id;
            return PartialView(MVC.Settings.Views.partial.ArchiveNotice);
        }

        public virtual ActionResult ArchiveAllMySpells(string archive)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            if (archive == "true")
            {
                SkillProcedures.ArchiveAllSpells(me.Id, true);
                TempData["Result"] = "You have archived all of your known non-bookmarked spells.  They will not appear on the attack modal until you unarchive them.";
            }
            else
            {
                SkillProcedures.ArchiveAllSpells(me.Id, false);
                TempData["Result"] = "You have unarchived all of your known non-bookmarked spells.  They will all now appear on the attack modal again.";
            }


            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult BookmarkSpell(int skillSourceId)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            // assert that player does own this skill
            var skill = SkillProcedures.GetSkillViewModel(skillSourceId, me.Id);
            
            ISkillRepository skillRepo = new EFSkillRepository();
            var countBookmark = skillRepo.Skills.Count(s => s.Bookmarked);

            if (skill == null)
            {
                TempData["Result"] = "You don't know this spell yet.";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (skill.dbSkill.IsArchived)
            {
                TempData["Result"] = "You cannot bookmark an archived spell.";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (skill.dbSkill.Bookmarked)
            {
                SkillProcedures.BookmarkSpell(skill.dbSkill.Id);
                TempData["Result"] = "You have successfully removed " + skill.StaticSkill.FriendlyName + " to your spell bookmark.";
            }
            else if(skillSourceId != skill.dbSkill.Id && countBookmark > 2)
            {
                TempData["Result"] = "You can only bookmark three spells.";
            }
            else if (!skill.dbSkill.Bookmarked)
            {
                SkillProcedures.BookmarkSpell(skill.dbSkill.Id);
                TempData["Result"] = "You have successfully added " + skill.StaticSkill.FriendlyName + " from your spell bookmark.";
            }

            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult PlayerStats(string id)
        {
            var player = PlayerProcedures.GetPlayerFromMembership(id);
            ViewBag.Name = player.GetFullName();
            ViewBag.PlayerId = player.Id;
            var output = DomainRegistry.Repository.Find(new GetPlayerStats { OwnerId = player.MembershipId });
            return View(MVC.Settings.Views.PlayerStats, output);
        }

        public virtual ActionResult PlayerStatsTopOfType(string type)
        {
            var output = StatsProcedures.GetLeaderPlayersInStat(type);
            return PartialView(MVC.Settings.Views.partial.PlayerStatsTopOfType, output);
        }

        public virtual ActionResult SetFriendNickname(int id)
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            var friend = FriendProcedures.GetFriend(id);

            if (friend.OwnerMembershipId != me.MembershipId && friend.FriendMembershipId != me.MembershipId)
            {
                TempData["Error"] = "This player is not a friend with you.";
                return RedirectToAction(MVC.PvP.MyFriends());
            }

            var output = new SetFriendNicknameViewModel
            {
                Nickname = friend.OwnerMembershipId == me.MembershipId ? friend.OwnerNicknameForFriend : friend.FriendNicknameForOwner,
                FriendshipId = friend.Id
            };

            return View(MVC.Settings.Views.SetFriendNickname, output);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult SetFriendNicknameSend(SetFriendNicknameViewModel input)
        {
            var myMembershipId = User.Identity.GetUserId();
            if (input.Nickname == null)
            {
                input.Nickname = "";
            }
            input.Nickname = input.Nickname.Trim();

            // asset the nickname falls within an appropriate range
            if (input.Nickname.Length == 0)
            {
                input.Nickname = "[UNASSIGNED]";
            }
            else if (input.Nickname.Length > PvPStatics.FriendNicknameMaxLength)
            {
                TempData["Error"] = $"Friend nicknames must be {PvPStatics.FriendNicknameMaxLength} characters or less.";
                return RedirectToAction(MVC.PvP.MyFriends());
            }

            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var friend = FriendProcedures.GetFriend(input.FriendshipId);

            if (friend.OwnerMembershipId != me.MembershipId && friend.FriendMembershipId != me.MembershipId)
            {
                TempData["Error"] = "This player is not a friend with you.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // set the nickname based on whether the current player is the owner or the friend
            if (friend.OwnerMembershipId == me.MembershipId)
            {
                TempData["Result"] = FriendProcedures.OwnerSetNicknameOfFriend(friend.Id, input.Nickname);
            }
            else if (friend.FriendMembershipId == me.MembershipId)
            {
                TempData["Result"] = FriendProcedures.FriendSetNicknameOfOwner(friend.Id, input.Nickname);
            }

            return RedirectToAction(MVC.PvP.MyFriends());
        }

        public virtual ActionResult MyRPClassifiedAds()
        {
            var userId = User.Identity.GetUserId();
            var output = DomainRegistry.Repository.Find(new GetUserRPClassifiedAds() { UserId = userId });

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            return View(MVC.Settings.Views.MyRPClassifiedAds, output);
        }

        public virtual ActionResult CreateRPClassifiedAd()
        {
            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];
            ViewBag.Edit = false;

            return View(MVC.Settings.Views.CreateOrUpdateRPClassifiedAd, TempData["input"] ?? new RPClassifiedAdDetail());
        }

        public virtual ActionResult UpdateRPClassifiedAd(int id)
        {
            var userId = User.Identity.GetUserId();
            RPClassifiedAdDetail ad;

            try
            {
                ad = DomainRegistry.Repository.FindSingle(new GetRPClassifiedAd() { RPClassifiedAdId = id, UserId = userId });
            }
            catch (RPClassifiedAdException ex)
            when (ex is RPClassifiedAdNotOwnerException ||
                  ex is RPClassifiedAdNotFoundException)
            {
                TempData["Error"] = ex.UserFriendlyError ?? ex.Message;
                TempData["SubError"] = ex.UserFriendlySubError;
                return RedirectToAction(MVC.Settings.MyRPClassifiedAds());
            }

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];
            ViewBag.Edit = true;

            return View(MVC.Settings.Views.CreateOrUpdateRPClassifiedAd, TempData["input"] ?? ad);
        }

        public virtual ActionResult RefreshRPClassifiedAd(int id)
        {
            var userId = User.Identity.GetUserId();

            try
            {
                DomainRegistry.Repository.Execute(new RefreshRPClassifiedAd() { RPClassifiedAdId = id, UserId = userId });
            }
            catch (RPClassifiedAdException ex)
            when (ex is RPClassifiedAdNotOwnerException ||
                  ex is RPClassifiedAdNotFoundException)
            {
                TempData["Error"] = ex.UserFriendlyError ?? ex.Message;
                TempData["SubError"] = ex.UserFriendlySubError;
                return RedirectToAction(MVC.Settings.MyRPClassifiedAds());
            }

            TempData["Result"] = "RP classified ad successfully refreshed.";
            return RedirectToAction(MVC.Settings.MyRPClassifiedAds());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult CreateRPClassifiedAd(RPClassifiedAdDetail input)
        {
            input.SetNullsToEmptyStrings();
            var userId = User.Identity.GetUserId();

            try
            {
                DomainRegistry.Repository.Execute(new CreateRPClassifiedAd()
                {
                    UserId = userId,
                    Title = input.Title,
                    Text = input.Text,
                    YesThemes = input.YesThemes,
                    NoThemes = input.NoThemes,
                    PreferredTimezones = input.PreferredTimezones
                });
            }
            catch (RPClassifiedAdInvalidInputException ex)
            {
                TempData["input"] = input;
                TempData["Error"] = ex.UserFriendlyError ?? ex.Message;
                TempData["SubError"] = ex.UserFriendlySubError;
                return RedirectToAction(MVC.Settings.CreateRPClassifiedAd());
            }
            catch (RPClassifiedAdLimitException ex)
            {
                TempData["Error"] = ex.UserFriendlyError ?? ex.Message;
                TempData["SubError"] = ex.UserFriendlySubError;
                return RedirectToAction(MVC.Settings.MyRPClassifiedAds());
            }
            catch (UserNotFoundException ex)
            {
                TempData["Error"] = ex.UserFriendlyError ?? ex.Message;
                TempData["SubError"] = ex.UserFriendlySubError;
                return RedirectToAction(MVC.PvP.Play());
            }

            TempData["Result"] = "RP classified ad successfully created.";
            return RedirectToAction(MVC.Settings.MyRPClassifiedAds());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult UpdateRPClassifiedAd(RPClassifiedAdDetail input)
        {
            var userId = User.Identity.GetUserId();
            try
            {
                DomainRegistry.Repository.Execute(new UpdateRPClassifiedAd()
                {
                    RPClassifiedAdId = input.Id,
                    UserId = userId,

                    Title = input.Title,
                    Text = input.Text,
                    YesThemes = input.YesThemes,
                    NoThemes = input.NoThemes,
                    PreferredTimezones = input.PreferredTimezones
                });
            }
            catch (RPClassifiedAdInvalidInputException ex)
            {
                TempData["input"] = input;
                TempData["Error"] = ex.UserFriendlyError ?? ex.Message;
                TempData["SubError"] = ex.UserFriendlySubError;
                return RedirectToAction(MVC.Settings.UpdateRPClassifiedAd());
            }
            catch (RPClassifiedAdException ex)
            when (ex is RPClassifiedAdNotOwnerException ||
                  ex is RPClassifiedAdNotFoundException)
            {
                TempData["Error"] = ex.UserFriendlyError ?? ex.Message;
                TempData["SubError"] = ex.UserFriendlySubError;
                return RedirectToAction(MVC.Settings.MyRPClassifiedAds());
            }

            TempData["Result"] = "RP classified ad successfully updated.";
            return RedirectToAction(MVC.Settings.MyRPClassifiedAds());
        }

        public virtual ActionResult DeleteRPClassifiedAd(int id)
        {
            var userId = User.Identity.GetUserId();

            try
            {
                DomainRegistry.Repository.Execute(new DeleteRPClassifiedAd() { RPClassifiedAdId = id, UserId = userId });
            }
            catch (RPClassifiedAdException ex)
            when (ex is RPClassifiedAdNotFoundException ||
                  ex is RPClassifiedAdNotOwnerException)
            {
                TempData["Error"] = ex.UserFriendlyError ?? ex.Message;
                TempData["SubError"] = ex.UserFriendlySubError;
                return RedirectToAction(MVC.Settings.MyRPClassifiedAds());
            }

            TempData["Result"] = "RP classified ad successfully deleted.";
            return RedirectToAction(MVC.Settings.MyRPClassifiedAds());
        }

        public virtual ActionResult ChaosRestoreBase(int option)
        {

            if (!PvPStatics.ChaosMode)
            {
                TempData["Error"] = "You can only do this in chaos mode.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // Get the player's membershipID
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            var restoreForm = (option == 0 || option == 2);
            var restoreName = (option == 1 || option == 2);

            var inBaseForm = me.FormSourceId == me.OriginalFormSourceId;
            var hasOriginalName = me.IsUsingOriginalName();

            if (restoreForm && inBaseForm && (!restoreName || hasOriginalName))
            {
                TempData["Error"] = "You are already in your original form.";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (restoreName && hasOriginalName && !restoreForm)
            {
                TempData["Error"] = "This is the only name you can remember having.";
                return RedirectToAction(MVC.PvP.Play());
            }

            PlayerProcedures.InstantRestoreToBase(me, restoreForm: restoreForm, restoreName: restoreName);

            TempData["Result"] = "You have chosen to restore parts of yourself to normal.";
            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult AllowChaosChanges(bool allowChanges)
        {

            if (!PvPStatics.ChaosMode)
            {
                TempData["Error"] = "You can only do this in chaos mode.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var myMembershipId = User.Identity.GetUserId();

            try
            {
                DomainRegistry.Repository.Execute(new AllowChaosChanges
                {
                    UserId = myMembershipId,
                    ChaosChangesEnabled = allowChanges
                });
                TempData["Result"] = $"Allowing chaos changes has been successfully set to {allowChanges}.";
            }
            catch (DomainException)
            {
                TempData["Error"] = "Failed to change chaos changes enabled/disabled.";
            }

            return RedirectToAction(MVC.PvP.Play());

        }

        public virtual ActionResult AllowOwnershipVisibility(bool allowSearch)
        {

            var myMembershipId = User.Identity.GetUserId();

            try
            {
                DomainRegistry.Repository.Execute(new SetOwnershipVisibility
                {
                    UserId = myMembershipId,
                    OwnershipVisibilityEnabled = allowSearch
                });
                TempData["Result"] = $"Ownership visibility has been successfully set to {allowSearch}.";
            }
            catch (DomainException)
            {
                TempData["Error"] = "Failed to change ownership visibility enabled/disabled.";
            }

            return RedirectToAction(MVC.PvP.Play());

        }

        public virtual ActionResult SetArtistBioVisibility(bool isLive)
        {
            if (!User.IsInRole(PvPStatics.Permissions_Artist))
            {
                TempData["Error"] = "You are not on the artist whitelist.";
                return RedirectToAction(MVC.PvP.Play());
            }

            try
            {
                var result = DomainRegistry.Repository.Execute(new SetArtistBioVisibility
                {
                    UserId = User.Identity.GetUserId(),
                    IsVisible = isLive
                });
                TempData["Result"] = result;
                return RedirectToAction(MVC.PvP.Play());
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(MVC.PvP.Play());
            }
        }
        
        //Learn all spells that the Lorekeeper can sell
        public virtual ActionResult LearnAnimateSpells()
        {

            if (!PvPStatics.ChaosMode)
            {
                TempData["Error"] = "You can only do this in chaos mode.";
                return RedirectToAction(MVC.PvP.Play());
            }

            try
            {
                var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
                SkillProcedures.GiveAllSkillsToPlayer(me.Id, PvPStatics.MobilityFull);
            }
            catch (DomainException)
            {
                TempData["Error"] = "Failed to learn all animate spells";
                return RedirectToAction(MVC.PvP.Play());
            }
            catch (Exception)
            {
                TempData["Error"] = "You already know every spell of this type";
                return RedirectToAction(MVC.PvP.Play());
            }

            TempData["Result"] = "You have successfully learned every animate spell";
            return RedirectToAction(MVC.PvP.Play());

        }

        //Learn all inanimate spells that the Lorekeeper can sell
        public virtual ActionResult LearnInanimateSpells()
        {

            if (!PvPStatics.ChaosMode)
            {
                TempData["Error"] = "You can only do this in chaos mode.";
                return RedirectToAction(MVC.PvP.Play());
            }

            try
            {
                var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
                SkillProcedures.GiveAllSkillsToPlayer(me.Id, PvPStatics.MobilityInanimate);
            }
            catch (DomainException)
            {
                TempData["Error"] = "Failed to learn all inanimate spells";
                return RedirectToAction(MVC.PvP.Play());
            }
            catch (Exception)
            {
                TempData["Error"] = "You already know every spell of this type";
                return RedirectToAction(MVC.PvP.Play());
            }

            TempData["Result"] = "You have successfully learned every inanimate spell";
            return RedirectToAction(MVC.PvP.Play());

        }

        //Learn all pet spells that the Lorekeeper can sell
        public virtual ActionResult LearnPetSpells()
        {

            if (!PvPStatics.ChaosMode)
            {
                TempData["Error"] = "You can only do this in chaos mode.";
                return RedirectToAction(MVC.PvP.Play());
            }

            try
            {
                var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
                SkillProcedures.GiveAllSkillsToPlayer(me.Id, PvPStatics.MobilityPet);
            }
            catch (DomainException)
            {
                TempData["Error"] = "Failed to learn all pet spells";
                return RedirectToAction(MVC.PvP.Play());
            }
            catch (Exception)
            {
                TempData["Error"] = "You already know every spell of this type";
                return RedirectToAction(MVC.PvP.Play());
            }

            TempData["Result"] = "You have successfully learned every pet spell";
            return RedirectToAction(MVC.PvP.Play());

        }

        //Forget all spells the user currently has, then give them Weaken as a base
        public virtual ActionResult ForgetAllSpells()
        {

            if (!PvPStatics.ChaosMode)
            {
                TempData["Error"] = "You can only do this in chaos mode.";
                return RedirectToAction(MVC.PvP.Play());
            }

            try
            {
                var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
                ForgetSpells(me.Id);
            }
            catch (DataException)
            {
                TempData["Error"] = "You have no spells to forget";
                return RedirectToAction(MVC.PvP.Play());
            }
            catch
            {
                TempData["Error"] = "Failed to forget all spells";
                return RedirectToAction(MVC.PvP.Play());
            }

            TempData["Result"] = "You have successfully forgotten all spells";
            return RedirectToAction(MVC.PvP.Play());

        }

        //Helper function for forgetting spells via chaos
        public virtual void ForgetSpells(int playerId)
        {
            //Get the list of spells that should be forgettable
            var skills = SkillProcedures.GetSkillViewModelsOwnedByPlayer(playerId)
                .Where(s => s.StaticSkill.IsPlayerLearnable &&
                !s.StaticSkill.ExclusiveToFormSourceId.HasValue &&
                !s.StaticSkill.ExclusiveToItemSourceId.HasValue &&
                s.StaticSkill.Id != PvPStatics.Spell_WeakenId &&
                s.StaticSkill.Id != PvPStatics.Dungeon_VanquishSpellSourceId &&
                s.StaticSkill.Id != BossProcedures_FaeBoss.SpellUsedAgainstNarcissaSourceId &&
                (s.StaticSkill.LearnedAtLocation ?? s.StaticSkill.LearnedAtRegion) != null);

            if (skills.IsEmpty())
            {
                throw new DataException("You have no spells to forget");
            }

            //Remove the skills
            ISkillRepository skillRepo = new EFSkillRepository();
            skillRepo.DeleteSkillList(skills);

        }
    }
}
