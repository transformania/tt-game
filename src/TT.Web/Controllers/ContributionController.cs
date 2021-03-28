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
using Newtonsoft.Json;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Forms.Commands;
using TT.Domain.Skills.Commands;
using TT.Domain.Identity.Queries;

namespace TT.Web.Controllers
{

    public partial class ContributionController : Controller
    {
        public virtual ActionResult ProofreadingContributions()
        {
            IContributionRepository contributionRepo = new EFContributionRepository();
            var proofreading = contributionRepo.Contributions.Where(c => c.AdminApproved && c.ProofreadingCopy).Select(
                i => new ProofreadingContributionsViewModel()
                {
                    Id = i.Id,
                    CheckedOutBy = i.CheckedOutBy,
                    CreationTimestamp = i.CreationTimestamp,
                    IsLive = i.IsLive,
                    NeedsToBeUpdated = i.NeedsToBeUpdated,
                    ProofreadingCopy = i.ProofreadingCopy,
                    ProofreadingLockIsOn = i.ProofreadingLockIsOn,
                    Form_FriendlyName = i.Form_FriendlyName,
                    Skill_FriendlyName = i.Skill_FriendlyName
                }
                );

            // add the rest of the submitted contributions if the player is a proofread
            if (!User.IsInRole(PvPStatics.Permissions_Proofreader))
            {
                return new JsonResult();
            }

            return Content(JsonConvert.SerializeObject(proofreading, Formatting.Indented), "application/json");
        }

        [Authorize]
        public virtual ActionResult Contribute()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            var model = new ContributePageViewModel
            {
                Me = me,
                HasPublicArtistBio = SettingsProcedures.PlayerHasArtistAuthorBio(myMembershipId),
                ArtistBios = DomainRegistry.Repository.Find(new GetArtistBios())
            };

            return View(MVC.Contribution.Views.Contribute, model);
        }

        [Authorize]
        public virtual ActionResult ContributeSpell(int Id = -1)
        {

            IContributionRepository contributionRepo = new EFContributionRepository();

            var currentUserId = User.Identity.GetUserId();

            IEnumerable<Contribution> myContributions = contributionRepo.Contributions.Where(c => c.OwnerMembershipId == currentUserId);

            var iAmProofreader = User.IsInRole(PvPStatics.Permissions_Proofreader);

            Contribution contribution;

            if (Id != -1)
            {
                // contribution = myContributions.FirstOrDefault(c => c.Id == Id);
                contribution = contributionRepo.Contributions.FirstOrDefault(c => c.Id == Id);
                if (contribution != null)
                {
                    ViewBag.Result = "Load successful.";

                    // assert player owns this
                    if (contribution.OwnerMembershipId != currentUserId && !iAmProofreader)
                    {
                        TempData["Error"] = "This contribution does not belong to your account.";
                        return RedirectToAction(MVC.PvP.Play());
                    }

                    // if this player is a proofreader and this contribution is not marked as ready for proofreading, tell the editor to go to the proofreading version instead.
                    if (iAmProofreader && !contribution.ProofreadingCopy)
                    {
                        var contributionProofed = contributionRepo.Contributions.FirstOrDefault(c => c.OwnerMembershipId == contribution.OwnerMembershipId && c.ProofreadingCopy && c.Skill_FriendlyName == contribution.Skill_FriendlyName && c.Form_FriendlyName == contribution.Form_FriendlyName);
                        if (contributionProofed != null)
                        {
                            TempData["Error"] = "There is already a proofreading version of this available.  Please load that instead.";
                            return RedirectToAction(MVC.PvP.Play());
                        }
                    }

                    // save the proofreading lock on this contribution
                    if (contribution.ProofreadingCopy)
                    {
                        contribution.ProofreadingLockIsOn = true;
                        contribution.CheckedOutBy = User.Identity.Name;
                        contribution.CreationTimestamp = DateTime.UtcNow;
                        contributionRepo.SaveContribution(contribution);
                    }
                }
                else
                {
                    TempData["Error"] = "Contribution not found.";
                    return RedirectToAction(MVC.PvP.Play());
                }
            }
            else
            {
                contribution = new Contribution
                {
                    Skill_ManaCost = 7,
                    Form_TFEnergyRequired = 100,
                    Skill_TFPointsAmount = 10,
                    Skill_HealthDamageAmount = 50,
                    Skill_IsPlayerLearnable = true
                };
            }

            ViewBag.Result = TempData["Result"];

            ViewBag.OtherContributions = myContributions;

            var bbox = new BalanceBox();
            bbox.LoadBalanceBox(contribution);
            var balance = bbox.GetBalance();
            ViewBag.BalanceScore = balance;


            #region for admin use only, see if statics exist
            if (User.IsInRole(PvPStatics.Permissions_Admin) && contribution.ProofreadingCopy)
            {
                IDbStaticSkillRepository skillRepo = new EFDbStaticSkillRepository();
                IDbStaticFormRepository formRepo = new EFDbStaticFormRepository();
                IDbStaticItemRepository itemRepo = new EFDbStaticItemRepository();

                var staticSkill = skillRepo.DbStaticSkills.FirstOrDefault(s => s.Id == contribution.SkillSourceId);
                var staticForm = formRepo.DbStaticForms.FirstOrDefault(f => f.Id == contribution.FormSourceId);
                var staticItem = itemRepo.DbStaticItems.FirstOrDefault(f => f.Id == contribution.ItemSourceId);

                if (staticSkill == null)
                {
                    ViewBag.StaticSkillExists = "<p class='bad'>No static skill found.</p>";
                }
                else
                {
                    ViewBag.StaticSkillExists += $"<p class='good'>Static skill found: {staticSkill.FriendlyName} </p>";
                }

                if (staticForm == null)
                {
                    ViewBag.StaticFormExists = "<p class='bad'>No static form found.</p>";
                }
                else
                {
                    ViewBag.StaticFormExists = $"<p class='good'>Static form found: {staticForm.FriendlyName}</p>";
                }

                if (staticItem == null && (contribution.Form_MobilityType == PvPStatics.MobilityInanimate || contribution.Form_MobilityType == PvPStatics.MobilityPet))
                {
                    ViewBag.StaticItemExists += "<p class='bad'>No static item/pet found.</p>";
                }
                else if (contribution.Form_MobilityType == PvPStatics.MobilityInanimate || contribution.Form_MobilityType == PvPStatics.MobilityPet)
                {
                    ViewBag.StaticItemExists += $"<p class='good'>Static item/pet found:  {staticItem.FriendlyName}</p>";
                }

            }
            #endregion

            return View(MVC.Contribution.Views.ContributeSpell, contribution);
        }

        [Authorize]
        public virtual ActionResult ContributePreview(int Id)
        {

            // assert only previewers can view this
            if (!User.IsInRole(PvPStatics.Permissions_Previewer))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            IContributionRepository contributionRepo = new EFContributionRepository();
            var contribution = contributionRepo.Contributions.FirstOrDefault(c => c.Id == Id && c.IsReadyForReview && !c.ProofreadingCopy);
            ViewBag.DisableLinks = true;

            var bbox = new BalanceBox();
            bbox.LoadBalanceBox(contribution);
            var balance = bbox.GetBalance();
            ViewBag.BalanceScore = balance;

            return View(MVC.Contribution.Views.ContributeSpell, contribution);
        }


        [Authorize]
        public virtual ActionResult ContributeBalanceCalculatorEffect(int id)
        {
            IEffectContributionRepository contributionRepo = new EFEffectContributionRepository();
            var contribution = contributionRepo.EffectContributions.FirstOrDefault(c => c.Id == id);

            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            var iAmProofreader = User.IsInRole(PvPStatics.Permissions_Proofreader);

            if (!iAmProofreader && contribution.OwnerMemberhipId != me.MembershipId)
            {
                TempData["Error"] = "That does not belong to you and you are not a proofreader.";
                return RedirectToAction(MVC.PvP.Play());
            }

            return View(MVC.Contribution.Views.BalanceCalculatorEffect, contribution);
        }

        [Authorize]
        public virtual ActionResult ContributeBalanceCalculator2(int id)
        {
            IContributionRepository contributionRepo = new EFContributionRepository();
            var contribution = contributionRepo.Contributions.FirstOrDefault(c => c.Id == id);

            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            var iAmProofreader = User.IsInRole(PvPStatics.Permissions_Proofreader);

            if (!iAmProofreader && contribution.OwnerMembershipId != me.MembershipId)
            {
                TempData["Error"] = "That does not belong to you and you are not a proofreader.";
                return RedirectToAction(MVC.PvP.Play());
            }

            return View(MVC.Contribution.Views.BalanceCalculator2, contribution);
        }

        [Authorize]
        public virtual ActionResult ContributeBalanceCalculatorSend(Contribution input)
        {
            IContributionRepository contributionRepo = new EFContributionRepository();
            var SaveMe = contributionRepo.Contributions.FirstOrDefault(c => c.Id == input.Id);

            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            var iAmProofreader = User.IsInRole(PvPStatics.Permissions_Proofreader);

            if (!iAmProofreader && SaveMe.OwnerMembershipId != me.MembershipId)
            {
                TempData["Error"] = "That does not belong to you and you are not a proofreader.";
                return RedirectToAction(MVC.PvP.Play());
            }
            else
            {
                SaveMe.HealthBonusPercent = input.HealthBonusPercent;
                SaveMe.ManaBonusPercent = input.ManaBonusPercent;
                SaveMe.ExtraSkillCriticalPercent = input.ExtraSkillCriticalPercent;
                SaveMe.HealthRecoveryPerUpdate = input.HealthRecoveryPerUpdate;
                SaveMe.ManaRecoveryPerUpdate = input.ManaRecoveryPerUpdate;
                SaveMe.SneakPercent = input.SneakPercent;
                SaveMe.EvasionPercent = input.EvasionPercent;
                SaveMe.EvasionNegationPercent = input.EvasionNegationPercent;
                SaveMe.MeditationExtraMana = input.MeditationExtraMana;
                SaveMe.CleanseExtraHealth = input.CleanseExtraHealth;
                SaveMe.MoveActionPointDiscount = input.MoveActionPointDiscount;
                SaveMe.SpellExtraTFEnergyPercent = input.SpellExtraTFEnergyPercent;
                SaveMe.SpellExtraHealthDamagePercent = input.SpellExtraHealthDamagePercent;
                SaveMe.CleanseExtraTFEnergyRemovalPercent = input.CleanseExtraTFEnergyRemovalPercent;
                SaveMe.SpellMisfireChanceReduction = input.SpellMisfireChanceReduction;
                SaveMe.SpellHealthDamageResistance = input.SpellHealthDamageResistance;
                SaveMe.SpellTFEnergyDamageResistance = input.SpellTFEnergyDamageResistance;
                SaveMe.ExtraInventorySpace = input.ExtraInventorySpace;

                // new stats
                SaveMe.Discipline = input.Discipline;
                SaveMe.Perception = input.Perception;
                SaveMe.Charisma = input.Charisma;
                SaveMe.Submission_Dominance = input.Submission_Dominance;

                SaveMe.Fortitude = input.Fortitude;
                SaveMe.Agility = input.Agility;
                SaveMe.Allure = input.Allure;
                SaveMe.Corruption_Purity = input.Corruption_Purity;

                SaveMe.Magicka = input.Magicka;
                SaveMe.Succour = input.Succour;
                SaveMe.Luck = input.Luck;
                SaveMe.Chaos_Order = input.Chaos_Order;


                SaveMe.History += "Bonus values edited by " + User.Identity.Name + " on " + DateTime.UtcNow + ".<br>";

                contributionRepo.SaveContribution(SaveMe);

                TempData["Result"] = "Contribution stats saved.";
                return RedirectToAction(MVC.PvP.Play());

            }
        }

        [Authorize]
        public virtual ActionResult ContributeBalanceCalculatorSend_Effect(EffectContribution input)
        {
            IEffectContributionRepository contributionRepo = new EFEffectContributionRepository();
            var SaveMe = contributionRepo.EffectContributions.FirstOrDefault(c => c.Id == input.Id);

            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            var iAmProofreader = User.IsInRole(PvPStatics.Permissions_Proofreader);

            if (!iAmProofreader && SaveMe.OwnerMemberhipId != me.MembershipId)
            {
                TempData["Error"] = "That does not belong to you and you are not a proofreader.";
                return RedirectToAction(MVC.PvP.Play());
            }
            else
            {
                SaveMe.HealthBonusPercent = input.HealthBonusPercent;
                SaveMe.ManaBonusPercent = input.ManaBonusPercent;
                SaveMe.ExtraSkillCriticalPercent = input.ExtraSkillCriticalPercent;
                SaveMe.HealthRecoveryPerUpdate = input.HealthRecoveryPerUpdate;
                SaveMe.ManaRecoveryPerUpdate = input.ManaRecoveryPerUpdate;
                SaveMe.SneakPercent = input.SneakPercent;
                SaveMe.EvasionPercent = input.EvasionPercent;
                SaveMe.EvasionNegationPercent = input.EvasionNegationPercent;
                SaveMe.MeditationExtraMana = input.MeditationExtraMana;
                SaveMe.CleanseExtraHealth = input.CleanseExtraHealth;
                SaveMe.MoveActionPointDiscount = input.MoveActionPointDiscount;
                SaveMe.SpellExtraTFEnergyPercent = input.SpellExtraTFEnergyPercent;
                SaveMe.SpellExtraHealthDamagePercent = input.SpellExtraHealthDamagePercent;
                SaveMe.CleanseExtraTFEnergyRemovalPercent = input.CleanseExtraTFEnergyRemovalPercent;
                SaveMe.SpellMisfireChanceReduction = input.SpellMisfireChanceReduction;
                SaveMe.SpellHealthDamageResistance = input.SpellHealthDamageResistance;
                SaveMe.SpellTFEnergyDamageResistance = input.SpellTFEnergyDamageResistance;
                SaveMe.ExtraInventorySpace = input.ExtraInventorySpace;

                // new stats
                SaveMe.Discipline = input.Discipline;
                SaveMe.Perception = input.Perception;
                SaveMe.Charisma = input.Charisma;
                SaveMe.Submission_Dominance = input.Submission_Dominance;

                SaveMe.Fortitude = input.Fortitude;
                SaveMe.Agility = input.Agility;
                SaveMe.Allure = input.Allure;
                SaveMe.Corruption_Purity = input.Corruption_Purity;

                SaveMe.Magicka = input.Magicka;
                SaveMe.Succour = input.Succour;
                SaveMe.Luck = input.Luck;
                SaveMe.Chaos_Order = input.Chaos_Order;

                SaveMe.Effect_Duration = input.Effect_Duration;

                //  SaveMe.History += "Bonus values edited by " + WebSecurity.CurrentUserName + " on " + DateTime.UtcNow + ".<br>";

                contributionRepo.SaveEffectContribution(SaveMe);

                TempData["Result"] = "Contribution stats saved.";
                return RedirectToAction(MVC.PvP.Play());

            }
        }

        [Authorize]
        public virtual ActionResult ContributeGraphicsNeeded()
        {
            IContributionRepository contributionRepo = new EFContributionRepository();
            IEnumerable<Contribution> output = contributionRepo.Contributions.Where(c => c.IsReadyForReview && c.AdminApproved && !c.IsLive && c.ProofreadingCopy);

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            return View(MVC.Contribution.Views.ContributeGraphicsNeeded, output);
        }

        [Authorize]
        public virtual ActionResult ContributeSetGraphicStatus(int id)
        {

            var iAmArtist = User.IsInRole(PvPStatics.Permissions_Artist);

            if (!iAmArtist)
            {
                TempData["Result"] = "You don't have permissions to do that.  If you are an artist and are interested in contributing artwork, please contact the administrator, Judoo.";
                return RedirectToAction(MVC.Contribution.ContributeGraphicsNeeded());
            }

            IContributionRepository contributionRepo = new EFContributionRepository();
            var cont = contributionRepo.Contributions.FirstOrDefault(c => c.Id == id);

            var output = new ContributionStatusViewModel
            {
                ContributionId = id,
                OwnerMembershipId = cont.OwnerMembershipId,
                Status = cont.AssignedToArtist,
            };

            return View(MVC.Contribution.Views.ContributeSetGraphicStatus, output);
        }

        [Authorize]
        public virtual ActionResult ContributeSetGraphicStatusSubmit(ContributionStatusViewModel input)
        {

            var iAmArtist = User.IsInRole(PvPStatics.Permissions_Artist);

            if (!iAmArtist)
            {
                TempData["Result"] = "You don't have permissions to do that.  If you are an artist and are interested in contributing artwork, please contact the administrator, Judoo.";
                return RedirectToAction(MVC.Contribution.ContributeGraphicsNeeded());
            }

            IContributionRepository contributionRepo = new EFContributionRepository();
            var cont = contributionRepo.Contributions.FirstOrDefault(c => c.Id == input.ContributionId);

            cont.AssignedToArtist = input.Status;
            cont.History += "Assigned artist changed by " + User.Identity.Name + " on " + DateTime.UtcNow + ".<br>";

            contributionRepo.SaveContribution(cont);


            TempData["Result"] = "Status saved!";
            return RedirectToAction(MVC.Contribution.ContributeGraphicsNeeded());

        }

        [HttpPost]
        public virtual ActionResult SendContribution(Contribution input)
        {
            var myMembershipId = User.Identity.GetUserId();
            IContributionRepository contributionRepo = new EFContributionRepository();
            Contribution SaveMe;

            Session["ContributionId"] = input.Id;

            var iAmProofreader = User.IsInRole(PvPStatics.Permissions_Proofreader);

            SaveMe = contributionRepo.Contributions.FirstOrDefault(c => c.Id == input.Id);
            if (SaveMe == null)
            {
                SaveMe = new Contribution();
                SaveMe.OwnerMembershipId = myMembershipId;
            }


            if (input.Id != -1)
            {

                // submitter is original author, ID stays the same and do NOT mark as proofreading version
                if (SaveMe != null && SaveMe.OwnerMembershipId == myMembershipId)
                {
                    SaveMe.Id = input.Id;
                }

                // submitter is not original author.  Do more logic...
                else if (SaveMe != null && SaveMe.OwnerMembershipId != myMembershipId)
                {
                    // this is a poorfreading copy.  Keep Id the same and keep it marked as a proofreading copy IF the editor is a proofreader
                    if (SaveMe.ProofreadingCopy && iAmProofreader)
                    {
                        SaveMe.Id = input.Id;
                        //SaveMe.ProofreadingCopy = true;
                    }
                    else
                    {
                        TempData["Result"] = "You do not have the authorization to edit this.  If you are a proofreader, make sure to load up the proofreading version instead.";
                        return RedirectToAction(MVC.PvP.Play());
                    }

                }


            }

            // unlock the proofreading flag since it has been saved
            input.ProofreadingLockIsOn = false;
            SaveMe.ProofreadingLockIsOn = false;
            SaveMe.CheckedOutBy = "";

            SaveMe.IsReadyForReview = input.IsReadyForReview;
            SaveMe.IsLive = input.IsLive;

            SaveMe.Skill_FriendlyName = input.Skill_FriendlyName;
            SaveMe.Skill_FormFriendlyName = input.Skill_FormFriendlyName;
            SaveMe.Skill_Description = input.Skill_Description;
            SaveMe.Skill_ManaCost = input.Skill_ManaCost;
            SaveMe.Skill_TFPointsAmount = input.Skill_TFPointsAmount;
            SaveMe.Skill_HealthDamageAmount = input.Skill_HealthDamageAmount;
            SaveMe.Skill_LearnedAtRegion = input.Skill_LearnedAtRegion;
            SaveMe.Skill_LearnedAtLocationOrRegion = input.Skill_LearnedAtLocationOrRegion;
            SaveMe.Skill_DiscoveryMessage = input.Skill_DiscoveryMessage;
            SaveMe.Skill_IsPlayerLearnable = input.Skill_IsPlayerLearnable;

            SaveMe.Form_FriendlyName = input.Form_FriendlyName;
            SaveMe.Form_Description = input.Form_Description;
            SaveMe.Form_TFEnergyRequired = input.Form_TFEnergyRequired;
            SaveMe.Form_Gender = input.Form_Gender;
            SaveMe.Form_MobilityType = input.Form_MobilityType;
            SaveMe.Form_BecomesItemDbName = input.Form_BecomesItemDbName;
            SaveMe.Form_Bonuses = input.Form_Bonuses;

            SaveMe.Form_TFMessage_20_Percent_1st = input.Form_TFMessage_20_Percent_1st;
            SaveMe.Form_TFMessage_40_Percent_1st = input.Form_TFMessage_40_Percent_1st;
            SaveMe.Form_TFMessage_60_Percent_1st = input.Form_TFMessage_60_Percent_1st;
            SaveMe.Form_TFMessage_80_Percent_1st = input.Form_TFMessage_80_Percent_1st;
            SaveMe.Form_TFMessage_100_Percent_1st = input.Form_TFMessage_100_Percent_1st;
            SaveMe.Form_TFMessage_Completed_1st = input.Form_TFMessage_Completed_1st;

            SaveMe.Form_TFMessage_20_Percent_1st_M = input.Form_TFMessage_20_Percent_1st_M;
            SaveMe.Form_TFMessage_40_Percent_1st_M = input.Form_TFMessage_40_Percent_1st_M;
            SaveMe.Form_TFMessage_60_Percent_1st_M = input.Form_TFMessage_60_Percent_1st_M;
            SaveMe.Form_TFMessage_80_Percent_1st_M = input.Form_TFMessage_80_Percent_1st_M;
            SaveMe.Form_TFMessage_100_Percent_1st_M = input.Form_TFMessage_100_Percent_1st_M;
            SaveMe.Form_TFMessage_Completed_1st_M = input.Form_TFMessage_Completed_1st_M;

            SaveMe.Form_TFMessage_20_Percent_1st_F = input.Form_TFMessage_20_Percent_1st_F;
            SaveMe.Form_TFMessage_40_Percent_1st_F = input.Form_TFMessage_40_Percent_1st_F;
            SaveMe.Form_TFMessage_60_Percent_1st_F = input.Form_TFMessage_60_Percent_1st_F;
            SaveMe.Form_TFMessage_80_Percent_1st_F = input.Form_TFMessage_80_Percent_1st_F;
            SaveMe.Form_TFMessage_100_Percent_1st_F = input.Form_TFMessage_100_Percent_1st_F;
            SaveMe.Form_TFMessage_Completed_1st_F = input.Form_TFMessage_Completed_1st_F;

            SaveMe.Form_TFMessage_20_Percent_3rd = input.Form_TFMessage_20_Percent_3rd;
            SaveMe.Form_TFMessage_40_Percent_3rd = input.Form_TFMessage_40_Percent_3rd;
            SaveMe.Form_TFMessage_60_Percent_3rd = input.Form_TFMessage_60_Percent_3rd;
            SaveMe.Form_TFMessage_80_Percent_3rd = input.Form_TFMessage_80_Percent_3rd;
            SaveMe.Form_TFMessage_100_Percent_3rd = input.Form_TFMessage_100_Percent_3rd;
            SaveMe.Form_TFMessage_Completed_3rd = input.Form_TFMessage_Completed_3rd;

            SaveMe.Form_TFMessage_20_Percent_3rd_M = input.Form_TFMessage_20_Percent_3rd_M;
            SaveMe.Form_TFMessage_40_Percent_3rd_M = input.Form_TFMessage_40_Percent_3rd_M;
            SaveMe.Form_TFMessage_60_Percent_3rd_M = input.Form_TFMessage_60_Percent_3rd_M;
            SaveMe.Form_TFMessage_80_Percent_3rd_M = input.Form_TFMessage_80_Percent_3rd_M;
            SaveMe.Form_TFMessage_100_Percent_3rd_M = input.Form_TFMessage_100_Percent_3rd_M;
            SaveMe.Form_TFMessage_Completed_3rd_M = input.Form_TFMessage_Completed_3rd_M;

            SaveMe.Form_TFMessage_20_Percent_3rd_F = input.Form_TFMessage_20_Percent_3rd_F;
            SaveMe.Form_TFMessage_40_Percent_3rd_F = input.Form_TFMessage_40_Percent_3rd_F;
            SaveMe.Form_TFMessage_60_Percent_3rd_F = input.Form_TFMessage_60_Percent_3rd_F;
            SaveMe.Form_TFMessage_80_Percent_3rd_F = input.Form_TFMessage_80_Percent_3rd_F;
            SaveMe.Form_TFMessage_100_Percent_3rd_F = input.Form_TFMessage_100_Percent_3rd_F;
            SaveMe.Form_TFMessage_Completed_3rd_F = input.Form_TFMessage_Completed_3rd_F;

            SaveMe.CursedTF_FormSourceId = input.CursedTF_FormSourceId;
            SaveMe.CursedTF_Fail = input.CursedTF_Fail;
            SaveMe.CursedTF_Fail_M = input.CursedTF_Fail_M;
            SaveMe.CursedTF_Fail_F = input.CursedTF_Fail_F;
            SaveMe.CursedTF_Succeed = input.CursedTF_Succeed;
            SaveMe.CursedTF_Succeed_M = input.CursedTF_Succeed_M;
            SaveMe.CursedTF_Succeed_F = input.CursedTF_Succeed_F;

            SaveMe.Item_FriendlyName = input.Item_FriendlyName;
            SaveMe.Item_Description = input.Item_Description;
            SaveMe.Item_ItemType = input.Item_ItemType;
            SaveMe.Item_UseCooldown = input.Item_UseCooldown;
            SaveMe.Item_Bonuses = input.Item_Bonuses;

            SaveMe.Item_UsageMessage_Item = input.Item_UsageMessage_Item;
            SaveMe.Item_UsageMessage_Player = input.Item_UsageMessage_Player;

            SaveMe.SubmitterName = input.SubmitterName;
            SaveMe.SubmitterUrl = input.SubmitterUrl;
            SaveMe.AdditionalSubmitterNames = input.AdditionalSubmitterNames;
            SaveMe.Notes = input.Notes;
            SaveMe.NeedsToBeUpdated = input.NeedsToBeUpdated;
            SaveMe.IsNonstandard = input.IsNonstandard;

            SaveMe.AssignedToArtist = input.AssignedToArtist;

            if (!input.ImageURL.IsNullOrEmpty() && User.IsInRole(PvPStatics.Permissions_Admin))
            {
                SaveMe.ImageURL = input.ImageURL;
            }

            SaveMe.CreationTimestamp = DateTime.UtcNow;

            if (SaveMe.ProofreadingCopy)
            {
                SaveMe.History += "Edited by " + User.Identity.Name + " on " + DateTime.UtcNow + ".<br>";
            }

            contributionRepo.SaveContribution(SaveMe);

            TempData["Result"] = "Contribution Saved!";
            return RedirectToAction(MVC.PvP.Play());
        }

        [Authorize]
        public virtual ActionResult SendContributionUndoLock(int id)
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            var iAmProofreader = User.IsInRole(PvPStatics.Permissions_Proofreader);

            if (!iAmProofreader)
            {
                TempData["Error"] = "You must be a proofreader in order to do this.";
                return RedirectToAction(MVC.PvP.Play());
            }

            IContributionRepository contributionRepo = new EFContributionRepository();
            var contribution = contributionRepo.Contributions.FirstOrDefault(c => c.Id == id);

            if (!contribution.ProofreadingCopy)
            {
                TempData["Error"] = "This is not a proofreading copy.";
                return RedirectToAction(MVC.PvP.Play());
            }

            contribution.ProofreadingLockIsOn = false;
            contribution.CheckedOutBy = "";
            contributionRepo.SaveContribution(contribution);

            return RedirectToAction(MVC.PvP.Play());
        }

        [Authorize]
        public virtual ActionResult ContributeEffect(int id = -1)
        {
            var myMembershipId = User.Identity.GetUserId();
            // get all of this players effect contributions
            IEffectContributionRepository effectContRepo = new EFEffectContributionRepository();
            IDbStaticSkillRepository staticSkillRepository = new EFDbStaticSkillRepository();

            IEnumerable<EffectContribution> myEffectContributions = effectContRepo.EffectContributions.Where(c => c.OwnerMemberhipId == myMembershipId);
            ViewBag.OtherEffectContributions = myEffectContributions;

            var output = effectContRepo.EffectContributions.FirstOrDefault(e => e.Id == id);

            var proofreading = new List<EffectContribution>();

            var iAmProofreader = User.IsInRole(PvPStatics.Permissions_Proofreader);
            var iAmAdmin = User.IsInRole(PvPStatics.Permissions_Admin);

            // add the rest of the submitted contributions if the player is a proofread
            if (iAmProofreader)
            {
                proofreading = effectContRepo.EffectContributions.Where(c => c.ApprovedByAdmin && c.ProofreadingCopy).ToList();
                ViewBag.Proofreading = proofreading;
            }

            // assert that this is the owner's work or else that the reader is a proofreader and is a proofreading copy

            if (output == null)
            {
                output = new EffectContribution
                {
                    OwnerMemberhipId = myMembershipId,
                    History = "",
                };
            }

            // not new... check for proofreading permissions
            else
            {
                if (output.OwnerMemberhipId != myMembershipId && (!iAmProofreader || (!output.ProofreadingCopy && !iAmAdmin)))
                {
                    TempData["Error"] = TempData["You do not have permission to view this."];
                    return RedirectToAction(MVC.PvP.Play());
                }
            }


            // if this is a proofreading copy, set the lock
            if (output.ProofreadingCopy)
            {
                output.ProofreadingLockIsOn = true;
                output.CheckedOutBy = User.Identity.Name;
                output.Timestamp = DateTime.UtcNow;
                effectContRepo.SaveEffectContribution(output);
            }




            if (User.IsInRole(PvPStatics.Permissions_Admin) && output.ProofreadingCopy)
            {

                var staticSkill =
                    staticSkillRepository.DbStaticSkills.FirstOrDefault(s => s.Id == output.SkillSourceId.Value);

                ViewBag.StaticEffectExists = $"<span class = 'bad'>Static effect not found!</span>";
                ViewBag.StaticSpellExists = "<span class = 'bad'>Static spell not found!<span>";

                IDbStaticEffectRepository effectRepo = new EFDbStaticEffectRepository();
                var possibleEffect = effectRepo.DbStaticEffects.FirstOrDefault(e => e.Id == output.EffectSourceId);
                if (possibleEffect != null)
                {
                    ViewBag.StaticEffectExists = $"<span class = 'good'>Static effect '{possibleEffect.FriendlyName}' exists!</span>";
                }

                IDbStaticSkillRepository skillRepo = new EFDbStaticSkillRepository();
                var possibleSkill = skillRepo.DbStaticSkills.FirstOrDefault(e => e.Id == output.SkillSourceId);
                if (possibleSkill != null)
                {
                    ViewBag.StaticSpellExists = $"<span class = 'good'>Static spell '{staticSkill.FriendlyName}' exists!</span>";
                }

            }

            var bbox = new BalanceBox();
            bbox.LoadBalanceBox(output);
            var balance = bbox.GetBalance__NoModifiersOrCaps();

            ViewBag.BalanceScore = balance * output.Effect_Duration;

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];


            return View(MVC.Contribution.Views.ContributeEffect, output);
        }

        public virtual ActionResult SendEffectContribution(EffectContribution input)
        {
            var myMembershipId = User.Identity.GetUserId();
            IEffectContributionRepository effectContRepo = new EFEffectContributionRepository();
            var saveme = effectContRepo.EffectContributions.FirstOrDefault(e => e.Id == input.Id);

            // TODO:  assert player owns this

            var iAmProofreader = User.IsInRole(PvPStatics.Permissions_Proofreader);

            if (saveme == null)
            {
                saveme = new EffectContribution
                {
                    OwnerMemberhipId = myMembershipId,
                    ReadyForReview = false,
                    ApprovedByAdmin = false,
                    IsLive = false,
                    History = "",
                };

                // make sure this actually is the player's own contribution
            }
            else if (saveme.OwnerMemberhipId != myMembershipId && !iAmProofreader)
            {
                TempData["Error"] = "This contribution does not belong to you and you are not a proofreader.";
                TempData["SubError"] = "You may have been logged out; check that you are logged in the the game still in another tab.";
                return RedirectToAction(MVC.Contribution.ContributeEffect());
            }

            if (input.Id != -1)
            {
                saveme.Id = input.Id;
            }

            saveme.SubmitterName = input.SubmitterName;
            saveme.SubmitterURL = input.SubmitterURL;
            saveme.ReadyForReview = input.ReadyForReview;

            saveme.Skill_FriendlyName = input.Skill_FriendlyName;
            saveme.Skill_UniqueToFormSourceId = input.Skill_UniqueToFormSourceId;
            saveme.Skill_UniqueToItemSourceId = input.Skill_UniqueToItemSourceId;
            saveme.Skill_UniqueToLocation = input.Skill_UniqueToLocation;
            saveme.Skill_Description = input.Skill_Description;
            saveme.Skill_ManaCost = input.Skill_ManaCost;

            saveme.Effect_FriendlyName = input.Effect_FriendlyName;
            saveme.Effect_Description = input.Effect_Description;
            saveme.Effect_Duration = input.Effect_Duration;
            saveme.Effect_Cooldown = input.Effect_Cooldown;
            saveme.Effect_Bonuses = input.Effect_Bonuses;
            saveme.Effect_IsRemovable = input.Effect_IsRemovable;
            saveme.Effect_VictimHitText = input.Effect_VictimHitText;
            saveme.Effect_VictimHitText_M = input.Effect_VictimHitText_M;
            saveme.Effect_VictimHitText_F = input.Effect_VictimHitText_F;
            saveme.Effect_AttackHitText = input.Effect_AttackHitText;
            saveme.Effect_AttackHitText_M = input.Effect_AttackHitText_M;
            saveme.Effect_AttackHitText_F = input.Effect_AttackHitText_F;

            saveme.Timestamp = DateTime.UtcNow;
            saveme.AdditionalSubmitterNames = input.AdditionalSubmitterNames;
            saveme.Notes = input.Notes;
            saveme.NeedsToBeUpdated = input.NeedsToBeUpdated;

            if (saveme.ProofreadingCopy)
            {
                saveme.ProofreadingLockIsOn = false;
                saveme.CheckedOutBy = "";
                saveme.History += "Edited by " + User.Identity.Name + " on " + DateTime.UtcNow + ".<br>";
            }


            effectContRepo.SaveEffectContribution(saveme);

            TempData["Result"] = "Effect Contribution saved!";
            return RedirectToAction(MVC.PvP.Play());
        }


        public virtual ActionResult UnlockEffectContribution(int id)
        {
            IEffectContributionRepository effectContRepo = new EFEffectContributionRepository();
            var saveme = effectContRepo.EffectContributions.FirstOrDefault(e => e.Id == id);
            saveme.ProofreadingLockIsOn = false;
            saveme.CheckedOutBy = "";
            effectContRepo.SaveEffectContribution(saveme);
            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult ContributorBio(string name)
        {
            // TODO: Figure out how to T4ize
            return View("~/Views/Contribution/Bios/" + name + ".cshtml");
        }

        public virtual ActionResult PublishSpell(int id)
        {

            if (!User.IsInRole(PvPStatics.Permissions_Admin) && !User.IsInRole(PvPStatics.Permissions_Publisher))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            var message = "started.<br>";

            IContributionRepository contributionRepo = new EFContributionRepository();
            IDbStaticSkillRepository skillRepo = new EFDbStaticSkillRepository();
            IDbStaticFormRepository formRepo = new EFDbStaticFormRepository();
            var contribution = contributionRepo.Contributions.FirstOrDefault(c => c.Id == id);

            var spell = skillRepo.DbStaticSkills.FirstOrDefault(s => s.Id == contribution.SkillSourceId);
            var form = formRepo.DbStaticForms.FirstOrDefault(s => s.Id == contribution.FormSourceId);

            if (spell == null)
            {
                spell = new DbStaticSkill();
                spell.FormSourceId = form.Id;
                message += "<p class='bad'>Made new spell.</p>";
            }
            else
            {
                message += "<p class='good'>Loaded existing spell.</p>";
            }

            spell.Description = contribution.Skill_Description;
            spell.DiscoveryMessage = contribution.Skill_DiscoveryMessage;
            spell.FriendlyName = contribution.Skill_FriendlyName;
            spell.HealthDamageAmount = contribution.Skill_HealthDamageAmount;
            spell.TFPointsAmount = contribution.Skill_TFPointsAmount;
            spell.IsPlayerLearnable = contribution.Skill_IsPlayerLearnable;

            //  TODO: THIS ASSUMES LEARNED AT SPECIFIC LOCATION; THIS NEEDS TO BE CHANGED
            spell.LearnedAtLocation = "";
            spell.LearnedAtRegion = "";
            if (contribution.Skill_LearnedAtLocationOrRegion == "region")
            {
                spell.LearnedAtRegion = contribution.Skill_LearnedAtRegion;
            }
            else
            {
                spell.LearnedAtLocation = contribution.Skill_LearnedAtRegion;
            }



            spell.ManaCost = contribution.Skill_ManaCost;
            spell.MobilityType = contribution.Form_MobilityType;

            #region write credits
            var output = "New spell, " + contribution.Skill_FriendlyName + ", submitted by ";

            if (!contribution.SubmitterUrl.IsNullOrEmpty())
            {
                output += "<a href=\"" + contribution.SubmitterUrl + "\">" + contribution.SubmitterName + "</a>!";
            }
            else
            {
                output += contribution.SubmitterName + "!";
            }

            if (!contribution.AdditionalSubmitterNames.IsNullOrEmpty())
            {
                output += "  Additional credits go to " + contribution.AdditionalSubmitterNames + ".  ";
            }

            if (!contribution.AssignedToArtist.IsNullOrEmpty())
            {
                output += " Graphic is by " + contribution.AssignedToArtist + ".  ";
            }

            if (contribution.Form_MobilityType == PvPStatics.MobilityFull && !contribution.Form_FriendlyName.IsNullOrEmpty())
            {
                output += $"This spell turns its victim into a {contribution.Form_FriendlyName}!";
            }
            else if ((contribution.Form_MobilityType == PvPStatics.MobilityInanimate || contribution.Form_MobilityType == PvPStatics.MobilityPet) && !contribution.Item_FriendlyName.IsNullOrEmpty())
            {
                output += $"This spell turns its victim into a {contribution.Item_FriendlyName}!";
            }

            message += output;

            #endregion

            skillRepo.SaveDbStaticSkill(spell);
            contribution.SkillSourceId = spell.Id;

            if (contribution.Skill_LearnedAtLocationOrRegion == "location")
            {
                var temp = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == spell.LearnedAtLocation);
                if (temp == null)
                {
                    message += "<span class='bad'> !!!!! WARNING:  NO LOCATION FOUND FOR THIS SPELL.</span>  ";
                }
            }

            if (contribution.Skill_LearnedAtLocationOrRegion == "region")
            {
                var temp = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.Region == spell.LearnedAtRegion);
                if (temp == null)
                {
                    message += "<span class='bad'> !!!!! WARNING:  NO REGION FOUND FOR THIS SPELL.</span>  ";
                }
            }

            contribution.History += "Spell published on " + DateTime.UtcNow + "<br>";
            contributionRepo.SaveContribution(contribution);

            try
            {
                // add in any missing FKs for this spell
                DomainRegistry.Repository.Execute(new SetSkillSourceFKs
                {
                    SkillSourceId = spell.Id,
                    ExclusiveToItemSourceId = spell.ExclusiveToItemSourceId,
                    ExclusiveToFormSourceId = spell.ExclusiveToFormSourceId,
                    GivesEffectSourceId = spell.GivesEffectSourceId,
                    FormSourceId = spell.FormSourceId
                });
            }
            catch (DomainException e)
            {
                message += "<br><br><span class='bad'>FAILED TO SET FOREIGN KEY TO FORMSOURCE. Error:  " + e.Message + "</span>";
            }


            ViewBag.Message = message;
            return View(MVC.Contribution.Views.Publish);
        }

        public virtual ActionResult PublishForm(int id)
        {
            if (!User.IsInRole(PvPStatics.Permissions_Admin) && !User.IsInRole(PvPStatics.Permissions_Publisher))
            {
                return RedirectToAction(MVC.PvP.Play());
            }



            var message = "started.<br>";

            IContributionRepository contributionRepo = new EFContributionRepository();
            IDbStaticFormRepository formRepo = new EFDbStaticFormRepository();
            ITFMessageRepository tfRepo = new EFTFMessageRepository();
            var contribution = contributionRepo.Contributions.FirstOrDefault(c => c.Id == id);

            var form = formRepo.DbStaticForms.FirstOrDefault(s => s.Id == contribution.FormSourceId);
            if (form == null)
            {
                form = new DbStaticForm();
                formRepo.SaveDbStaticForm(form); // need to get the id of this new form
                message += "<p class='bad'>Wrote NEW form to database.</p>";
            }
            else
            {
                message += "<p class='good'>Loaded existing form from database.</p>";
            }

            var tf = tfRepo.TFMessages.FirstOrDefault(t => t.FormSourceId == form.Id);

            if (tf == null)
            {
                tf = new TFMessage();
                tf.FormSourceId = form.Id;
                message += "<p class='bad'>Wrote NEW tf message object to database.</p>";
            }
            else
            {
                message += "<p class='good'>Loaded existing tf message object from database.</p>";
            }

            form.ItemSourceId = contribution.ItemSourceId;
            form.Description = contribution.Form_Description;
            form.FriendlyName = contribution.Form_FriendlyName;
            form.Gender = contribution.Form_Gender;
            form.MobilityType = contribution.Form_MobilityType;
            message += "Remember to add the portrait URL manually.<br>";
            form.TFEnergyRequired = contribution.Form_TFEnergyRequired;


            form.HealthBonusPercent = contribution.HealthBonusPercent;
            form.ManaBonusPercent = contribution.ManaBonusPercent;
            form.ExtraSkillCriticalPercent = contribution.ExtraSkillCriticalPercent;
            form.HealthRecoveryPerUpdate = contribution.HealthRecoveryPerUpdate;
            form.ManaRecoveryPerUpdate = contribution.ManaRecoveryPerUpdate;
            form.SneakPercent = contribution.SneakPercent;
            form.EvasionPercent = contribution.EvasionPercent;
            form.EvasionNegationPercent = contribution.EvasionNegationPercent;
            form.MeditationExtraMana = contribution.MeditationExtraMana;
            form.CleanseExtraHealth = contribution.CleanseExtraHealth;
            form.MoveActionPointDiscount = contribution.MoveActionPointDiscount;
            form.SpellExtraTFEnergyPercent = contribution.SpellExtraTFEnergyPercent;
            form.SpellExtraHealthDamagePercent = contribution.SpellExtraHealthDamagePercent;
            form.CleanseExtraTFEnergyRemovalPercent = contribution.CleanseExtraTFEnergyRemovalPercent;
            form.SpellMisfireChanceReduction = contribution.SpellMisfireChanceReduction;
            form.SpellHealthDamageResistance = contribution.SpellHealthDamageResistance;
            form.SpellTFEnergyDamageResistance = contribution.SpellTFEnergyDamageResistance;
            form.ExtraInventorySpace = contribution.ExtraInventorySpace;


            // new stats
            form.Discipline = contribution.Discipline;
            form.Perception = contribution.Perception;
            form.Charisma = contribution.Charisma;
            form.Submission_Dominance = contribution.Submission_Dominance;

            form.Fortitude = contribution.Fortitude;
            form.Agility = contribution.Agility;
            form.Allure = contribution.Allure;
            form.Corruption_Purity = contribution.Corruption_Purity;

            form.Magicka = contribution.Magicka;
            form.Succour = contribution.Succour;
            form.Luck = contribution.Luck;
            form.Chaos_Order = contribution.Chaos_Order;

            if (contribution.Form_MobilityType == PvPStatics.MobilityFull)
            {
                form.PortraitUrl = contribution.ImageURL;
            }

            tf.TFMessage_20_Percent_1st = contribution.Form_TFMessage_20_Percent_1st;
            tf.TFMessage_40_Percent_1st = contribution.Form_TFMessage_40_Percent_1st;
            tf.TFMessage_60_Percent_1st = contribution.Form_TFMessage_60_Percent_1st;
            tf.TFMessage_80_Percent_1st = contribution.Form_TFMessage_80_Percent_1st;
            tf.TFMessage_100_Percent_1st = contribution.Form_TFMessage_100_Percent_1st;
            tf.TFMessage_Completed_1st = contribution.Form_TFMessage_Completed_1st;

            tf.TFMessage_20_Percent_1st_M = contribution.Form_TFMessage_20_Percent_1st_M;
            tf.TFMessage_40_Percent_1st_M = contribution.Form_TFMessage_40_Percent_1st_M;
            tf.TFMessage_60_Percent_1st_M = contribution.Form_TFMessage_60_Percent_1st_M;
            tf.TFMessage_80_Percent_1st_M = contribution.Form_TFMessage_80_Percent_1st_M;
            tf.TFMessage_100_Percent_1st_M = contribution.Form_TFMessage_100_Percent_1st_M;
            tf.TFMessage_Completed_1st_M = contribution.Form_TFMessage_Completed_1st_M;

            tf.TFMessage_20_Percent_1st_F = contribution.Form_TFMessage_20_Percent_1st_F;
            tf.TFMessage_40_Percent_1st_F = contribution.Form_TFMessage_40_Percent_1st_F;
            tf.TFMessage_60_Percent_1st_F = contribution.Form_TFMessage_60_Percent_1st_F;
            tf.TFMessage_80_Percent_1st_F = contribution.Form_TFMessage_80_Percent_1st_F;
            tf.TFMessage_100_Percent_1st_F = contribution.Form_TFMessage_100_Percent_1st_F;
            tf.TFMessage_Completed_1st_F = contribution.Form_TFMessage_Completed_1st_F;

            tf.TFMessage_20_Percent_3rd = contribution.Form_TFMessage_20_Percent_3rd;
            tf.TFMessage_40_Percent_3rd = contribution.Form_TFMessage_40_Percent_3rd;
            tf.TFMessage_60_Percent_3rd = contribution.Form_TFMessage_60_Percent_3rd;
            tf.TFMessage_80_Percent_3rd = contribution.Form_TFMessage_80_Percent_3rd;
            tf.TFMessage_100_Percent_3rd = contribution.Form_TFMessage_100_Percent_3rd;
            tf.TFMessage_Completed_3rd = contribution.Form_TFMessage_Completed_3rd;

            tf.TFMessage_20_Percent_3rd_M = contribution.Form_TFMessage_20_Percent_3rd_M;
            tf.TFMessage_40_Percent_3rd_M = contribution.Form_TFMessage_40_Percent_3rd_M;
            tf.TFMessage_60_Percent_3rd_M = contribution.Form_TFMessage_60_Percent_3rd_M;
            tf.TFMessage_80_Percent_3rd_M = contribution.Form_TFMessage_80_Percent_3rd_M;
            tf.TFMessage_100_Percent_3rd_M = contribution.Form_TFMessage_100_Percent_3rd_M;
            tf.TFMessage_Completed_3rd_M = contribution.Form_TFMessage_Completed_3rd_M;

            tf.TFMessage_20_Percent_3rd_F = contribution.Form_TFMessage_20_Percent_3rd_F;
            tf.TFMessage_40_Percent_3rd_F = contribution.Form_TFMessage_40_Percent_3rd_F;
            tf.TFMessage_60_Percent_3rd_F = contribution.Form_TFMessage_60_Percent_3rd_F;
            tf.TFMessage_80_Percent_3rd_F = contribution.Form_TFMessage_80_Percent_3rd_F;
            tf.TFMessage_100_Percent_3rd_F = contribution.Form_TFMessage_100_Percent_3rd_F;
            tf.TFMessage_Completed_3rd_F = contribution.Form_TFMessage_Completed_3rd_F;

            tf.FormSourceId = form.Id;

            tfRepo.SaveTFMessage(tf);
            formRepo.SaveDbStaticForm(form);
            contribution.FormSourceId = form.Id;
            ViewBag.Message = message;

            contribution.History += "Form published on " + DateTime.UtcNow + "<br>";
            contributionRepo.SaveContribution(contribution);

            return View(MVC.Contribution.Views.Publish);
        }

        public virtual ActionResult PublishItem(int id)
        {

            if (!User.IsInRole(PvPStatics.Permissions_Admin) && !User.IsInRole(PvPStatics.Permissions_Publisher))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            var message = "started.<br>";

            IContributionRepository contributionRepo = new EFContributionRepository();
            IDbStaticItemRepository itemRepo = new EFDbStaticItemRepository();
            var contribution = contributionRepo.Contributions.FirstOrDefault(c => c.Id == id);

            var item = itemRepo.DbStaticItems.FirstOrDefault(s => s.Id == contribution.ItemSourceId);

            if (item == null)
            {
                item = new DbStaticItem();
                message += "<p class='bad'>CREATED NEW ENTRY.</p>";
            }
            else
            {
                message += "<p class='good'>Loaded existing item.</p>";
            }

            item.Description = contribution.Item_Description;
            item.FriendlyName = contribution.Item_FriendlyName;
            message += "<p>You must set the filename for the image yourself.</p>";
            item.ItemType = contribution.Item_ItemType;
            item.UseCooldown = contribution.Item_UseCooldown;

            item.UsageMessage_Item = contribution.Item_UsageMessage_Item;
            item.UsageMessage_Player = contribution.Item_UsageMessage_Player;

            item.Findable = false;

            if (contribution.Form_MobilityType == PvPStatics.MobilityInanimate || contribution.Form_MobilityType == PvPStatics.MobilityPet)
            {
                item.PortraitUrl = contribution.ImageURL;

                // update the form's graphic too while we're at it.
                IDbStaticFormRepository formRepo = new EFDbStaticFormRepository();
                var form = formRepo.DbStaticForms.FirstOrDefault(f => f.ItemSourceId == item.Id);
                if (form != null)
                {
                    form.PortraitUrl = item.PortraitUrl;
                    formRepo.SaveDbStaticForm(form);
                }

            }

            item.HealthBonusPercent = contribution.HealthBonusPercent;
            item.ManaBonusPercent = contribution.ManaBonusPercent;
            item.ExtraSkillCriticalPercent = contribution.ExtraSkillCriticalPercent;
            item.HealthRecoveryPerUpdate = contribution.HealthRecoveryPerUpdate;
            item.ManaRecoveryPerUpdate = contribution.ManaRecoveryPerUpdate;
            item.SneakPercent = contribution.SneakPercent;
            item.EvasionPercent = contribution.EvasionPercent;
            item.EvasionNegationPercent = contribution.EvasionNegationPercent;
            item.MeditationExtraMana = contribution.MeditationExtraMana;
            item.CleanseExtraHealth = contribution.CleanseExtraHealth;
            item.MoveActionPointDiscount = contribution.MoveActionPointDiscount;
            item.SpellExtraTFEnergyPercent = contribution.SpellExtraTFEnergyPercent;
            item.SpellExtraHealthDamagePercent = contribution.SpellExtraHealthDamagePercent;
            item.CleanseExtraTFEnergyRemovalPercent = contribution.CleanseExtraTFEnergyRemovalPercent;
            item.SpellMisfireChanceReduction = contribution.SpellMisfireChanceReduction;
            item.SpellHealthDamageResistance = contribution.SpellHealthDamageResistance;
            item.SpellTFEnergyDamageResistance = contribution.SpellTFEnergyDamageResistance;
            item.ExtraInventorySpace = contribution.ExtraInventorySpace;

            // new stats
            item.Discipline = contribution.Discipline;
            item.Perception = contribution.Perception;
            item.Charisma = contribution.Charisma;
            item.Submission_Dominance = contribution.Submission_Dominance;

            item.Fortitude = contribution.Fortitude;
            item.Agility = contribution.Agility;
            item.Allure = contribution.Allure;
            item.Corruption_Purity = contribution.Corruption_Purity;

            item.Magicka = contribution.Magicka;
            item.Succour = contribution.Succour;
            item.Luck = contribution.Luck;
            item.Chaos_Order = contribution.Chaos_Order;

            item.CurseTFFormSourceId = contribution.CursedTF_FormSourceId;

            if (item.CurseTFFormSourceId != null)
            {
                ITFMessageRepository tfRepo = new EFTFMessageRepository();
                var tf = tfRepo.TFMessages.FirstOrDefault(t => t.FormSourceId == item.CurseTFFormSourceId);

                if (tf == null)
                {
                    message += $"<p class='bad'>Unable to locate TF message id '{item.CurseTFFormSourceId}'.  Not writing curse text.  Make sure to publish the form first if needed.</p>";
                }
                else
                {
                    tf.CursedTF_Fail = contribution.CursedTF_Fail;
                    tf.CursedTF_Fail_M = contribution.CursedTF_Fail_M;
                    tf.CursedTF_Fail_F = contribution.CursedTF_Fail_F;
                    tf.CursedTF_Succeed = contribution.CursedTF_Succeed;
                    tf.CursedTF_Succeed_M = contribution.CursedTF_Succeed_M;
                    tf.CursedTF_Succeed_F = contribution.CursedTF_Succeed_F;
                    tfRepo.SaveTFMessage(tf);

                    message += "<p class='good'>Added TF curse text.</p>";
                }

            }



            itemRepo.SaveDbStaticItem(item);
            contribution.ItemSourceId = item.Id;

            ViewBag.Message = message;

            contribution.History += "Item published on " + DateTime.UtcNow + "<br>";
            contributionRepo.SaveContribution(contribution);

            return View(MVC.Contribution.Views.Publish);
        }

        public virtual ActionResult PublishEffect(int id)
        {
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            var message = "";

            IEffectContributionRepository contRepo = new EFEffectContributionRepository();
            IDbStaticEffectRepository effectRepo = new EFDbStaticEffectRepository();

            var contribution = contRepo.EffectContributions.FirstOrDefault(e => e.Id == id);
            var effect = effectRepo.DbStaticEffects.FirstOrDefault(e => e.Id == contribution.EffectSourceId);


            if (effect == null)
            {
                effect = new DbStaticEffect();
                
                message += "<p class='bad'>Made new effect.</p>";
            }
            else
            {
                message += "<p class='good'>Loaded existing effect.</p>";
            }

            effect.FriendlyName = contribution.Effect_FriendlyName;
            effect.Description = contribution.Effect_Description;
            effect.Duration = contribution.Effect_Duration;
            effect.Cooldown = contribution.Effect_Cooldown;

            effect.ObtainedAtLocation = contribution.Skill_UniqueToLocation;
            effect.IsRemovable = contribution.Effect_IsRemovable;

            effect.MessageWhenHit = contribution.Effect_VictimHitText;
            effect.MessageWhenHit_M = contribution.Effect_VictimHitText_M;
            effect.MessageWhenHit_F = contribution.Effect_VictimHitText_F;

            effect.AttackerWhenHit = contribution.Effect_AttackHitText;
            effect.AttackerWhenHit_M = contribution.Effect_AttackHitText_M;
            effect.AttackerWhenHit_F = contribution.Effect_AttackHitText_F;

            effect.HealthBonusPercent = contribution.HealthBonusPercent;
            effect.ManaBonusPercent = contribution.ManaBonusPercent;
            effect.ExtraSkillCriticalPercent = contribution.ExtraSkillCriticalPercent;
            effect.HealthRecoveryPerUpdate = contribution.HealthRecoveryPerUpdate;
            effect.ManaRecoveryPerUpdate = contribution.ManaRecoveryPerUpdate;
            effect.SneakPercent = contribution.SneakPercent;
            effect.EvasionPercent = contribution.EvasionPercent;
            effect.EvasionNegationPercent = contribution.EvasionNegationPercent;
            effect.MeditationExtraMana = contribution.MeditationExtraMana;
            effect.CleanseExtraHealth = contribution.CleanseExtraHealth;
            effect.MoveActionPointDiscount = contribution.MoveActionPointDiscount;
            effect.SpellExtraTFEnergyPercent = contribution.SpellExtraTFEnergyPercent;
            effect.SpellExtraHealthDamagePercent = contribution.SpellExtraHealthDamagePercent;
            effect.CleanseExtraTFEnergyRemovalPercent = contribution.CleanseExtraTFEnergyRemovalPercent;
            effect.SpellMisfireChanceReduction = contribution.SpellMisfireChanceReduction;
            effect.SpellHealthDamageResistance = contribution.SpellHealthDamageResistance;
            effect.SpellTFEnergyDamageResistance = contribution.SpellTFEnergyDamageResistance;
            effect.ExtraInventorySpace = contribution.ExtraInventorySpace;

            // new stats
            effect.Discipline = contribution.Discipline;
            effect.Perception = contribution.Perception;
            effect.Charisma = contribution.Charisma;
            effect.Submission_Dominance = contribution.Submission_Dominance;

            effect.Fortitude = contribution.Fortitude;
            effect.Agility = contribution.Agility;
            effect.Allure = contribution.Allure;
            effect.Corruption_Purity = contribution.Corruption_Purity;

            effect.Magicka = contribution.Magicka;
            effect.Succour = contribution.Succour;
            effect.Luck = contribution.Luck;
            effect.Chaos_Order = contribution.Chaos_Order;

            effectRepo.SaveDbStaticEffect(effect);

            contribution.History += "Published effect on " + DateTime.UtcNow + ".<br>";
            contribution.EffectSourceId = effect.Id;
            contRepo.SaveEffectContribution(contribution);

            ViewBag.Message = message;

            ViewBag.Message += "<br>New effect, " + contribution.Effect_FriendlyName + ", by " + contribution.SubmitterName + ".";

            return View(MVC.Contribution.Views.Publish);
        }

        public virtual ActionResult PublishSpell_Effect(int id)
        {
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            IEffectContributionRepository contRepo = new EFEffectContributionRepository();
            IDbStaticSkillRepository skillRepo = new EFDbStaticSkillRepository();

            var contribution = contRepo.EffectContributions.First(e => e.Id == id);

            if (String.IsNullOrEmpty(contribution.Skill_FriendlyName))
            {
                TempData["Error"] = "There is no spell name set.  Do you mean to publish this?";
                return RedirectToAction(MVC.PvP.Play());
            }

            var staticSkill = skillRepo.DbStaticSkills.FirstOrDefault(s => s.GivesEffectSourceId == contribution.EffectSourceId);

            var message = "";

            if (staticSkill == null)
            {
                staticSkill = new DbStaticSkill();
                message += "<p class='bad'>Made new spell.</p>";
            }
            else
            {
                message += "<p class='good'>Loaded existing spell.</p>";
            }

            staticSkill.Description = contribution.Skill_Description;
            staticSkill.FriendlyName = contribution.Skill_FriendlyName;

            staticSkill.ManaCost = contribution.Skill_ManaCost;
            staticSkill.MobilityType = "curse";

            staticSkill.ExclusiveToFormSourceId = contribution.Skill_UniqueToFormSourceId;
            staticSkill.ExclusiveToItemSourceId = contribution.Skill_UniqueToItemSourceId;

            skillRepo.SaveDbStaticSkill(staticSkill);

            contribution.SkillSourceId = staticSkill.Id;

            contribution.History += "Published spell on " + DateTime.UtcNow + ".<br>";
            contRepo.SaveEffectContribution(contribution);

            // add in any missing FKs for this spell
            DomainRegistry.Repository.Execute(new SetSkillSourceFKs
            {
                SkillSourceId = staticSkill.Id,
                ExclusiveToItemSourceId = staticSkill.ExclusiveToItemSourceId,
                ExclusiveToFormSourceId = staticSkill.ExclusiveToFormSourceId,
                GivesEffectSourceId = contribution.EffectSourceId,
                FormSourceId = staticSkill.FormSourceId
            });

            ViewBag.Message = message;
            return View(MVC.Contribution.Views.Publish);
        }

        public virtual ActionResult MarkAsLive(int id)
        {
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            var message = "";
            IContributionRepository contributionRepo = new EFContributionRepository();
            var contribution = contributionRepo.Contributions.FirstOrDefault(c => c.Id == id);
            var contribution_original = contributionRepo.Contributions.FirstOrDefault(c => c.Id == contribution.ProofreadingCopyForOriginalId);

            contribution.IsLive = true;
            contributionRepo.SaveContribution(contribution);
            message += "<p>Contribution marked as live.</p>";

            if (contribution_original != null)
            {
                contribution_original.IsLive = true;
                contributionRepo.SaveContribution(contribution_original);
                message += "<p>Original contribution marked as live.</p>";

            }
            else
            {
                message += "<p>Original contribution not found.</p>";
            }

            ViewBag.Message = message;
            return View(MVC.Contribution.Views.Publish);
        }

        public virtual ActionResult MarkEffectAsLive(int id)
        {
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            var message = "";
            IEffectContributionRepository contributionRepo = new EFEffectContributionRepository();
            var contribution = contributionRepo.EffectContributions.FirstOrDefault(c => c.Id == id);
            var contribution_original = contributionRepo.EffectContributions.FirstOrDefault(c => c.Id == contribution.ProofreadingCopyForOriginalId);

            contribution.IsLive = true;
            contributionRepo.SaveEffectContribution(contribution);
            message += "<p>Contribution effect marked as live.</p>";

            if (contribution_original != null)
            {
                contribution_original.IsLive = true;
                contributionRepo.SaveEffectContribution(contribution_original);
                message += "<p>Original effect contribution marked as live.</p>";

            }
            else
            {
                message += "<p>Original effect contribution not found.</p>";
            }

            ViewBag.Message = message;
            return View(MVC.Contribution.Views.Publish);
        }

        public virtual ActionResult SetSpellAsLive(int id)
        {
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            IContributionRepository contributionRepo = new EFContributionRepository();
            IDbStaticSkillRepository skillRepo = new EFDbStaticSkillRepository();
            var contribution = contributionRepo.Contributions.FirstOrDefault(c => c.Id == id);

            var sskill = skillRepo.DbStaticSkills.FirstOrDefault(s => s.Id == contribution.SkillSourceId);
            sskill.IsLive = "live";
            skillRepo.SaveDbStaticSkill(sskill);
            ViewBag.Message = "Set to live.";

            return View(MVC.Contribution.Views.Publish);

        }

        public virtual ActionResult MyDMRolls()
        {
            IDMRollRepository repo = new EFDMRollRepository();
            var myMembershipId = User.Identity.GetUserId();
            return View(MVC.Contribution.Views.MyDMRolls, repo.DMRolls.Where(r => r.MembershipOwnerId == myMembershipId));
        }

        [Authorize]
        public virtual ActionResult DMRoll(int id = -1)
        {
            IDMRollRepository repo = new EFDMRollRepository();
            var output = repo.DMRolls.FirstOrDefault(r => r.Id == id);
            if (output == null)
            {
                output = new DMRoll();
            }
            else
            {
                if (output.MembershipOwnerId != User.Identity.GetUserId() && !User.IsInRole(PvPStatics.Permissions_Admin))
                {
                    TempData["Error"] = "This does not belong to you.";
                    return RedirectToAction(MVC.PvP.Play());
                }
            }

            return View(MVC.Contribution.Views.DMRoll, output);
        }

        [Authorize]
        public virtual ActionResult SendDMRoll(DMRoll input)
        {
            IDMRollRepository repo = new EFDMRollRepository();
            var roll = repo.DMRolls.FirstOrDefault(i => i.Id == input.Id);
            var myMembershipId = User.Identity.GetUserId();

            if (roll == null)
            {
                roll = new DMRoll();
            }
            else
            {
                if (roll.MembershipOwnerId != myMembershipId && !User.IsInRole(PvPStatics.Permissions_Admin))
                {
                    TempData["Error"] = "This does not belong to you.";
                    return RedirectToAction(MVC.PvP.Play());
                }
                if (roll.Message.Length > 500)
                {
                    ViewBag["Error"] = "The message canno be longer than 500 characters.";
                    return View(MVC.Contribution.Views.DMRoll, input);
                }
            }

            if (roll.MembershipOwnerId != "0" && roll.MembershipOwnerId != "-1" && roll.MembershipOwnerId != "-2" && !User.IsInRole(PvPStatics.Permissions_Admin))
            {
                roll.MembershipOwnerId = myMembershipId;
            }

            roll.Tags = input.Tags.ToLower();
            roll.Message = input.Message;
            roll.ActionType = input.ActionType;

            if (roll.IsLive)
            {
                roll.IsLive = false;
            }

            repo.SaveDMRoll(roll);

            ViewBag.Result = "DM Encounter saved.";
            return RedirectToAction(MVC.Contribution.MyDMRolls());

        }

        [Authorize]
        public virtual ActionResult ReviewDMRolls()
        {
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }
            IDMRollRepository repo = new EFDMRollRepository();
            return View(MVC.Contribution.Views.ReviewDMRolls, repo.DMRolls.Where(r => !r.IsLive));
        }

        [Authorize]
        public virtual ActionResult ApproveDMRoll(int id)
        {

            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            IDMRollRepository repo = new EFDMRollRepository();
            var roll = repo.DMRolls.FirstOrDefault(i => i.Id == id);
            roll.IsLive = true;
            repo.SaveDMRoll(roll);

            return RedirectToAction(MVC.Contribution.ReviewDMRolls());
        }

        public virtual ActionResult GetContributionTable()
        {
            IContributionRepository contributionRepo = new EFContributionRepository();
            IEffectContributionRepository effectContributionRepo = new EFEffectContributionRepository();

            var output = new List<ContributionCredit>();
            var uniqueOwnerIds = contributionRepo.Contributions.Where(c => c.ProofreadingCopy && c.IsLive && c.OwnerMembershipId != "0" && c.OwnerMembershipId != "-1" && c.OwnerMembershipId != "-2" && c.SubmitterName != null && c.SubmitterName != "" && !c.IsNonstandard).Select(c => c.OwnerMembershipId).Distinct().ToList();


            foreach (var ownerId in uniqueOwnerIds)
            {
                var addme = new ContributionCredit
                {
                    OwnerMembershipId = ownerId,
                };

                var AuthorContribs = contributionRepo.Contributions.Where(c => c.OwnerMembershipId == ownerId && !c.IsNonstandard && c.IsLive && c.ProofreadingCopy).OrderByDescending(c => c.OwnerMembershipId).FirstOrDefault();
                if (AuthorContribs == null) continue;

                addme.AuthorName = AuthorContribs.SubmitterName;

                addme.AnimateFormCount =
                    contributionRepo.Contributions.Count(
                        c =>
                            c.OwnerMembershipId == ownerId && !c.IsNonstandard &&
                            c.Form_MobilityType == PvPStatics.MobilityFull && c.IsLive && c.ProofreadingCopy);

                addme.InanimateFormCount =
                    contributionRepo.Contributions.Count(
                        c =>
                            c.OwnerMembershipId == ownerId && !c.IsNonstandard &&
                            c.Form_MobilityType == PvPStatics.MobilityInanimate && c.IsLive && c.ProofreadingCopy);

                addme.AnimalFormCount =
                    contributionRepo.Contributions.Count(
                        c =>
                            c.OwnerMembershipId == ownerId && !c.IsNonstandard &&
                            c.Form_MobilityType == PvPStatics.MobilityPet && c.IsLive && c.ProofreadingCopy);

                addme.Website = AuthorContribs.SubmitterUrl.IsNullOrEmpty() ? "" : AuthorContribs.SubmitterUrl;

                addme.EffectCount =
                    effectContributionRepo.EffectContributions.Count(
                        c => c.OwnerMemberhipId == ownerId && c.IsLive && c.ProofreadingCopy);

                addme.SpellCount = addme.AnimateFormCount + addme.InanimateFormCount + addme.AnimalFormCount;

                output.Add(addme);
            }

            return View(MVC.Contribution.Views.GetContributionTable, output);
        }



    }
}