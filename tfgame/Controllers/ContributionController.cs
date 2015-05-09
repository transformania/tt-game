using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.Filters;
using tfgame.Procedures;
using tfgame.Statics;
using tfgame.ViewModels;
using WebMatrix.WebData;

namespace tfgame.Controllers
{

     [InitializeSimpleMembership]
    public class ContributionController : Controller
    {
        //
        // GET: /Contribution/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ContributeEffect(int id)
        {
            // get all of this players effect contributions
            IEffectContributionRepository effectContRepo = new EFEffectContributionRepository();

            IEnumerable<EffectContribution> myEffectContributions = effectContRepo.EffectContributions.Where(c => c.OwnerMemberhipId == WebSecurity.CurrentUserId);
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
                    OwnerMemberhipId = WebSecurity.CurrentUserId,
                    History = "",
                };
            }

            // not new... check for proofreading permissions
            else
            {
                if (output.OwnerMemberhipId != WebSecurity.CurrentUserId && (iAmProofreader == false || (output.ProofreadingCopy == false && iAmAdmin == false)))
                {
                    TempData["Error"] = TempData["You do not have permission to view this."];
                    return RedirectToAction("Play", "PvPController");
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
            catch
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
                    OwnerMemberhipId = WebSecurity.CurrentUserId,
                    ReadyForReview = false,
                    ApprovedByAdmin = false,
                    IsLive = false,
                    History = "",
                };

                // make sure this actually is the player's own contribution
            }
            else if (saveme.OwnerMemberhipId != WebSecurity.CurrentUserId && iAmProofreader == false)
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
            item.Findable = false;

            if (contribution.Form_MobilityType == "inanimate" || contribution.Form_MobilityType == "animal")
            {
                item.PortraitUrl = contribution.ImageURL;
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
            
        //           public decimal InstantHealthRestore { get; set; }
        //public decimal InstantManaRestore { get; set; }
        //public decimal ReuseableHealthRestore { get; set; }
        //public decimal ReuseableManaRestore { get; set; }

            //item.InstantHealthRestore = contribution.Item_

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
            return View(repo.DMRolls.Where(r => r.MembershipOwnerId == WebSecurity.CurrentUserId));
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
                if (output.MembershipOwnerId != WebSecurity.CurrentUserId && User.IsInRole(PvPStatics.Permissions_Admin) == false)
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
                if (roll.MembershipOwnerId != WebSecurity.CurrentUserId && User.IsInRole(PvPStatics.Permissions_Admin) == false)
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
                roll.MembershipOwnerId = WebSecurity.CurrentUserId;
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