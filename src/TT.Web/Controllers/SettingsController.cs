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
                TimeUntilLogout = PvPStatics.OfflineAfterXMinutes - Math.Abs(Math.Floor(me.LastActionTimestamp.Subtract(DateTime.UtcNow).TotalMinutes)),
                Player = me,
                Strikes = DomainRegistry.Repository.Find(new GetUserStrikes { UserId = myMembershipId })
            };

            return View(MVC.Settings.Views.Settings, output);
        }

        public virtual ActionResult ChangeGameMode(int mode)
        {
            var myMembershipId = User.Identity.GetUserId();
            try
            {
                DomainRegistry.Repository.Execute(new ChangeGameMode
                {
                    MembershipId = myMembershipId,
                    GameMode = mode,
                    InChaos = PvPStatics.ChaosMode
                });

                var modeName = "";
                if (mode == GameModeStatics.SuperProtection)
                    modeName = "SuperProtection";
                else if (mode == GameModeStatics.Protection)
                    modeName = "Protection";
                if (mode == GameModeStatics.PvP)
                    modeName = "PvP";

                TempData["Result"] = $"You have successfully change your game to {modeName} mode.";
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
            output.PlayerBio = SettingsProcedures.GetPlayerBioFromMembershipId(id);
            if (output.PlayerBio == null)
            {
                TempData["Error"] = "It seems that this player has not written a player biography yet.";
                return RedirectToAction(MVC.PvP.Play());
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

            if (me.GameMode == GameModeStatics.PvP)
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
                    PlayerProcedures.ChangePlayerActionManaNoTimestamp(0, -drop, 0, me.Id);
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
                    PlayerProcedures.ChangePlayerActionManaNoTimestamp(0, -me.Health, 0, me.Id);
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

        public virtual ActionResult ToggleBlacklistOnPlayer(int id)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var target = PlayerProcedures.GetPlayer(id);

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


            TempData["Result"] = BlacklistProcedures.TogglePlayerBlacklist(me, target);


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
        /// Allows a player to claim a new base form if they have earned one through being a contributor or artist.  Multiple custom forms can be toggled through by clicking the link mulitple times; each click will advance to the next available form and upon reaching the final form loop back to the first one.
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult UseMyCustomForm()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            IContributorCustomFormRepository repo = new EFContributorCustomFormRepository();
            var customForms = repo.ContributorCustomForms.Where(c => c.OwnerMembershipId == myMembershipId).ToList();

            if (!customForms.Any())
            {
                TempData["Error"] = "You do not have any custom base forms.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var newForm = customForms.First();

            var index = 0;
            foreach (var c in customForms)
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
                me.OriginalForm = newForm.CustomForm.dbName;
                PlayerProcedures.InstantRestoreToBase(me);
            }
            else
            {
                PlayerProcedures.SetCustomBase(me, newForm.CustomForm.dbName);
            }


            TempData["Result"] = "Your custom form has been set.";
            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult ArchiveSpell(string name)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            // assert that player does own this skill
            var skill = SkillProcedures.GetSkillViewModel(name, me.Id);

            if (skill == null)
            {
                TempData["Error"] = "You don't know this spell yet.";
                return RedirectToAction(MVC.PvP.Play());
            }

            SkillProcedures.ArchiveSpell(skill.dbSkill.Id);

            if (!skill.dbSkill.IsArchived)
            {
                ViewBag.Message = "You have successfully archived " + skill.Skill.FriendlyName + ".";
            }
            else
            {
                ViewBag.Message = "You have successfully restored " + skill.Skill.FriendlyName + " from your spell archive.";
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
                TempData["Result"] = "You have archived all of your known spells.  They will not appear on the attack modal until you unarchive them.";
            }
            else
            {
                SkillProcedures.ArchiveAllSpells(me.Id, false);
                TempData["Result"] = "You have unarchived all of your known spells.  They will all now appear on the attack modal again.";
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

        public virtual ActionResult ChaosRestoreBase()
        {

            if (!PvPStatics.ChaosMode)
            {
                TempData["Error"] = "You can only do this in chaos mode.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            if (me.Form == me.OriginalForm)
            {
                TempData["Error"] = "You are already in your original form.";
                return RedirectToAction(MVC.PvP.Play());
            }

            PlayerProcedures.InstantRestoreToBase(me);

            TempData["Result"] = "You have been restored to your base form.";
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
    }
}