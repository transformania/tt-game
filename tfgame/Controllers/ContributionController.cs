using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.Procedures;
using tfgame.Statics;
using tfgame.ViewModels;
using WebMatrix.WebData;
using Microsoft.AspNet.Identity;

namespace tfgame.Controllers
{

    public class ContributionController : Controller
    {
        //
        // GET: /Contribution/
        public ActionResult Index()
        {
            return View();
        }


        [Authorize]
        public ActionResult Contribute(int Id = -1)
        {

            IContributionRepository contributionRepo = new EFContributionRepository();

            int currentUserId = ((User.Identity.GetUserId() != null) ? Convert.ToInt32(User.Identity.GetUserId()) : -1);

            IEnumerable<Contribution> myContributions = contributionRepo.Contributions.Where(c => c.OwnerMembershipId == currentUserId);
            IEnumerable<Contribution> proofreading = null;

            bool iAmProofreader = User.IsInRole(PvPStatics.Permissions_Proofreader);

            // add the rest of the submitted contributions if the player is a proofread
            if (iAmProofreader == true)
            {
                proofreading = contributionRepo.Contributions.Where(c => c.AdminApproved == true && c.ProofreadingCopy == true);
            }

            Contribution contribution;

            if (Id != -1)
            {
                try
                {
                    // contribution = myContributions.FirstOrDefault(c => c.Id == Id);
                    contribution = contributionRepo.Contributions.FirstOrDefault(c => c.Id == Id);
                    ViewBag.Result = "Load successful.";

                    // assert player owns this
                    if (contribution.OwnerMembershipId != currentUserId && iAmProofreader == false)
                    {
                        TempData["Error"] = "This contribution does not belong to your account.";
                        return RedirectToAction("Play", "PvP");
                    }

                    // if this player is a proofreader and this contribution is not marked as ready for proofreading, tell the editor to go to the proofreading version instead.
                    if (iAmProofreader == true && contribution.ProofreadingCopy == false)
                    {
                        Contribution contributionProofed = contributionRepo.Contributions.FirstOrDefault(c => c.OwnerMembershipId == contribution.OwnerMembershipId && c.ProofreadingCopy == true && c.Skill_FriendlyName == contribution.Skill_FriendlyName && c.Form_FriendlyName == contribution.Form_FriendlyName);
                        if (contributionProofed != null)
                        {
                            TempData["Error"] = "There is already a proofreading version of this available.  Please load that instead.";
                            return RedirectToAction("Play", "PvP");
                        }



                    }

                    // save the proofreading lock on this contribution
                    if (contribution.ProofreadingCopy == true)
                    {
                        contribution.ProofreadingLockIsOn = true;
                        contribution.CheckedOutBy = WebSecurity.CurrentUserName;
                        contribution.CreationTimestamp = DateTime.UtcNow;
                        contributionRepo.SaveContribution(contribution);
                    }

                }
                catch
                {
                    contribution = new Contribution();
                    contribution.OwnerMembershipId = ((User.Identity.GetUserId() != null) ? Convert.ToInt32(User.Identity.GetUserId()) : -1);
                }
            }
            else
            {
                contribution = new Contribution();
                contribution.Skill_ManaCost = 7;
                contribution.Form_TFEnergyRequired = 100;
                contribution.Skill_TFPointsAmount = 10;
                contribution.Skill_HealthDamageAmount = 4.5M;
            }

            ViewBag.Result = TempData["Result"];

            ViewBag.OtherContributions = myContributions;
            ViewBag.Proofreading = proofreading;

            BalanceBox bbox = new BalanceBox();
            bbox.LoadBalanceBox(contribution);
            decimal balance = bbox.GetBalance();
            ViewBag.BalanceScore = balance;


            #region for admin use only, see if statics exist
            if (User.IsInRole(PvPStatics.Permissions_Admin) == true && contribution.ProofreadingCopy == true)
            {
                IDbStaticSkillRepository skillRepo = new EFDbStaticSkillRepository();
                IDbStaticFormRepository formRepo = new EFDbStaticFormRepository();
                IDbStaticItemRepository itemRepo = new EFDbStaticItemRepository();

                string skilldbname = "skill_" + contribution.Skill_FriendlyName.Replace(" ", "_") + "_" + contribution.SubmitterName.Replace(" ", "_");
                string formdbname = "form_" + contribution.Form_FriendlyName.Replace(" ", "_") + "_" + contribution.SubmitterName.Replace(" ", "_");

                string itemdbname = "";

                if (contribution.Form_MobilityType == "inanimate")
                {
                    itemdbname = "item_" + contribution.Form_FriendlyName.Replace(" ", "_") + "_" + contribution.SubmitterName.Replace(" ", "_");
                }
                else if (contribution.Form_MobilityType == "animal")
                {
                    itemdbname = "animal_" + contribution.Form_FriendlyName.Replace(" ", "_") + "_" + contribution.SubmitterName.Replace(" ", "_");
                }

                DbStaticSkill sskill = skillRepo.DbStaticSkills.FirstOrDefault(s => s.dbName == skilldbname);
                DbStaticForm sform = formRepo.DbStaticForms.FirstOrDefault(f => f.dbName == formdbname);
                DbStaticItem sitem = itemRepo.DbStaticItems.FirstOrDefault(f => f.dbName == itemdbname);

                if (sskill == null)
                {
                    ViewBag.StaticSkillExists = "<p class='bad'>No static skill found:  " + skilldbname + "</p>";
                }
                else
                {
                    ViewBag.StaticSkillExists += "<p class='good'>Static skill found:  " + skilldbname + "</p>";
                }

                if (sform == null)
                {
                    ViewBag.StaticFormExists = "<p class='bad'>No static form found:  " + formdbname + "</p>";
                }
                else
                {
                    ViewBag.StaticFormExists = "<p class='good'>Static form found:  " + formdbname + "</p>";
                }

                if (sitem == null && (contribution.Form_MobilityType == "inanimate" || contribution.Form_MobilityType == "animal"))
                {
                    ViewBag.StaticItemExists += "<p class='bad'>No static item/pet found:  " + itemdbname + "</p>";
                }
                else if (contribution.Form_MobilityType == "inanimate" || contribution.Form_MobilityType == "animal")
                {
                    ViewBag.StaticItemExists += "<p class='good'>Static item/pet found:  " + itemdbname + "</p>";
                }

            }
            #endregion

            return View(contribution);
        }

        [Authorize]
        public ActionResult ContributePreview(int Id)
        {

            // assert only previewers can view this
            if (User.IsInRole(PvPStatics.Permissions_Previewer) == false)
            {
                return View("Play", "PvP");
            }

            IContributionRepository contributionRepo = new EFContributionRepository();
            Contribution contribution = contributionRepo.Contributions.FirstOrDefault(c => c.Id == Id && c.IsReadyForReview == true && c.ProofreadingCopy == false);
            ViewBag.DisableLinks = true;

            BalanceBox bbox = new BalanceBox();
            bbox.LoadBalanceBox(contribution);
            decimal balance = bbox.GetBalance();
            ViewBag.BalanceScore = balance;

            return View("Contribute", contribution);
        }


        [Authorize]
        public ActionResult ContributeBalanceCalculatorEffect(int id)
        {
            IEffectContributionRepository contributionRepo = new EFEffectContributionRepository();
            EffectContribution contribution = contributionRepo.EffectContributions.FirstOrDefault(c => c.Id == id);

            Player me = PlayerProcedures.GetPlayerFromMembership(((User.Identity.GetUserId() != null) ? Convert.ToInt32(User.Identity.GetUserId()) : -1));
            bool iAmProofreader = User.IsInRole(PvPStatics.Permissions_Proofreader);

            if (iAmProofreader == false && contribution.OwnerMemberhipId != me.MembershipId)
            {
                TempData["Error"] = "That does not belong to you and you are not a proofreader.";
                return RedirectToAction("Play", "PvP");
            }
            else
            {

            }

            return View("~/Views/Contribution/BalanceCalculatorEffect.cshtml", contribution);
        }

        [Authorize]
        public ActionResult ContributeBalanceCalculator2(int id)
        {
            IContributionRepository contributionRepo = new EFContributionRepository();
            Contribution contribution = contributionRepo.Contributions.FirstOrDefault(c => c.Id == id);

            Player me = PlayerProcedures.GetPlayerFromMembership(((User.Identity.GetUserId() != null) ? Convert.ToInt32(User.Identity.GetUserId()) : -1));
            bool iAmProofreader = User.IsInRole(PvPStatics.Permissions_Proofreader);

            if (iAmProofreader == false && contribution.OwnerMembershipId != me.MembershipId)
            {
                TempData["Error"] = "That does not belong to you and you are not a proofreader.";
                return RedirectToAction("Play", "PvP");
            }
            else
            {

            }

            return View("~/Views/Contribution/BalanceCalculator2.cshtml", contribution);
        }

        [Authorize]
        public ActionResult ContributeBalanceCalculatorSend(Contribution input)
        {
            IContributionRepository contributionRepo = new EFContributionRepository();
            Contribution SaveMe = contributionRepo.Contributions.FirstOrDefault(c => c.Id == input.Id);

            Player me = PlayerProcedures.GetPlayerFromMembership(((User.Identity.GetUserId() != null) ? Convert.ToInt32(User.Identity.GetUserId()) : -1));
            bool iAmProofreader = User.IsInRole(PvPStatics.Permissions_Proofreader);

            if (iAmProofreader == false && SaveMe.OwnerMembershipId != me.MembershipId)
            {
                TempData["Error"] = "That does not belong to you and you are not a proofreader.";
                return RedirectToAction("Play", "PvP");
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


                SaveMe.History += "Bonus values edited by " + WebSecurity.CurrentUserName + " on " + DateTime.UtcNow + ".<br>";

                contributionRepo.SaveContribution(SaveMe);

                TempData["Result"] = "Contribution stats saved.";
                return RedirectToAction("Play", "PvP");

            }
        }

        [Authorize]
        public ActionResult ContributeBalanceCalculatorSend_Effect(EffectContribution input)
        {
            IEffectContributionRepository contributionRepo = new EFEffectContributionRepository();
            EffectContribution SaveMe = contributionRepo.EffectContributions.FirstOrDefault(c => c.Id == input.Id);

            Player me = PlayerProcedures.GetPlayerFromMembership(((User.Identity.GetUserId() != null) ? Convert.ToInt32(User.Identity.GetUserId()) : -1));
            bool iAmProofreader = User.IsInRole(PvPStatics.Permissions_Proofreader);

            if (iAmProofreader == false && SaveMe.OwnerMemberhipId != me.MembershipId)
            {
                TempData["Error"] = "That does not belong to you and you are not a proofreader.";
                return RedirectToAction("Play", "PvP");
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
                return RedirectToAction("Play", "PvP");

            }
        }

        [Authorize]
        public ActionResult ContributeGraphicsNeeded()
        {
            IContributionRepository contributionRepo = new EFContributionRepository();
            IEnumerable<Contribution> output = contributionRepo.Contributions.Where(c => c.IsReadyForReview == true && c.AdminApproved == true && c.IsLive == false && c.ProofreadingCopy == true);

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            return View("ContributeGraphicsNeeded", output);
        }

        [Authorize]
        public ActionResult ContributeSetGraphicStatus(int id)
        {

            bool iAmArtist = User.IsInRole(PvPStatics.Permissions_Artist);

            if (iAmArtist == false)
            {
                TempData["Result"] = "You don't have permissions to do that.  If you are an artist and are interested in contributing artwork, please contact the administrator, Judoo.";
                return RedirectToAction("ContributeGraphicsNeeded");
            }

            IContributionRepository contributionRepo = new EFContributionRepository();
            Contribution cont = contributionRepo.Contributions.FirstOrDefault(c => c.Id == id);

            ContributionStatusViewModel output = new ContributionStatusViewModel
            {
                ContributionId = id,
                OwnerMembershipId = cont.OwnerMembershipId,
                Status = cont.AssignedToArtist,
            };

            return View("ContributeSetGraphicStatus", output);
        }

        [Authorize]
        public ActionResult ContributeSetGraphicStatusSubmit(ContributionStatusViewModel input)
        {

            bool iAmArtist = User.IsInRole(PvPStatics.Permissions_Artist);

            if (iAmArtist == false)
            {
                TempData["Result"] = "You don't have permissions to do that.  If you are an artist and are interested in contributing artwork, please contact the administrator, Judoo.";
                return RedirectToAction("ContributeGraphicsNeeded");
            }

            IContributionRepository contributionRepo = new EFContributionRepository();
            Contribution cont = contributionRepo.Contributions.FirstOrDefault(c => c.Id == input.ContributionId);

            cont.AssignedToArtist = input.Status;
            cont.History += "Assigned artist changed by " + WebSecurity.CurrentUserName + " on " + DateTime.UtcNow + ".<br>";

            contributionRepo.SaveContribution(cont);


            TempData["Result"] = "Status saved!";
            return RedirectToAction("ContributeGraphicsNeeded");

        }

        [HttpPost]
        public ActionResult SendContribution(Contribution input)
        {

            IContributionRepository contributionRepo = new EFContributionRepository();
            Contribution SaveMe;

            Session["ContributionId"] = input.Id;

            bool iAmProofreader = User.IsInRole(PvPStatics.Permissions_Proofreader);

            SaveMe = contributionRepo.Contributions.FirstOrDefault(c => c.Id == input.Id);
            if (SaveMe == null)
            {
                SaveMe = new Contribution();
                SaveMe.OwnerMembershipId = ((User.Identity.GetUserId() != null) ? Convert.ToInt32(User.Identity.GetUserId()) : -1);
            }


            if (input.Id != -1)
            {

                // submitter is original author, ID stays the same and do NOT mark as proofreading version
                if (SaveMe != null && SaveMe.OwnerMembershipId == ((User.Identity.GetUserId() != null) ? Convert.ToInt32(User.Identity.GetUserId()) : -1))
                {
                    SaveMe.Id = input.Id;
                }

                // submitter is not original author.  Do more logic...
                else if (SaveMe != null && SaveMe.OwnerMembershipId != ((User.Identity.GetUserId() != null) ? Convert.ToInt32(User.Identity.GetUserId()) : -1))
                {
                    // this is a poorfreading copy.  Keep Id the same and keep it marked as a proofreading copy IF the editor is a proofreader
                    if (SaveMe.ProofreadingCopy == true && iAmProofreader == true)
                    {
                        SaveMe.Id = input.Id;
                        //SaveMe.ProofreadingCopy = true;
                    }
                    else
                    {
                        TempData["Result"] = "You do not have the authorization to edit this.  If you are a proofreader, make sure to load up the proofreading version instead.";
                        return RedirectToAction("Play", "PvP");
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

            SaveMe.CursedTF_FormdbName = input.CursedTF_FormdbName;
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

            //SaveMe.HealthBonusPercent = input.HealthBonusPercent;
            //SaveMe.ManaBonusPercent = input.ManaBonusPercent;
            //SaveMe.ExtraSkillCriticalPercent = input.ExtraSkillCriticalPercent;
            //SaveMe.HealthRecoveryPerUpdate = input.HealthRecoveryPerUpdate;
            //SaveMe.ManaRecoveryPerUpdate = input.ManaRecoveryPerUpdate;
            //SaveMe.SneakPercent = input.SneakPercent;
            //SaveMe.EvasionPercent = input.EvasionPercent;
            //SaveMe.EvasionNegationPercent = input.EvasionNegationPercent;
            //SaveMe.MeditationExtraMana = input.MeditationExtraMana;
            //SaveMe.CleanseExtraHealth = input.CleanseExtraHealth;
            //SaveMe.MoveActionPointDiscount = input.MoveActionPointDiscount;
            //SaveMe.SpellExtraTFEnergyPercent = input.SpellExtraTFEnergyPercent;
            //SaveMe.SpellExtraHealthDamagePercent = input.SpellExtraHealthDamagePercent;
            //SaveMe.CleanseExtraTFEnergyRemovalPercent = input.CleanseExtraTFEnergyRemovalPercent;
            //SaveMe.SpellMisfireChanceReduction = input.SpellMisfireChanceReduction;
            //SaveMe.SpellHealthDamageResistance = input.SpellHealthDamageResistance;
            //SaveMe.SpellTFEnergyDamageResistance = input.SpellTFEnergyDamageResistance;
            //SaveMe.ExtraInventorySpace = input.ExtraInventorySpace;





            SaveMe.SubmitterName = input.SubmitterName;
            SaveMe.SubmitterUrl = input.SubmitterUrl;
            SaveMe.AdditionalSubmitterNames = input.AdditionalSubmitterNames;
            SaveMe.Notes = input.Notes;
            SaveMe.NeedsToBeUpdated = input.NeedsToBeUpdated;
            SaveMe.IsNonstandard = input.IsNonstandard;

            SaveMe.AssignedToArtist = input.AssignedToArtist;

            if (input.ImageURL != null && input.ImageURL != "" && User.IsInRole(PvPStatics.Permissions_Admin) == true)
            {
                SaveMe.ImageURL = input.ImageURL;
            }

            SaveMe.CreationTimestamp = DateTime.UtcNow;

            if (SaveMe.ProofreadingCopy == true)
            {
                SaveMe.History += "Edited by " + WebSecurity.CurrentUserName + " on " + DateTime.UtcNow + ".<br>";
            }

            contributionRepo.SaveContribution(SaveMe);

            #region notify admins

            // Idea here is to notify the admins that there is a new contribution if it is the first time it has been sent in review and the first time only.  (I don't
            // want admins to get spammed if a user edits it 5 times while waiting for it to get approved.)

            //// if contribution is set to ready for review and wasn't before, notify admins to take a look
            //if (SaveMe.IsReadyForReview == false && input.IsReadyForReview == true && input.ProofreadingCopy == false)
            //{
            //    // Judoo
            //    try {
            //        Player derp = PlayerProcedures.GetPlayerFromMembership(69);
            //        PlayerLogProcedures.AddPlayerLog(derp.Id, "<b>A new contribution has been sent in for review by " + input.SubmitterName + " on " + DateTime.UtcNow + ".</b>", true);
            //    } catch {

            //    }

            //    // Mizuho
            //    try
            //    {
            //        Player mizu = PlayerProcedures.GetPlayerFromMembership(3490);
            //        PlayerLogProcedures.AddPlayerLog(mizu.Id, "<b>A new contribution has been sent in for review by " + input.SubmitterName + " on " + DateTime.UtcNow + ".</b>", true);
            //    }
            //    catch
            //    {

            //    }

            //    // Arrhae
            //    try
            //    {
            //        Player Arrhae = PlayerProcedures.GetPlayerFromMembership(251);
            //        PlayerLogProcedures.AddPlayerLog(Arrhae.Id, "<b>A new contribution has been sent in for review by " + input.SubmitterName + " on " + DateTime.UtcNow + ".</b>", true);
            //    }
            //    catch
            //    {

            //    }
            //}
            #endregion

            TempData["Result"] = "Contribution Saved!";
            return RedirectToAction("Play", "PvP");
        }

        [Authorize]
        public ActionResult SendContributionUndoLock(int id)
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(((User.Identity.GetUserId() != null) ? Convert.ToInt32(User.Identity.GetUserId()) : -1));
            bool iAmProofreader = User.IsInRole(PvPStatics.Permissions_Proofreader);

            if (iAmProofreader == false)
            {
                TempData["Error"] = "You must be a proofreader in order to do this.";
                return RedirectToAction("Play", "PvP");
            }

            IContributionRepository contributionRepo = new EFContributionRepository();
            Contribution contribution = contributionRepo.Contributions.FirstOrDefault(c => c.Id == id);

            if (contribution.ProofreadingCopy == false)
            {
                TempData["Error"] = "This is not a proofreading copy.";
                return RedirectToAction("Play", "PvP");
            }

            contribution.ProofreadingLockIsOn = false;
            contribution.CheckedOutBy = "";
            contributionRepo.SaveContribution(contribution);

            return RedirectToAction("Play", "PvP");
        }

        public ActionResult ContributeEffect(int id)
        {
            // get all of this players effect contributions
            IEffectContributionRepository effectContRepo = new EFEffectContributionRepository();

            IEnumerable<EffectContribution> myEffectContributions = effectContRepo.EffectContributions.Where(c => c.OwnerMemberhipId == ((User.Identity.GetUserId() != null) ? Convert.ToInt32(User.Identity.GetUserId()) : -1));
            ViewBag.OtherEffectContributions = myEffectContributions;

            EffectContribution output = effectContRepo.EffectContributions.FirstOrDefault(e => e.Id == id);

            List<EffectContribution> proofreading = new List<EffectContribution>();

            bool iAmProofreader = User.IsInRole(PvPStatics.Permissions_Proofreader);
            bool iAmAdmin = User.IsInRole(PvPStatics.Permissions_Admin);

            // add the rest of the submitted contributions if the player is a proofread
            if (iAmProofreader == true)
            {
                proofreading = effectContRepo.EffectContributions.Where(c => c.ApprovedByAdmin == true && c.ProofreadingCopy == true).ToList();
                ViewBag.Proofreading = proofreading;
            }

            // assert that this is the owner's work or else that the reader is a proofreader and is a proofreading copy

            if (output == null)
            {
                output = new EffectContribution
                {
                    OwnerMemberhipId = ((User.Identity.GetUserId() != null) ? Convert.ToInt32(User.Identity.GetUserId()) : -1),
                    History = "",
                };
            }

            // not new... check for proofreading permissions
            else
            {
                if (output.OwnerMemberhipId != ((User.Identity.GetUserId() != null) ? Convert.ToInt32(User.Identity.GetUserId()) : -1) && (iAmProofreader == false || (output.ProofreadingCopy == false && iAmAdmin == false)))
                {
                    TempData["Error"] = TempData["You do not have permission to view this."];
                    return RedirectToAction("Play", "PvP");
                }
            }


           // if this is a proofreading copy, set the lock
            if (output.ProofreadingCopy)
            {
                output.ProofreadingLockIsOn = true;
                output.CheckedOutBy = WebSecurity.CurrentUserName;
                output.Timestamp = DateTime.UtcNow;
                effectContRepo.SaveEffectContribution(output);
            }




            if (User.IsInRole(PvPStatics.Permissions_Admin) == true && output.ProofreadingCopy == true)
            {

                string effectDbName = output.GetEffectDbName();
                string spellDbName = "NO SKILL NAME SET";

                try {
                    spellDbName = output.GetSkillDbName();
                }
                catch
                {

                }

                ViewBag.StaticEffectExists = "<span class = 'bad'>Static effect for " + effectDbName + " not found!</span>";
                ViewBag.StaticSpellExists = "<span class = 'bad'>Static spell for " + spellDbName + " not found!<span>";

                IDbStaticEffectRepository effectRepo = new EFDbStaticEffectRepository();
                DbStaticEffect possibleEffect = effectRepo.DbStaticEffects.FirstOrDefault(e => e.dbName == effectDbName);
                if (possibleEffect != null)
                {
                    ViewBag.StaticEffectExists = "<span class = 'good'>Static effect " + effectDbName + " exists!</span>";
                }

                IDbStaticSkillRepository skillRepo = new EFDbStaticSkillRepository();
                DbStaticSkill possibleSkill = skillRepo.DbStaticSkills.FirstOrDefault(e => e.dbName == spellDbName);
                if (possibleSkill != null)
                {
                    ViewBag.StaticSpellExists = "<span class = 'good'>Static spell " + spellDbName + " exists!</span>";
                }

            }

            BalanceBox bbox = new BalanceBox();
            bbox.LoadBalanceBox(output);
            decimal balance = bbox.GetBalance__NoModifiersOrCaps();

            try { 
                ViewBag.BalanceScore = balance*output.Effect_Duration;
            }
            catch (DivideByZeroException)
            {
                ViewBag.BalanceScore = "NEEDS DURATION";
            }

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];


            return View(output);
        }

        public ActionResult SendEffectContribution(EffectContribution input)
        {

            IEffectContributionRepository effectContRepo = new EFEffectContributionRepository();
            EffectContribution saveme = effectContRepo.EffectContributions.FirstOrDefault(e => e.Id == input.Id);

            // TODO:  assert player owns this

            bool iAmProofreader = User.IsInRole(PvPStatics.Permissions_Proofreader);

            if (saveme == null)
            {
                saveme = new EffectContribution
                {
                    OwnerMemberhipId = ((User.Identity.GetUserId() != null) ? Convert.ToInt32(User.Identity.GetUserId()) : -1),
                    ReadyForReview = false,
                    ApprovedByAdmin = false,
                    IsLive = false,
                    History = "",
                };

                // make sure this actually is the player's own contribution
            }
            else if (saveme.OwnerMemberhipId != ((User.Identity.GetUserId() != null) ? Convert.ToInt32(User.Identity.GetUserId()) : -1) && iAmProofreader == false)
            {
                TempData["Error"] = "This contribution does not belong to you and you are not a proofreader.";
                TempData["SubError"] = "You may have been logged out; check that you are logged in the the game still in another tab.";
                return RedirectToAction("ContributeEffect", "Contribution", new { @id = -1 });
            }

            if (input.Id != -1)
            {
                saveme.Id = input.Id;
            }

            saveme.SubmitterName = input.SubmitterName;
            saveme.SubmitterURL = input.SubmitterURL;
            saveme.ReadyForReview = input.ReadyForReview;

            saveme.Skill_FriendlyName = input.Skill_FriendlyName;
            saveme.Skill_UniqueToForm = input.Skill_UniqueToForm;
            saveme.Skill_UniqueToItem = input.Skill_UniqueToItem;
            saveme.Skill_UniqueToLocation = input.Skill_UniqueToLocation;
            saveme.Skill_Description = input.Skill_Description;
            saveme.Skill_ManaCost = input.Skill_ManaCost;

            saveme.Effect_FriendlyName = input.Effect_FriendlyName;
            saveme.Effect_Description = input.Effect_Description ;
            saveme.Effect_Duration = input.Effect_Duration ;
            saveme.Effect_Cooldown = input.Effect_Cooldown ;
            saveme.Effect_Bonuses = input.Effect_Bonuses ;
            saveme.Effect_IsRemovable = input.Effect_IsRemovable;
            saveme.Effect_VictimHitText = input.Effect_VictimHitText ;
            saveme.Effect_VictimHitText_M = input.Effect_VictimHitText_M ;
            saveme.Effect_VictimHitText_F = input.Effect_VictimHitText_F ;
            saveme.Effect_AttackHitText = input.Effect_AttackHitText ;
            saveme.Effect_AttackHitText_M = input.Effect_AttackHitText_M ;
            saveme.Effect_AttackHitText_F = input.Effect_AttackHitText_F ;

            saveme.Timestamp = DateTime.UtcNow;
            saveme.AdditionalSubmitterNames = input.AdditionalSubmitterNames;
            saveme.Notes = input.Notes;
            saveme.NeedsToBeUpdated = input.NeedsToBeUpdated;

            if (saveme.ProofreadingCopy)
            {
                saveme.ProofreadingLockIsOn = false;
                saveme.CheckedOutBy = "";
                saveme.History += "Edited by " + WebSecurity.CurrentUserName + " on " + DateTime.UtcNow + ".<br>";
            }
 

            effectContRepo.SaveEffectContribution(saveme);


            //return RedirectToAction("ContributeEffect", "Contribution", new { @id = input.Id });

            TempData["Result"] = "Effect Contribution saved!";
            return RedirectToAction("Play", "PvP");
        }


        public ActionResult UnlockEffectContribution(int id)
        {
            IEffectContributionRepository effectContRepo = new EFEffectContributionRepository();
            EffectContribution saveme = effectContRepo.EffectContributions.FirstOrDefault(e => e.Id == id);
            saveme.ProofreadingLockIsOn = false;
            saveme.CheckedOutBy = "";
            effectContRepo.SaveEffectContribution(saveme);
            return RedirectToAction("Play", "PvP");
        }

        public ActionResult ContributorBioList()
        {
            return View("ContributorBioList");
        }

        public ActionResult ContributorBio(string name)
        {
            return View("~/Views/Contribution/Bios/" + name + ".cshtml");
        }

        public ActionResult PublishSpell(int id)
        {

            if (User.IsInRole(PvPStatics.Permissions_Admin) == false && User.IsInRole(PvPStatics.Permissions_Publisher) == false)
            {
                return View("ContributorBioList");
            }

            string message = "started.<br>";

            IContributionRepository contributionRepo = new EFContributionRepository();
            IDbStaticSkillRepository skillRepo = new EFDbStaticSkillRepository();
            Contribution contribution = contributionRepo.Contributions.FirstOrDefault(c => c.Id == id);

            string skilldbname = contribution.GetSkillDbName();
            string formdbname = "form_" + contribution.Form_FriendlyName.Replace(" ", "_") + "_" + contribution.SubmitterName.Replace(" ", "_");

            DbStaticSkill spell = skillRepo.DbStaticSkills.FirstOrDefault(s => s.dbName == skilldbname);

            if (spell == null)
            {
                spell = new DbStaticSkill();
                spell.dbName = skilldbname;
                spell.FormdbName = formdbname;
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
            string output = "New spell, " + contribution.Skill_FriendlyName + ", submitted by ";

            if (contribution.SubmitterUrl != null && contribution.SubmitterUrl != "")
            {
                output += "<a href=\"" + contribution.SubmitterUrl + "\">" + contribution.SubmitterName + "</a>!";
            }
            else
            {
                output += contribution.SubmitterName + "!";
            }

            if (contribution.AdditionalSubmitterNames != null && contribution.AdditionalSubmitterNames != "")
            {
                output += "  Additional credits go to " + contribution.AdditionalSubmitterNames + ".";
            }

            if (contribution.AssignedToArtist != null && contribution.AssignedToArtist != "")
            {
                output += "  .  Graphic is by " + contribution.AssignedToArtist + ".";
            }

            message += output;

            #endregion

            skillRepo.SaveDbStaticSkill(spell);

            if (contribution.Skill_LearnedAtLocationOrRegion == "location")
            {
                Location temp = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == spell.LearnedAtLocation);
                if (temp == null)
                {
                    message += "<span class='bad'> !!!!! WARNING:  NO LOCATION FOUND FOR THIS SPELL.</span>  ";
                }
            }

            if (contribution.Skill_LearnedAtLocationOrRegion == "region")
            {
                Location temp = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.Region == spell.LearnedAtRegion);
                if (temp == null)
                {
                    message += "<span class='bad'> !!!!! WARNING:  NO REGION FOUND FOR THIS SPELL.</span>  ";
                }
            }

            contribution.History += "Spell published on " + DateTime.UtcNow + "<br>";
            contributionRepo.SaveContribution(contribution);

            ViewBag.Message = message;
            return View("Publish");
        }

        public ActionResult PublishForm(int id)
        {
            if (User.IsInRole(PvPStatics.Permissions_Admin) == false && User.IsInRole(PvPStatics.Permissions_Publisher) == false)
            {
                return View("ContributorBioList");
            }



            string message = "started.<br>";

            IContributionRepository contributionRepo = new EFContributionRepository();
            IDbStaticFormRepository formRepo = new EFDbStaticFormRepository();
            ITFMessageRepository tfRepo = new EFTFMessageRepository();
            Contribution contribution = contributionRepo.Contributions.FirstOrDefault(c => c.Id == id);

            string formdbname = "form_" + contribution.Form_FriendlyName.Replace(" ", "_") + "_" + contribution.SubmitterName.Replace(" ", "_");

            string itemdbname = "";

            if (contribution.Form_MobilityType == "inanimate")
            {
                itemdbname = "item_" + contribution.Form_FriendlyName.Replace(" ", "_") + "_" + contribution.SubmitterName.Replace(" ", "_");
            }
            else if (contribution.Form_MobilityType == "animal")
            {
                itemdbname = "animal_" + contribution.Form_FriendlyName.Replace(" ", "_") + "_" + contribution.SubmitterName.Replace(" ", "_");
            }

            DbStaticForm form = formRepo.DbStaticForms.FirstOrDefault(s => s.dbName == formdbname);
            if (form == null)
            {
                form = new DbStaticForm();
                form.dbName = formdbname;
                message += "<p class='bad'>Wrote NEW form to database.</p>";
            }
            else
            {
                message += "<p class='good'>Loaded existing form from database.</p>";
            }

            TFMessage tf = tfRepo.TFMessages.FirstOrDefault(t => t.FormDbName == formdbname);

            if (tf == null)
            {
                tf = new TFMessage();
                tf.FormDbName = formdbname;
                message += "<p class='bad'>Wrote NEW tf message object to database.</p>";
            }
            else
            {
                message += "<p class='good'>Loaded existing tf message object from database.</p>";
            }

            form.BecomesItemDbName = itemdbname;
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

            if (contribution.Form_MobilityType == "full")
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

            tfRepo.SaveTFMessage(tf);
            formRepo.SaveDbStaticForm(form);
            ViewBag.Message = message;

            contribution.History += "Form published on " + DateTime.UtcNow + "<br>";
            contributionRepo.SaveContribution(contribution);

            PlayerProcedures.LoadFormRAMBuffBox();

            return View("Publish");
        }

        public ActionResult PublishItem(int id)
        {

            if (User.IsInRole(PvPStatics.Permissions_Admin) == false && User.IsInRole(PvPStatics.Permissions_Publisher) == false)
            {
                return View("ContributorBioList");
            }

            string message = "started.<br>";

            IContributionRepository contributionRepo = new EFContributionRepository();
            IDbStaticItemRepository itemRepo = new EFDbStaticItemRepository();
            Contribution contribution = contributionRepo.Contributions.FirstOrDefault(c => c.Id == id);

            string itemdbname = "";

            if (contribution.Form_MobilityType == "inanimate")
            {
                itemdbname = "item_" + contribution.Form_FriendlyName.Replace(" ", "_") + "_" + contribution.SubmitterName.Replace(" ", "_");
            }
            else if (contribution.Form_MobilityType == "animal")
            {
                itemdbname = "animal_" + contribution.Form_FriendlyName.Replace(" ", "_") + "_" + contribution.SubmitterName.Replace(" ", "_");
            }

            DbStaticItem item = itemRepo.DbStaticItems.FirstOrDefault(s => s.dbName == itemdbname);

            if (item == null)
            {
                item = new DbStaticItem();
                item.dbName = itemdbname;
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

            if (contribution.Form_MobilityType == "inanimate" || contribution.Form_MobilityType == "animal")
            {
                item.PortraitUrl = contribution.ImageURL;

                // update the form's graphic too while we're at it.
                IDbStaticFormRepository formRepo = new EFDbStaticFormRepository();
                DbStaticForm form = formRepo.DbStaticForms.FirstOrDefault(f => f.BecomesItemDbName == item.dbName);
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

            item.CurseTFFormdbName = contribution.CursedTF_FormdbName;
            
        //           public decimal InstantHealthRestore { get; set; }
        //public decimal InstantManaRestore { get; set; }
        //public decimal ReuseableHealthRestore { get; set; }
        //public decimal ReuseableManaRestore { get; set; }

            //item.InstantHealthRestore = contribution.Item_

            // write the form to
           // string formdbname = "form_" + contribution.Form_FriendlyName.Replace(" ", "_") + "_" + contribution.SubmitterName.Replace(" ", "_");
            if (item.CurseTFFormdbName != null && item.CurseTFFormdbName.Length > 0) { 
                ITFMessageRepository tfRepo = new EFTFMessageRepository();
                TFMessage tf = tfRepo.TFMessages.FirstOrDefault(t => t.FormDbName == item.CurseTFFormdbName);

                if (tf == null)
                {
                    message += "<p class='bad'>Unable to locate TF message " + item.CurseTFFormdbName + ".  Not writing curse text.  Make sure to publish the form first if needed.</p>";
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

            ViewBag.Message = message;

            contribution.History += "Item published on " + DateTime.UtcNow + "<br>";
            contributionRepo.SaveContribution(contribution);

            ItemProcedures.LoadItemRAMBuffBox();

            return View("Publish");
        }

        public ActionResult PublishEffect(int id)
        {
            if (User.IsInRole(PvPStatics.Permissions_Admin) == false)
            {
                return View("ContributorBioList");
            }

            string message = "";

            IEffectContributionRepository contRepo = new EFEffectContributionRepository();
            IDbStaticEffectRepository effectRepo = new EFDbStaticEffectRepository();

            EffectContribution contribution = contRepo.EffectContributions.FirstOrDefault(e => e.Id == id);
            string effectDbName = contribution.GetEffectDbName();
            DbStaticEffect effect = effectRepo.DbStaticEffects.FirstOrDefault(e => e.dbName == effectDbName);

  
            if (effect == null)
            {
                effect = new DbStaticEffect
                {
                    dbName = contribution.GetEffectDbName(),
                };
                message += "<p class='bad'>Made new effect.</p>";
            }
            else
            {
                message += "<p class='good'>Loaded existing effect.</p>";
            }

            effect.FriendlyName = contribution.Effect_FriendlyName;
            effect.Description = contribution.Effect_Description;
           // effect.AvailableAtLevel = 0;
           // effect.PreRequesite = contribution.Effect_Pre

          //  effect.isLevelUpPerk = contribution.;
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

            // if this is a castable skill, we need to create or update the skill as well.
            if ((contribution.Skill_UniqueToForm != null && contribution.Skill_UniqueToForm != "") || (contribution.Skill_UniqueToItem != null && contribution.Skill_UniqueToLocation != ""))
            {
                IDbStaticSkillRepository skillRepo = new EFDbStaticSkillRepository();

            }

            effectRepo.SaveDbStaticEffect(effect);

            contribution.History += "Published effect on " + DateTime.UtcNow + ".<br>";
            contRepo.SaveEffectContribution(contribution);

            ViewBag.Message = message;

            ViewBag.Message += "<br>New effect, " + contribution.Effect_FriendlyName + ", by " + contribution.SubmitterName + ".";
            EffectProcedures.LoadEffectRAMBuffBox();


            return View("Publish");
        }

        public ActionResult PublishSpell_Effect(int id)
        {
            if (User.IsInRole(PvPStatics.Permissions_Admin) == false)
            {
                return View("ContributorBioList");
            }

            IEffectContributionRepository contRepo = new EFEffectContributionRepository();
            IDbStaticSkillRepository skillRepo = new EFDbStaticSkillRepository();

            EffectContribution contribution = contRepo.EffectContributions.First(e => e.Id == id);
            string effectDbName = contribution.GetEffectDbName();
            DbStaticSkill skill = skillRepo.DbStaticSkills.FirstOrDefault(s => s.GivesEffect == effectDbName);

            string message = "";

            if (skill == null)
            {
                skill = new DbStaticSkill();
                skill.dbName = contribution.GetSkillDbName();
                message += "<p class='bad'>Made new spell.</p>";
            }
            else
            {
                message += "<p class='good'>Loaded existing spell.</p>";
            }

            skill.GivesEffect = contribution.GetEffectDbName();
            skill.Description = contribution.Skill_Description;
            skill.FriendlyName = contribution.Skill_FriendlyName;

            skill.ManaCost = contribution.Skill_ManaCost;
            skill.MobilityType = "curse";

            skill.ExclusiveToForm = contribution.Skill_UniqueToForm;
            skill.ExclusiveToItem = contribution.Skill_UniqueToItem;

            skillRepo.SaveDbStaticSkill(skill);

            contribution.History += "Published spell on " + DateTime.UtcNow + ".<br>";
            contRepo.SaveEffectContribution(contribution);

            ViewBag.Message = message;
            return View("Publish");
        }

        public ActionResult MarkAsLive(int id)
        {
            if (User.IsInRole(PvPStatics.Permissions_Admin) == false)
            {
                return View("ContributorBioList");
            }

            string message = "";
            IContributionRepository contributionRepo = new EFContributionRepository();
            Contribution contribution = contributionRepo.Contributions.FirstOrDefault(c => c.Id == id);
            Contribution contribution_original = contributionRepo.Contributions.FirstOrDefault(c => c.Id == contribution.ProofreadingCopyForOriginalId);

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
            return View("Publish");
        }

        public ActionResult MarkEffectAsLive(int id)
        {
            if (User.IsInRole(PvPStatics.Permissions_Admin) == false)
            {
                return View("ContributorBioList");
            }

            string message = "";
            IEffectContributionRepository contributionRepo = new EFEffectContributionRepository();
            EffectContribution contribution = contributionRepo.EffectContributions.FirstOrDefault(c => c.Id == id);
            EffectContribution contribution_original = contributionRepo.EffectContributions.FirstOrDefault(c => c.Id == contribution.ProofreadingCopyForOriginalId);

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
            return View("Publish");
        }

        public ActionResult SetSpellAsLive(int id)
        {
            if (User.IsInRole(PvPStatics.Permissions_Admin) == false)
            {
                return View("ContributorBioList");
            }

            IContributionRepository contributionRepo = new EFContributionRepository();
            IDbStaticSkillRepository skillRepo = new EFDbStaticSkillRepository();
            Contribution contribution = contributionRepo.Contributions.FirstOrDefault(c => c.Id == id);

            string skilldbname = "skill_" + contribution.Skill_FriendlyName.Replace(" ", "_") + "_" + contribution.SubmitterName.Replace(" ", "_");


            DbStaticSkill sskill = skillRepo.DbStaticSkills.FirstOrDefault(s => s.dbName == skilldbname);
            sskill.IsLive = "live";
            skillRepo.SaveDbStaticSkill(sskill);
            ViewBag.Message = "Set to live.";

            return View("Publish");

        }

        public ActionResult StaticsExist(int id)
        {
            if (User.IsInRole(PvPStatics.Permissions_Admin) == false)
            {
                return View("ContributorBioList");
            }

            IContributionRepository contributionRepo = new EFContributionRepository();
            IDbStaticSkillRepository skillRepo = new EFDbStaticSkillRepository();
            IDbStaticFormRepository formRepo = new EFDbStaticFormRepository();
            IDbStaticItemRepository itemRepo = new EFDbStaticItemRepository();
            Contribution contribution = contributionRepo.Contributions.FirstOrDefault(c => c.Id == id);

            string skilldbname = "skill_" + contribution.Skill_FriendlyName.Replace(" ", "_") + "_" + contribution.SubmitterName.Replace(" ", "_");
            string formdbname = "form_" + contribution.Form_FriendlyName.Replace(" ", "_") + "_" + contribution.SubmitterName.Replace(" ", "_");

            string itemdbname = "";

            if (contribution.Form_MobilityType == "inanimate")
            {
                itemdbname = "item_" + contribution.Form_FriendlyName.Replace(" ", "_") + "_" + contribution.SubmitterName.Replace(" ", "_");
            }
            else if (contribution.Form_MobilityType == "animal")
            {
                itemdbname = "animal_" + contribution.Form_FriendlyName.Replace(" ", "_") + "_" + contribution.SubmitterName.Replace(" ", "_");
            }

            DbStaticSkill sskill = skillRepo.DbStaticSkills.FirstOrDefault(s => s.dbName == skilldbname);
            DbStaticForm sform = formRepo.DbStaticForms.FirstOrDefault(f => f.dbName == formdbname);
            DbStaticItem sitem = itemRepo.DbStaticItems.FirstOrDefault(f => f.dbName == itemdbname);

            string message = "";

            if (sskill == null)
            {
                message += "<p class='bad'>No static skill found:  " + skilldbname + "</p>";
            } else {
                message += "<p class='good'>Static skill found.</p>";
            }

            if (sform == null)
            {
                message += "<p class='bad'>No static form found:  " + formdbname + "</p>";
            }
            else
            {
                message += "<p class='good'>Static form found.</p>";
            }

            if (sitem == null && (contribution.Form_MobilityType == "inanimate" || contribution.Form_MobilityType == "animal"))
            {
                message += "<p class='bad'>No static item/pet found:  " + itemdbname + "</p>";
            }
            else if (contribution.Form_MobilityType == "inanimate" || contribution.Form_MobilityType == "animal")
            {
                message += "<p class='good'>Static item/pet found.</p>";
            }

            ViewBag.Message = message;

            return View("Publish");

        }

        public ActionResult MyDMRolls()
        {
            IDMRollRepository repo = new EFDMRollRepository();
            return View(repo.DMRolls.Where(r => r.MembershipOwnerId == ((User.Identity.GetUserId() != null) ? Convert.ToInt32(User.Identity.GetUserId()) : -1)));
        }

          [Authorize]
        public ActionResult DMRoll(int id)
        {
            IDMRollRepository repo = new EFDMRollRepository();
            DMRoll output = repo.DMRolls.FirstOrDefault(r => r.Id == id);
            if (output == null)
            {
                output = new DMRoll();
            }
            else
            {
                if (output.MembershipOwnerId != ((User.Identity.GetUserId() != null) ? Convert.ToInt32(User.Identity.GetUserId()) : -1) && User.IsInRole(PvPStatics.Permissions_Admin) == false)
                {
                    TempData["Error"] = "This does not belong to you.";
                    return RedirectToAction("Play", "PvP");
                }
            }

            return View(output);
        }

          [Authorize]
        public ActionResult SendDMRoll(DMRoll input)
        {
            IDMRollRepository repo = new EFDMRollRepository();
            DMRoll roll = repo.DMRolls.FirstOrDefault(i => i.Id == input.Id);

            if (roll == null)
            {
                roll = new DMRoll();
            }
            else
            {
                if (roll.MembershipOwnerId != ((User.Identity.GetUserId() != null) ? Convert.ToInt32(User.Identity.GetUserId()) : -1) && User.IsInRole(PvPStatics.Permissions_Admin) == false)
                {
                    TempData["Error"] = "This does not belong to you.";
                    return RedirectToAction("Play", "PvP");
                }
                if (roll.Message.Length > 500)
                {
                    ViewBag["Error"] = "The message canno be longer than 500 characters.";
                    return View("DMRoll", input);
                } 
            }

            if (roll.MembershipOwnerId > 0 && User.IsInRole(PvPStatics.Permissions_Admin) == false)
            {
                roll.MembershipOwnerId = ((User.Identity.GetUserId() != null) ? Convert.ToInt32(User.Identity.GetUserId()) : -1);
            }

            roll.Tags = input.Tags.ToLower();
            roll.Message = input.Message;
            roll.ActionType = input.ActionType;

            if (roll.IsLive == true)
            {
                roll.IsLive = false;
            }

            repo.SaveDMRoll(roll);

            ViewBag.Result = "DM Encounter saved.";
            return RedirectToAction("MyDMRolls");

        }

          [Authorize]
          public ActionResult ReviewDMRolls()
          {
              if (User.IsInRole(PvPStatics.Permissions_Admin) == false)
              {
                  return RedirectToAction("Play", "PvP");
              }
              IDMRollRepository repo = new EFDMRollRepository();
              return View(repo.DMRolls.Where(r => r.IsLive == false));
          }

         [Authorize]
          public ActionResult ApproveDMRoll(int id)
          {

              if (User.IsInRole(PvPStatics.Permissions_Admin) == false)
              {
                  return RedirectToAction("Play", "PvP");
              }

              IDMRollRepository repo = new EFDMRollRepository();
              DMRoll roll = repo.DMRolls.FirstOrDefault(i => i.Id == id);
              roll.IsLive = true;
              repo.SaveDMRoll(roll);

              return RedirectToAction("ReviewDMRolls");
          }

         public ActionResult GetContributionTable()
         {
             IContributionRepository contributionRepo = new EFContributionRepository();
             IEffectContributionRepository effectContributionRepo = new EFEffectContributionRepository();

             List<ContributionCredit> output = new List<ContributionCredit>();
             List<int> uniqueOwnerIds = contributionRepo.Contributions.Where(c => c.ProofreadingCopy == true && c.IsLive == true && c.OwnerMembershipId > 0 && c.SubmitterName != null && c.SubmitterName != "").Select(c => c.OwnerMembershipId).Distinct().ToList();
             

             foreach (int ownerId in uniqueOwnerIds)
             {
                 ContributionCredit addme = new ContributionCredit
                 {
                     OwnerMembershipId = ownerId,
                     AuthorName = contributionRepo.Contributions.Where(c => c.OwnerMembershipId == ownerId && c.IsNonstandard == false && c.IsLive == true && c.ProofreadingCopy == true).OrderByDescending(c => c.OwnerMembershipId).First().SubmitterName,
                     AnimateFormCount = contributionRepo.Contributions.Where(c => c.OwnerMembershipId == ownerId && c.IsNonstandard == false && c.Form_MobilityType == "full" && c.IsLive == true && c.ProofreadingCopy == true).Count(),
                     InanimateFormCount = contributionRepo.Contributions.Where(c => c.OwnerMembershipId == ownerId && c.IsNonstandard == false && c.Form_MobilityType == "inanimate" && c.IsLive == true && c.ProofreadingCopy == true).Count(),
                     AnimalFormCount = contributionRepo.Contributions.Where(c => c.OwnerMembershipId == ownerId && c.IsNonstandard == false && c.Form_MobilityType == "animal" && c.IsLive == true && c.ProofreadingCopy == true).Count(),
                     Website = contributionRepo.Contributions.Where(c => c.OwnerMembershipId == ownerId && c.IsNonstandard == false && c.IsLive == true && c.ProofreadingCopy == true).OrderByDescending(c => c.OwnerMembershipId).First().SubmitterUrl,
                     EffectCount = effectContributionRepo.EffectContributions.Where(c => c.OwnerMemberhipId == ownerId && c.IsLive == true && c.ProofreadingCopy == true).Count(),
                    
                 };
                 addme.SpellCount = addme.AnimateFormCount + addme.InanimateFormCount + addme.AnimalFormCount;
                 output.Add(addme);
             }

             return View(output);
         }

   

	}
}