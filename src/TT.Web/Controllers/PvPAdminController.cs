using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TT.Domain;
using TT.Domain.Abstract;
using TT.Domain.ClassifiedAds.Commands;
using TT.Domain.Concrete;
using TT.Domain.Exceptions;
using TT.Domain.Models;
using TT.Domain.Procedures;
using TT.Domain.Procedures.BossProcedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;
using TT.Domain.Exceptions.RPClassifiedAds;
using TT.Domain.Items.Commands;
using TT.Domain.Items.Queries;
using TT.Domain.Players.Commands;
using TT.Domain.World.Commands;
using TT.Domain.World.DTOs;
using TT.Domain.World.Queries;

namespace TT.Web.Controllers
{
    public partial class PvPAdminController : Controller
    {

        public virtual ActionResult Index()
        {
            // assert only admins can view this
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            ViewBag.Message = TempData["Message"];
            return View();
        }

        /// <summary>
        /// Iterates through all locations in the world and makes sure all connections hook up properly, ie a location that connections to the east does in fact have a location to the that connections back.
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult CheckLocationConsistency()
        {
            #region location checks

            string output = "<h1>Location Errors: </h1><br>";
            List<Location> places = LocationsStatics.LocationList.GetLocation.ToList();

            foreach (Location place in places)
            {
                if (place.Name_North != null)
                {
                    Location x = places.FirstOrDefault(l => l.dbName == place.Name_North);
                    if (x == null)
                    {
                        output += "Location <b>" + place.dbName + " (" + place.Name + ")</b> is referencing location <b>" + place.Name_North + "</b> which is null.<br>";
                    }
                    else
                    {
                        if (x.Name_South != place.dbName)
                        {
                            output += "North:  Location <b>" + place.dbName + "</b> connects to <b>" + x.dbName + "</b> but it doesn't connect back.<br>";
                        }
                    }
                }
                if (place.Name_East != null)
                {
                    Location x = places.FirstOrDefault(l => l.dbName == place.Name_East);
                    if (x == null)
                    {
                        output += "Location <b>" + place.dbName + "(" + place.Name + ")</b> is referencing location <b>" + place.Name_East + "</b> which is null.";
                    }
                    else
                    {
                        if (x.Name_West != place.dbName)
                        {
                            output += "East:  Location <b>" + place.dbName + "</b> connects to <b>" + x.dbName + "</b> but it doesn't connect back.<br>";
                        }
                    }
                }
                if (place.Name_West != null)
                {
                    Location x = places.FirstOrDefault(l => l.dbName == place.Name_West);
                    if (x == null)
                    {
                        output += "Location <b>" + place.dbName + "(" + place.Name + ")</b> is referencing location <b>" + place.Name_West + "</b> which is null.";
                    }
                    else
                    {
                        if (x.Name_East != place.dbName)
                        {
                            output += "West:  Location <b>" + place.dbName + "</b> connects to <b>" + x.dbName + "</b> but it doesn't connect back.<br>";
                        }
                    }
                }
                if (place.Name_South != null)
                {
                    Location x = places.FirstOrDefault(l => l.dbName == place.Name_South);
                    if (x == null)
                    {
                        output += "Location <b>" + place.dbName + "(" + place.Name + ")</b> is referencing location <b>" + place.Name_South + "</b> which is null.";
                    }
                    else
                    {
                        if (x.Name_North != place.dbName)
                        {
                            output += "South:  Location <b>" + place.dbName + "</b> connects to <b>" + x.dbName + "</b> but it doesn't connect back.<br>";
                        }
                    }
                }
            }

            #endregion

            TempData["Message"] = output;
            return RedirectToAction(MVC.PvPAdmin.Index());
        }

        /// <summary>
        /// Admin tool.  Updates some information about the status of the round, including turn number, overall round duration, whether it is chaos mode, or the test server.
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult ChangeWorldStats()
        {
            // assert only admins can view this
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            IPvPWorldStatRepository repo = new EFPvPWorldStatRepository();

            PvPWorldStat output = repo.PvPWorldStats.First();

            return View(output);

        }

        [Authorize]
        public virtual ActionResult ChangeRoundText()
        {

            // assert only admins can view this
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            var game = DomainRegistry.Repository.FindSingle(new GetWorld());

            return View(game);
        }

        /// <summary>
        /// Admin tool.  This permits an administrator to change the current round name/number, ie 'Alpha Round 40', 'Beta Round 3', etc.
        /// </summary>
        /// <param name="input">Name of the new round</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult SendChangeRound(WorldDetail input)
        {

            // assert only admins can view this
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            try
            {
                DomainRegistry.Repository.Execute(new UpdateRoundNumber { RoundNumber = input.RoundNumber });
                PvPStatics.AlphaRound = input.RoundNumber;
                TempData["Result"] = "Round number updated!";
            }
            catch (DomainException e)
            {
                TempData["Error"] = "Error updating round number: " + e.Message;
            }

            return RedirectToAction(MVC.PvP.Play());
        }

        /// <summary>
        /// Admin tool.  Updates some information about the status of the round, including turn number, overall round duration, whether it is chaos mode, or the test server.
        /// </summary>
        public virtual ActionResult ChangeWorldStatsSend(PvPWorldStat input)
        {
            // assert only admins can view this
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            IPvPWorldStatRepository repo = new EFPvPWorldStatRepository();

            PvPWorldStat data = repo.PvPWorldStats.FirstOrDefault(i => i.Id == input.Id);

            data.TurnNumber = input.TurnNumber;
            data.RoundDuration = input.RoundDuration;
            data.ChaosMode = input.ChaosMode;
            data.TestServer = input.TestServer;

            repo.SavePvPWorldStat(data);

            PvPStatics.ChaosMode = data.ChaosMode;
            PvPStatics.RoundDuration = data.RoundDuration;

            TempData["Result"] = "World Data Saved!";
            return RedirectToAction(MVC.PvP.Play());

        }

        public virtual ActionResult ApproveEffectContributionList()
        {

            // assert only admins can view this
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            IEffectContributionRepository effectConRepo = new EFEffectContributionRepository();
            IEnumerable<EffectContribution> output = effectConRepo.EffectContributions.Where(c => !c.IsLive && c.ReadyForReview && !c.ApprovedByAdmin && !c.ProofreadingCopy);

            return View(output);
        }

        public virtual ActionResult ApproveEffectContribution(int id)
        {
            // assert only admins can view this
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            IEffectContributionRepository effectConRepo = new EFEffectContributionRepository();

            EffectContribution OldCopy = effectConRepo.EffectContributions.FirstOrDefault(c => c.Id == id);

            EffectContribution ProofreadCopy = effectConRepo.EffectContributions.FirstOrDefault(c => c.ProofreadingCopy && c.Effect_FriendlyName == OldCopy.Effect_FriendlyName);

            if (ProofreadCopy != null)
            {
                ViewBag.Message = "There's already a proofreading copy for this.";
                return View(MVC.PvPAdmin.Views.Index);
            }
            else
            {
                ProofreadCopy = new EffectContribution();
            }

            OldCopy.ApprovedByAdmin = true;

            effectConRepo.SaveEffectContribution(OldCopy);

            ProofreadCopy.OwnerMemberhipId = OldCopy.OwnerMemberhipId;
            ProofreadCopy.SubmitterName = OldCopy.SubmitterName;
            ProofreadCopy.SubmitterURL = OldCopy.SubmitterURL;
            ProofreadCopy.ReadyForReview = OldCopy.ReadyForReview;

            ProofreadCopy.Skill_FriendlyName = OldCopy.Skill_FriendlyName;
            ProofreadCopy.Skill_UniqueToForm = OldCopy.Skill_UniqueToForm;
            ProofreadCopy.Skill_UniqueToItem = OldCopy.Skill_UniqueToItem;
            ProofreadCopy.Skill_UniqueToLocation = OldCopy.Skill_UniqueToLocation;
            ProofreadCopy.Skill_Description = OldCopy.Skill_Description;
            ProofreadCopy.Skill_ManaCost = OldCopy.Skill_ManaCost;

            ProofreadCopy.Effect_FriendlyName = OldCopy.Effect_FriendlyName;
            ProofreadCopy.Effect_Description = OldCopy.Effect_Description;
            ProofreadCopy.Effect_Duration = OldCopy.Effect_Duration;
            ProofreadCopy.Effect_Cooldown = OldCopy.Effect_Cooldown;
            ProofreadCopy.Effect_Bonuses = OldCopy.Effect_Bonuses;
            ProofreadCopy.Effect_VictimHitText = OldCopy.Effect_VictimHitText;
            ProofreadCopy.Effect_VictimHitText_M = OldCopy.Effect_VictimHitText_M;
            ProofreadCopy.Effect_VictimHitText_F = OldCopy.Effect_VictimHitText_F;
            ProofreadCopy.Effect_AttackHitText = OldCopy.Effect_AttackHitText;
            ProofreadCopy.Effect_AttackHitText_M = OldCopy.Effect_AttackHitText_M;
            ProofreadCopy.Effect_AttackHitText_F = OldCopy.Effect_AttackHitText_F;

            ProofreadCopy.Timestamp = DateTime.UtcNow;
            ProofreadCopy.ProofreadingCopy = true;
            ProofreadCopy.ProofreadingCopyForOriginalId = OldCopy.Id;
            ProofreadCopy.ApprovedByAdmin = true;

            ProofreadCopy.AdditionalSubmitterNames = OldCopy.AdditionalSubmitterNames;
            ProofreadCopy.Notes = OldCopy.Notes;

            // stats
            ProofreadCopy.HealthBonusPercent = OldCopy.HealthBonusPercent;
            ProofreadCopy.ManaBonusPercent = OldCopy.ManaBonusPercent;
            ProofreadCopy.ExtraSkillCriticalPercent = OldCopy.ExtraSkillCriticalPercent;
            ProofreadCopy.HealthRecoveryPerUpdate = OldCopy.HealthRecoveryPerUpdate;
            ProofreadCopy.ManaRecoveryPerUpdate = OldCopy.ManaRecoveryPerUpdate;
            ProofreadCopy.SneakPercent = OldCopy.SneakPercent;
            ProofreadCopy.EvasionPercent = OldCopy.EvasionPercent;
            ProofreadCopy.EvasionNegationPercent = OldCopy.EvasionNegationPercent;
            ProofreadCopy.MeditationExtraMana = OldCopy.MeditationExtraMana;
            ProofreadCopy.CleanseExtraHealth = OldCopy.CleanseExtraHealth;
            ProofreadCopy.MoveActionPointDiscount = OldCopy.MoveActionPointDiscount;
            ProofreadCopy.SpellExtraTFEnergyPercent = OldCopy.SpellExtraTFEnergyPercent;
            ProofreadCopy.SpellExtraHealthDamagePercent = OldCopy.SpellExtraHealthDamagePercent;
            ProofreadCopy.CleanseExtraTFEnergyRemovalPercent = OldCopy.CleanseExtraTFEnergyRemovalPercent;
            ProofreadCopy.SpellMisfireChanceReduction = OldCopy.SpellMisfireChanceReduction;
            ProofreadCopy.SpellHealthDamageResistance = OldCopy.SpellHealthDamageResistance;
            ProofreadCopy.SpellTFEnergyDamageResistance = OldCopy.SpellTFEnergyDamageResistance;
            ProofreadCopy.ExtraInventorySpace = OldCopy.ExtraInventorySpace;

            ProofreadCopy.Discipline = OldCopy.Discipline;
            ProofreadCopy.Perception = OldCopy.Perception;
            ProofreadCopy.Charisma = OldCopy.Charisma;
            ProofreadCopy.Fortitude = OldCopy.Fortitude;
            ProofreadCopy.Agility = OldCopy.Agility;
            ProofreadCopy.Allure = OldCopy.Allure;
            ProofreadCopy.Magicka = OldCopy.Magicka;
            ProofreadCopy.Succour = OldCopy.Succour;
            ProofreadCopy.Luck = OldCopy.Luck;

            effectConRepo.SaveEffectContribution(ProofreadCopy);


            return RedirectToAction(MVC.PvPAdmin.ApproveEffectContributionList());

        }

        public virtual ActionResult PublicBroadcast()
        {
            // assert only admins can view this
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }
            else
            {
                PublicBroadcastViewModel output = new PublicBroadcastViewModel();
                return View(output);
            }
        }

        public virtual ActionResult SendPublicBroadcast(PublicBroadcastViewModel input)
        {
            // assert only admins can view this
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }
            else
            {
                IPlayerRepository playerRepo = new EFPlayerRepository();
                IPlayerLogRepository logRepo = new EFPlayerLogRepository();

                string msg = "<span class='bad'>PUBLIC SERVER NOTE:  " + input.Message + "</span>";

                List<Player> players = playerRepo.Players.Where(p => p.BotId == AIStatics.ActivePlayerBotId).ToList();

                string errors = "";

                foreach (Player p in players)
                {
                    try
                    {
                        PlayerLogProcedures.AddPlayerLog(p.Id, msg, true);
                    }
                    catch (Exception e)
                    {
                        errors += "<p>" + p.GetFullName() + "encountered:  " + e.ToString() + "</p><br/>";
                    }
                }

                TempData["Message"] = errors;

                return RedirectToAction(MVC.PvPAdmin.Index());
            }
        }

        /// <summary>
        /// Admin tool.  Updates the last game update as shown in the header tab.
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult ChangeGameDate()
        {
            // assert only admins can view this
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }
            else
            {
                PublicBroadcastViewModel output = new PublicBroadcastViewModel();
                IPvPWorldStatRepository repo = new EFPvPWorldStatRepository();

                PvPWorldStat stats = repo.PvPWorldStats.FirstOrDefault(p => p.Id != -1);

                if (stats != null)
                {
                    output.Message = stats.GameNewsDate;
                }

                return View(output);
            }
        }

        /// <summary>
        /// Admin tool.  Updates the last game update as shown in the header tab.
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult ChangeGameDateSend(PublicBroadcastViewModel input)
        {
            // assert only admins can view this
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }
            else
            {
                IPvPWorldStatRepository repo = new EFPvPWorldStatRepository();

                PvPWorldStat stats = repo.PvPWorldStats.FirstOrDefault(p => p.Id != -1);
                stats.GameNewsDate = input.Message;
                repo.SavePvPWorldStat(stats);
                PvPStatics.LastGameUpdate = stats.GameNewsDate;

                //   TempData["Message"] = errors;

                return RedirectToAction(MVC.PvPAdmin.Index());
            }
        }

        public virtual ActionResult test()
        {
            // assert only admins can view this
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            return RedirectToAction(MVC.PvPAdmin.Index());
        }

        public virtual ActionResult MigrateItemPortraits()
        {
            // assert only admins can view this
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            IDbStaticItemRepository itemRepo = new EFDbStaticItemRepository();
            IDbStaticFormRepository formRepo = new EFDbStaticFormRepository();

            List<DbStaticForm> forms = formRepo.DbStaticForms.Where(f => f.MobilityType == PvPStatics.MobilityInanimate || f.MobilityType == PvPStatics.MobilityPet).ToList();

            foreach (DbStaticForm form in forms)
            {

                DbStaticItem item = itemRepo.DbStaticItems.FirstOrDefault(i => i.dbName == form.BecomesItemDbName);

                if (item != null)
                {
                    form.PortraitUrl = item.PortraitUrl;
                    formRepo.SaveDbStaticForm(form);
                }
            }

            return RedirectToAction(MVC.PvPAdmin.Index());
        }


        /// <summary>
        /// In case the friendly NPCs don't spawn when round starts or for other reasons, spawn them manually.  NPCs that are already spawned are not spawned twice.
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult SpawnNPCs()
        {
            // assert only admins can view this
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            BossProcedures_Lindella.SpawnLindella();
            BossProcedures_PetMerchant.SpawnPetMerchant();
            BossProcedures_Jewdewfae.SpawnFae();
            BossProcedures_Bartender.SpawnBartender();
            BossProcedures_Loremaster.SpawnLoremaster();

            return RedirectToAction(MVC.PvPAdmin.Index());
        }

        public virtual ActionResult ItemPetJSON()
        {
            // assert only admins or players with JSON pulling can do this
            if (!User.IsInRole(PvPStatics.Permissions_Admin) && !User.IsInRole(PvPStatics.Permissions_JSON))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            IDbStaticItemRepository itemRepo = new EFDbStaticItemRepository();
            return Json(itemRepo.DbStaticItems, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult FormJSON()
        {
            // assert only admins or players with JSON pulling can do this
            if (!User.IsInRole(PvPStatics.Permissions_Admin) && !User.IsInRole(PvPStatics.Permissions_JSON))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            IDbStaticFormRepository formRepo = new EFDbStaticFormRepository();
            return Json(formRepo.DbStaticForms, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult SpellJSON()
        {
            // assert only admins or players with JSON pulling can do this
            if (!User.IsInRole(PvPStatics.Permissions_Admin) && !User.IsInRole(PvPStatics.Permissions_JSON))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            IDbStaticSkillRepository skillRepo = new EFDbStaticSkillRepository();
            IEnumerable<DbStaticSkill> output = skillRepo.DbStaticSkills.ToList();
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult EffectJSON()
        {
            // assert only admins can view this
            if (!User.IsInRole(PvPStatics.Permissions_Admin) && !User.IsInRole(PvPStatics.Permissions_JSON))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            IDbStaticEffectRepository effectRepo = new EFDbStaticEffectRepository();
            return Json(effectRepo.DbStaticEffects, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult FurnitureJSON()
        {
            // assert only admins can view this
            if (!User.IsInRole(PvPStatics.Permissions_Admin) && !User.IsInRole(PvPStatics.Permissions_JSON))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            IDbStaticFurnitureRepository effectRepo = new EFDbStaticFurnitureRepository();
            return Json(effectRepo.DbStaticFurnitures, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult LocationJSON()
        {
            // assert only admins can view this
            if (!User.IsInRole(PvPStatics.Permissions_Admin) && !User.IsInRole(PvPStatics.Permissions_JSON))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            List<Location> locations = LocationsStatics.LocationList.GetLocation.Where(l => !l.Region.Equals("dungeon")).ToList();


            // conceal some data about dungeon location in case whoever pulls this JSON is trying to make a map
            foreach (Location l in locations)
            {
                if (l.Region.Equals("dungeon"))
                {
                    l.X = 0;
                    l.Y = 0;
                    l.Name_East = "";
                    l.Name_North = "";
                    l.Name_South = "";
                    l.Name_West = "";
                }
            }

            return Json(locations, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult ApproveContributionList()
        {
            // assert only admins can view this
            if (!User.IsInRole(PvPStatics.Permissions_Admin) && !User.IsInRole(PvPStatics.Permissions_Previewer))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            IContributionRepository contRepo = new EFContributionRepository();

            IEnumerable<Contribution> output = contRepo.Contributions.Where(c => !c.IsLive && c.IsReadyForReview && !c.AdminApproved && !c.ProofreadingCopy);
            return View(output);

        }

        public virtual ActionResult ApproveContribution(int id)
        {
            // assert only admins or spell approvers can view this
            if (!User.IsInRole(PvPStatics.Permissions_Admin) && !User.IsInRole(PvPStatics.Permissions_SpellApprover))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            IContributionRepository contributionRepo = new EFContributionRepository();

            Contribution OldCopy = contributionRepo.Contributions.FirstOrDefault(c => c.Id == id);

            Contribution ProofreadCopy = contributionRepo.Contributions.FirstOrDefault(c => c.ProofreadingCopy && c.Skill_FriendlyName == OldCopy.Skill_FriendlyName);


            Player owner = PlayerProcedures.GetPlayerFromMembership(OldCopy.OwnerMembershipId);

            if (owner != null)
            {
                PlayerLogProcedures.AddPlayerLog(owner.Id, "<b>A contribution you have submitted has been approved.</b>", true);
            }

            if (ProofreadCopy != null)
            {
                ViewBag.Message = "There's already a proofreading copy for this.";
                return View(MVC.PvPAdmin.Views.Index);
            }
            else
            {
                ProofreadCopy = new Contribution();
            }

            // unlock the proofreading flag since it has been saved


            ProofreadCopy.OwnerMembershipId = OldCopy.OwnerMembershipId;
            ProofreadCopy.IsReadyForReview = true;
            ProofreadCopy.IsLive = false;
            ProofreadCopy.ProofreadingCopy = true;
            ProofreadCopy.CheckedOutBy = "";
            ProofreadCopy.ProofreadingLockIsOn = false;
            ProofreadCopy.AdminApproved = true;

            ProofreadCopy.Skill_FriendlyName = OldCopy.Skill_FriendlyName;
            ProofreadCopy.Skill_FormFriendlyName = OldCopy.Skill_FormFriendlyName;
            ProofreadCopy.Skill_Description = OldCopy.Skill_Description;
            ProofreadCopy.Skill_ManaCost = OldCopy.Skill_ManaCost;
            ProofreadCopy.Skill_TFPointsAmount = OldCopy.Skill_TFPointsAmount;
            ProofreadCopy.Skill_HealthDamageAmount = OldCopy.Skill_HealthDamageAmount;
            ProofreadCopy.Skill_LearnedAtLocationOrRegion = OldCopy.Skill_LearnedAtLocationOrRegion;
            ProofreadCopy.Skill_LearnedAtRegion = OldCopy.Skill_LearnedAtRegion;
            ProofreadCopy.Skill_DiscoveryMessage = OldCopy.Skill_DiscoveryMessage;
            ProofreadCopy.Skill_IsPlayerLearnable = OldCopy.Skill_IsPlayerLearnable;

            ProofreadCopy.Form_FriendlyName = OldCopy.Form_FriendlyName;
            ProofreadCopy.Form_Description = OldCopy.Form_Description;
            ProofreadCopy.Form_TFEnergyRequired = OldCopy.Form_TFEnergyRequired;
            ProofreadCopy.Form_Gender = OldCopy.Form_Gender;
            ProofreadCopy.Form_MobilityType = OldCopy.Form_MobilityType;
            ProofreadCopy.Form_BecomesItemDbName = OldCopy.Form_BecomesItemDbName;
            ProofreadCopy.Form_Bonuses = OldCopy.Form_Bonuses;

            ProofreadCopy.Form_TFMessage_20_Percent_1st = OldCopy.Form_TFMessage_20_Percent_1st;
            ProofreadCopy.Form_TFMessage_40_Percent_1st = OldCopy.Form_TFMessage_40_Percent_1st;
            ProofreadCopy.Form_TFMessage_60_Percent_1st = OldCopy.Form_TFMessage_60_Percent_1st;
            ProofreadCopy.Form_TFMessage_80_Percent_1st = OldCopy.Form_TFMessage_80_Percent_1st;
            ProofreadCopy.Form_TFMessage_100_Percent_1st = OldCopy.Form_TFMessage_100_Percent_1st;
            ProofreadCopy.Form_TFMessage_Completed_1st = OldCopy.Form_TFMessage_Completed_1st;

            ProofreadCopy.Form_TFMessage_20_Percent_1st_M = OldCopy.Form_TFMessage_20_Percent_1st_M;
            ProofreadCopy.Form_TFMessage_40_Percent_1st_M = OldCopy.Form_TFMessage_40_Percent_1st_M;
            ProofreadCopy.Form_TFMessage_60_Percent_1st_M = OldCopy.Form_TFMessage_60_Percent_1st_M;
            ProofreadCopy.Form_TFMessage_80_Percent_1st_M = OldCopy.Form_TFMessage_80_Percent_1st_M;
            ProofreadCopy.Form_TFMessage_100_Percent_1st_M = OldCopy.Form_TFMessage_100_Percent_1st_M;
            ProofreadCopy.Form_TFMessage_Completed_1st_M = OldCopy.Form_TFMessage_Completed_1st_M;

            ProofreadCopy.Form_TFMessage_20_Percent_1st_F = OldCopy.Form_TFMessage_20_Percent_1st_F;
            ProofreadCopy.Form_TFMessage_40_Percent_1st_F = OldCopy.Form_TFMessage_40_Percent_1st_F;
            ProofreadCopy.Form_TFMessage_60_Percent_1st_F = OldCopy.Form_TFMessage_60_Percent_1st_F;
            ProofreadCopy.Form_TFMessage_80_Percent_1st_F = OldCopy.Form_TFMessage_80_Percent_1st_F;
            ProofreadCopy.Form_TFMessage_100_Percent_1st_F = OldCopy.Form_TFMessage_100_Percent_1st_F;
            ProofreadCopy.Form_TFMessage_Completed_1st_F = OldCopy.Form_TFMessage_Completed_1st_F;

            ProofreadCopy.Form_TFMessage_20_Percent_3rd = OldCopy.Form_TFMessage_20_Percent_3rd;
            ProofreadCopy.Form_TFMessage_40_Percent_3rd = OldCopy.Form_TFMessage_40_Percent_3rd;
            ProofreadCopy.Form_TFMessage_60_Percent_3rd = OldCopy.Form_TFMessage_60_Percent_3rd;
            ProofreadCopy.Form_TFMessage_80_Percent_3rd = OldCopy.Form_TFMessage_80_Percent_3rd;
            ProofreadCopy.Form_TFMessage_100_Percent_3rd = OldCopy.Form_TFMessage_100_Percent_3rd;
            ProofreadCopy.Form_TFMessage_Completed_3rd = OldCopy.Form_TFMessage_Completed_3rd;

            ProofreadCopy.Form_TFMessage_20_Percent_3rd_M = OldCopy.Form_TFMessage_20_Percent_3rd_M;
            ProofreadCopy.Form_TFMessage_40_Percent_3rd_M = OldCopy.Form_TFMessage_40_Percent_3rd_M;
            ProofreadCopy.Form_TFMessage_60_Percent_3rd_M = OldCopy.Form_TFMessage_60_Percent_3rd_M;
            ProofreadCopy.Form_TFMessage_80_Percent_3rd_M = OldCopy.Form_TFMessage_80_Percent_3rd_M;
            ProofreadCopy.Form_TFMessage_100_Percent_3rd_M = OldCopy.Form_TFMessage_100_Percent_3rd_M;
            ProofreadCopy.Form_TFMessage_Completed_3rd_M = OldCopy.Form_TFMessage_Completed_3rd_M;

            ProofreadCopy.Form_TFMessage_20_Percent_3rd_F = OldCopy.Form_TFMessage_20_Percent_3rd_F;
            ProofreadCopy.Form_TFMessage_40_Percent_3rd_F = OldCopy.Form_TFMessage_40_Percent_3rd_F;
            ProofreadCopy.Form_TFMessage_60_Percent_3rd_F = OldCopy.Form_TFMessage_60_Percent_3rd_F;
            ProofreadCopy.Form_TFMessage_80_Percent_3rd_F = OldCopy.Form_TFMessage_80_Percent_3rd_F;
            ProofreadCopy.Form_TFMessage_100_Percent_3rd_F = OldCopy.Form_TFMessage_100_Percent_3rd_F;
            ProofreadCopy.Form_TFMessage_Completed_3rd_F = OldCopy.Form_TFMessage_Completed_3rd_F;

            ProofreadCopy.CursedTF_FormdbName = OldCopy.CursedTF_FormdbName;
            ProofreadCopy.CursedTF_Fail = OldCopy.CursedTF_Fail;
            ProofreadCopy.CursedTF_Fail_M = OldCopy.CursedTF_Fail_M;
            ProofreadCopy.CursedTF_Fail_F = OldCopy.CursedTF_Fail_F;
            ProofreadCopy.CursedTF_Succeed = OldCopy.CursedTF_Succeed;
            ProofreadCopy.CursedTF_Succeed_M = OldCopy.CursedTF_Succeed_M;
            ProofreadCopy.CursedTF_Succeed_F = OldCopy.CursedTF_Succeed_F;

            ProofreadCopy.Item_FriendlyName = OldCopy.Item_FriendlyName;
            ProofreadCopy.Item_Description = OldCopy.Item_Description;
            ProofreadCopy.Item_ItemType = OldCopy.Item_ItemType;
            ProofreadCopy.Item_UseCooldown = OldCopy.Item_UseCooldown;
            ProofreadCopy.Item_UsageMessage_Item = OldCopy.Item_UsageMessage_Item;
            ProofreadCopy.Item_UsageMessage_Player = OldCopy.Item_UsageMessage_Player;
            ProofreadCopy.Item_Bonuses = OldCopy.Item_Bonuses;

            ProofreadCopy.SubmitterName = OldCopy.SubmitterName;
            ProofreadCopy.AdditionalSubmitterNames = OldCopy.AdditionalSubmitterNames;
            ProofreadCopy.Notes = OldCopy.Notes;
            ProofreadCopy.SubmitterUrl = OldCopy.SubmitterUrl;

            ProofreadCopy.AssignedToArtist = OldCopy.AssignedToArtist;

            ProofreadCopy.CreationTimestamp = DateTime.UtcNow;

            ProofreadCopy.ProofreadingCopyForOriginalId = OldCopy.Id;

            ProofreadCopy.HealthBonusPercent = OldCopy.HealthBonusPercent;
            ProofreadCopy.ManaBonusPercent = OldCopy.ManaBonusPercent;
            ProofreadCopy.ExtraSkillCriticalPercent = OldCopy.ExtraSkillCriticalPercent;
            ProofreadCopy.HealthRecoveryPerUpdate = OldCopy.HealthRecoveryPerUpdate;
            ProofreadCopy.ManaRecoveryPerUpdate = OldCopy.ManaRecoveryPerUpdate;
            ProofreadCopy.SneakPercent = OldCopy.SneakPercent;
            ProofreadCopy.EvasionPercent = OldCopy.EvasionPercent;
            ProofreadCopy.EvasionNegationPercent = OldCopy.EvasionNegationPercent;
            ProofreadCopy.MeditationExtraMana = OldCopy.MeditationExtraMana;
            ProofreadCopy.CleanseExtraHealth = OldCopy.CleanseExtraHealth;
            ProofreadCopy.MoveActionPointDiscount = OldCopy.MoveActionPointDiscount;
            ProofreadCopy.SpellExtraTFEnergyPercent = OldCopy.SpellExtraTFEnergyPercent;
            ProofreadCopy.SpellExtraHealthDamagePercent = OldCopy.SpellExtraHealthDamagePercent;
            ProofreadCopy.CleanseExtraTFEnergyRemovalPercent = OldCopy.CleanseExtraTFEnergyRemovalPercent;
            ProofreadCopy.SpellMisfireChanceReduction = OldCopy.SpellMisfireChanceReduction;
            ProofreadCopy.SpellHealthDamageResistance = OldCopy.SpellHealthDamageResistance;
            ProofreadCopy.SpellTFEnergyDamageResistance = OldCopy.SpellTFEnergyDamageResistance;
            ProofreadCopy.ExtraInventorySpace = OldCopy.ExtraInventorySpace;


            ProofreadCopy.Discipline = OldCopy.Discipline;
            ProofreadCopy.Perception = OldCopy.Perception;
            ProofreadCopy.Charisma = OldCopy.Charisma;
            ProofreadCopy.Fortitude = OldCopy.Fortitude;
            ProofreadCopy.Agility = OldCopy.Agility;
            ProofreadCopy.Allure = OldCopy.Allure;
            ProofreadCopy.Magicka = OldCopy.Magicka;
            ProofreadCopy.Succour = OldCopy.Succour;
            ProofreadCopy.Luck = OldCopy.Luck;

            ProofreadCopy.ImageURL = OldCopy.ImageURL;

            // ------------------ ORIGINAL COPY ------------------------------


            OldCopy.IsReadyForReview = true;
            OldCopy.IsLive = false;
            OldCopy.AdminApproved = true;
            OldCopy.ProofreadingCopy = false;



            contributionRepo.SaveContribution(ProofreadCopy);
            contributionRepo.SaveContribution(OldCopy);

            ViewBag.Message = "Success.";
            return RedirectToAction(MVC.PvPAdmin.ApproveContributionList());

        }

        public virtual ActionResult RejectContribution(int id)
        {
            // assert only admins or spell approvers can view this
            if (!User.IsInRole(PvPStatics.Permissions_Admin) && !User.IsInRole(PvPStatics.Permissions_SpellApprover))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            IContributionRepository contRepo = new EFContributionRepository();

            Contribution contribution = contRepo.Contributions.FirstOrDefault(i => i.Id == id);
            contribution.IsReadyForReview = false;
            contRepo.SaveContribution(contribution);

            Player owner = PlayerProcedures.GetPlayerFromMembership(contribution.OwnerMembershipId);

            if (owner != null)
            {
                PlayerLogProcedures.AddPlayerLog(owner.Id, "<b>A contribution you have submitted has been rejected.  You should have received or will soon a message explaining why or what else needs to be done before this contribution can be accepted.  If you do not, please message Judoo on the forums.</b>", true);
            }


            return RedirectToAction(MVC.PvPAdmin.ApproveContributionList());
        }

        public virtual ActionResult ServerBalance_Forms()
        {
            List<BalancePageViewModel> output = new List<BalancePageViewModel>();

            foreach (DbStaticForm form in FormStatics.GetAllAnimateForms())
            {
                BalanceBox bbox = new BalanceBox();
                bbox.LoadBalanceBox(form);
                decimal balance = bbox.GetBalance();
                decimal absolute = bbox.GetPointTotal();
                BalancePageViewModel addme = new BalancePageViewModel
                {
                    dbName = form.dbName,
                    FriendlyName = form.FriendlyName,
                    Balance = balance,
                    AbsolutePoints = absolute
                };
                output.Add(addme);
            }

            ViewBag.Text = "Forms";
            return View(MVC.PvPAdmin.Views.ServerBalance, output.OrderByDescending(s => s.Balance));

        }

        public virtual ActionResult ServerBalance_Items()
        {
            List<BalancePageViewModel> output = new List<BalancePageViewModel>();

            foreach (DbStaticItem item in ItemStatics.GetAllNonPetItems())
            {
                BalanceBox bbox = new BalanceBox();
                bbox.LoadBalanceBox(item);
                decimal balance = bbox.GetBalance();
                decimal absolute = bbox.GetPointTotal();
                BalancePageViewModel addme = new BalancePageViewModel
                {
                    dbName = item.dbName,
                    FriendlyName = item.FriendlyName,
                    Balance = balance,
                    AbsolutePoints = absolute
                };
                output.Add(addme);
            }

            ViewBag.Text = "Items";
            return View(MVC.PvPAdmin.Views.ServerBalance, output.OrderByDescending(s => s.Balance));

        }

        public virtual ActionResult ServerBalance_Pets()
        {
            List<BalancePageViewModel> output = new List<BalancePageViewModel>();

            foreach (DbStaticItem item in ItemStatics.GetAllPetItems())
            {
                BalanceBox bbox = new BalanceBox();
                bbox.LoadBalanceBox(item);
                decimal balance = bbox.GetBalance();
                decimal absolute = bbox.GetPointTotal();
                BalancePageViewModel addme = new BalancePageViewModel
                {
                    dbName = item.dbName,
                    FriendlyName = item.FriendlyName,
                    Balance = balance,
                    AbsolutePoints = absolute
                };
                output.Add(addme);
            }

            ViewBag.Text = "Pets";
            return View(MVC.PvPAdmin.Views.ServerBalance, output.OrderByDescending(s => s.Balance));

        }

        public virtual ActionResult ServerBalance_Effects()
        {
            List<BalancePageViewModel> output = new List<BalancePageViewModel>();

            IEffectContributionRepository effectContributionRepo = new EFEffectContributionRepository();
            List<EffectContribution> effectsToAnalyze = effectContributionRepo.EffectContributions.Where(e => e.IsLive && e.ProofreadingCopy).ToList();

            foreach (EffectContribution effect in effectsToAnalyze)
            {
                BalanceBox bbox = new BalanceBox();
                bbox.LoadBalanceBox(effect);
                decimal balance = bbox.GetBalance__NoModifiersOrCaps();
                decimal absolute = bbox.GetPointTotal();
                BalancePageViewModel addme = new BalancePageViewModel
                {
                    dbName = effect.GetEffectDbName(),
                    FriendlyName = effect.Effect_FriendlyName,
                    Balance = balance,
                    AbsolutePoints = absolute
                };
                output.Add(addme);
            }

            ViewBag.Text = "Effects";
            return View(MVC.PvPAdmin.Views.ServerBalance, output.OrderByDescending(s => s.Balance));

        }

        public virtual ActionResult ViewServerLog(int turn)
        {
            IServerLogRepository serverLogRepo = new EFServerLogRepository();
            ServerLog log = serverLogRepo.ServerLogs.FirstOrDefault(t => t.TurnNumber == turn);
            return View(log);
        }

        public virtual ActionResult ViewUpdateLogs()
        {
            IServerLogRepository serverLogRepo = new EFServerLogRepository();
            return View(serverLogRepo.ServerLogs);
        }

        public virtual ActionResult FaeList()
        {

            // assert only admins can view this
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            IJewdewfaeEncounterRepository repo = new EFJewdewfaeEncounterRepository();
            var encounters = repo.JewdewfaeEncounters.ToList();

            return View(encounters);

        }

        public virtual ActionResult WriteFae(int id)
        {
            // assert only admins can view this
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            IJewdewfaeEncounterRepository repo = new EFJewdewfaeEncounterRepository();
            JewdewfaeEncounter encounter = repo.JewdewfaeEncounters.FirstOrDefault(e => e.Id == id);

            if (encounter == null)
            {
                encounter = new JewdewfaeEncounter
                {
                    IsLive = false,
                };
            }

            if (encounter.IntroText == null)
            {
                encounter.IntroText = "";
            }

            if (encounter.FailureText == null)
            {
                encounter.FailureText = "";
            }

            if (encounter.CorrectFormText == null)
            {
                encounter.CorrectFormText = "";
            }


            ViewBag.IntroText = encounter.IntroText.Replace("[", "<").Replace("]", ">");
            ViewBag.FailureText = encounter.FailureText.Replace("[", "<").Replace("]", ">");
            ViewBag.CorrectFormText = encounter.CorrectFormText.Replace("[", "<").Replace("]", ">");

            ViewBag.LocationExists = "";
            ViewBag.FormExists = "";

            Location loc = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == encounter.dbLocationName);

            if (loc == null)
            {
                ViewBag.LocationExists = "<span class='bad'>LOCATION " + encounter.dbLocationName + " DOES NOT EXIST.</span>";
            }
            else
            {
                ViewBag.LocationExists = "<span class='good'>LOCATION " + encounter.dbLocationName + " EXISTs.</span>";
            }

            DbStaticForm form = FormStatics.GetForm(encounter.RequiredForm);

            if (form == null)
            {
                ViewBag.FormExists = "<span class='bad'>FORM " + encounter.RequiredForm + " DOES NOT EXIST.</span>";
            }
            else
            {
                ViewBag.FormExists = "<span class='good'>FORM " + encounter.RequiredForm + " EXISTs.</span>";
            }

            return View(encounter);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult WriteFaeSend(JewdewfaeEncounter input)
        {
            // assert only admins can do this
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            input.RequiredForm = input.RequiredForm.Trim();
            input.dbLocationName = input.dbLocationName.Trim();

            IJewdewfaeEncounterRepository repo = new EFJewdewfaeEncounterRepository();

            JewdewfaeEncounter encounter = repo.JewdewfaeEncounters.FirstOrDefault(f => f.Id == input.Id);

            if (encounter == null)
            {
                encounter = new JewdewfaeEncounter();
            }

            //  encounter.Id = input.Id;
            encounter.dbLocationName = input.dbLocationName;
            encounter.RequiredForm = input.RequiredForm;
            encounter.IsLive = input.IsLive;
            encounter.FailureText = input.FailureText;
            encounter.IntroText = input.IntroText;
            encounter.CorrectFormText = input.CorrectFormText;

            repo.SaveJewdewfaeEncounter(encounter);

            return RedirectToAction(MVC.PvPAdmin.FaeList());


        }

        public virtual ActionResult WriteFaeEncounter()
        {

            // assert only admins can view this
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }


            FairyChallengeBag output = new FairyChallengeBag();

            try
            {
                // load data from the xml
                string filename = AppDomain.CurrentDomain.BaseDirectory + "XMLs/FairyChallengeText/fae_temp.xml";
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(FairyChallengeBag));
                System.IO.StreamReader file = new System.IO.StreamReader(filename);
                output = (FairyChallengeBag)reader.Deserialize(file);
            }
            catch
            {

            }


            return View(output);
        }

        public virtual ActionResult LoadSpecificEncounter(string filename)
        {

            FairyChallengeBag output = new FairyChallengeBag();

            // assert only admins can view this
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            // TODO:  finish this!
            return View(MVC.PvPAdmin.Views.WriteFaeEncounter, output);
        }

        [ValidateInput(false)]
        public virtual ActionResult WriteFaeEncounterSend(FairyChallengeBag input)
        {

            // assert only admins can view this
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            string path = Server.MapPath("~/XMLs/FairyChallengeText/");

            // input.title = "Serialization Overview";
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(FairyChallengeBag));

            System.IO.StreamWriter file = new System.IO.StreamWriter(
                path + "fae_temp.xml");
            writer.Serialize(file, input);
            file.Close();

            ViewBag.Result = "done";

            ViewBag.IntroText = input.IntroText.Replace("[", "<").Replace("]", ">");
            ViewBag.FailureText = input.FailureText.Replace("[", "<").Replace("]", ">");
            ViewBag.CorrectFormText = input.CorrectFormText.Replace("[", "<").Replace("]", ">");



            return View(MVC.PvPAdmin.Views.WriteFaeEncounter, input);
        }

        [Authorize]
        public virtual ActionResult ResetAllPlayersWithIPAddress(string address)
        {

            // assert only admin can view this
            bool iAmModerator = User.IsInRole(PvPStatics.Permissions_Moderator);
            if (!iAmModerator)
            {
                ViewBag.Message = "You aren't allowed to do this.";
                return View(MVC.PvPAdmin.Views.Index);
            }


            IPlayerRepository playerRepo = new EFPlayerRepository();
            List<Player> players = playerRepo.Players.Where(p => p.IpAddress == address).ToList();
            foreach (Player p in players)
            {
                p.IpAddress = "reset";
                playerRepo.SavePlayer(p);
                PlayerLogProcedures.AddPlayerLog(p.Id, "<b class='good'>Server notice:  Your IP address has been reset.</b>", true);
            }

            return View(MVC.PvPAdmin.Views.Play);

        }

        [Authorize]
        public virtual ActionResult ToggleBanOnGlobalChat(int id)
        {

            // assert only admin can view this
            bool iAmModerator = User.IsInRole(PvPStatics.Permissions_Moderator);
            if (!iAmModerator)
            {
                ViewBag.Message = "You aren't allowed to do this.";
                return View(MVC.PvPAdmin.Views.Index);
            }


            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player bannedPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == id);

            if (bannedPlayer.IsBannedFromGlobalChat)
            {

                bannedPlayer.IsBannedFromGlobalChat = false;
                TempData["Result"] = "Ban has been LIFTED.";
            }
            else
            {
                bannedPlayer.IsBannedFromGlobalChat = true;
                TempData["Result"] = "Player has been banned from global chat.";
            }
            playerRepo.SavePlayer(bannedPlayer);

            return RedirectToAction(MVC.PvP.Play());

        }

        public virtual ActionResult Scratchpad()
        {
            return View();
        }

        public virtual ActionResult AuditDonators()
        {

            // assert only admin can view this
            bool iAmAdmin = User.IsInRole(PvPStatics.Permissions_Admin);
            if (!iAmAdmin)
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            string output = "";

            IDonatorRepository repo = new EFDonatorRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();

            foreach (Donator d in repo.Donators.ToList())
            {
                Player player = playerRepo.Players.FirstOrDefault(p => p.MembershipId == d.OwnerMembershipId);

                if (player != null && player.DonatorLevel > 0)
                {
                    output += "Looking at " + player.GetFullName() + ".";

                    if (player.DonatorLevel > d.Tier)
                    {
                        player.DonatorLevel = d.Tier;
                        output += "  Knocking down to tier " + d.Tier + ".  </br>";
                        string message = "<span class='bad'>MESSAGE FROM SERVER:  Your Patreon donation tier has been changed to " + d.Tier + ".  If you feel this is in error, please send a private message to Judoo on the forums or through Patreon.  Thank you for your past support!</span>";
                        PlayerLogProcedures.AddPlayerLog(player.Id, message, true);
                    }
                    else
                    {
                        output += "  Okay at tier " + d.Tier + ".  </br>";
                        string message = "<span class='good'>MESSAGE FROM SERVER:  Your Patreon donation has been processed and remains at Tier " + d.Tier + ".  Thank you for your support!</span>";
                        PlayerLogProcedures.AddPlayerLog(player.Id, message, true);
                    }

                    playerRepo.SavePlayer(player);

                }



            }

            TempData["Result"] = output;
            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult Killswitch()
        {
            if (User.IsInRole(PvPStatics.Permissions_Killswitcher))
            {
                IPvPWorldStatRepository repo = new EFPvPWorldStatRepository();
                PvPWorldStat stats = repo.PvPWorldStats.FirstOrDefault(p => p.Id != -1);
                stats.LastUpdateTimestamp = stats.LastUpdateTimestamp.AddDays(1);
                repo.SavePvPWorldStat(stats);

                IPlayerLogRepository logRepo = new EFPlayerLogRepository();
                Player me = PlayerProcedures.GetPlayerFromMembership("69");
                PlayerLog newlog = new PlayerLog
                {
                    IsImportant = true,
                    Message = "<span class='bad'><b>" + User.Identity.Name + " activated the game pause killswitch.</b></span>",
                    PlayerId = me.Id,
                    Timestamp = DateTime.UtcNow,
                };
                logRepo.SavePlayerLog(newlog);

            }
            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult KillswitchRestore()
        {
            if (User.IsInRole(PvPStatics.Permissions_Admin))
            {
                IPvPWorldStatRepository repo = new EFPvPWorldStatRepository();
                PvPWorldStat stats = repo.PvPWorldStats.FirstOrDefault(p => p.Id != -1);
                stats.LastUpdateTimestamp = DateTime.UtcNow;
                repo.SavePvPWorldStat(stats);
            }
            return RedirectToAction(MVC.PvP.Play());
        }

        [Authorize]
        public virtual ActionResult FindMissingThumbnails()
        {
            if (User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return View();
            }
            else
            {
                ViewBag.Message = "You aren't allowed to do this.";
                return View(MVC.PvPAdmin.Views.Index);
            }
        }

        [Authorize]
        public virtual ActionResult ViewPlayerItems(int id)
        {
            if (User.IsInRole(PvPStatics.Permissions_Admin) || User.IsInRole(PvPStatics.Permissions_Moderator))
            {
                ViewBag.playeritems = ItemProcedures.GetAllPlayerItems(id).OrderByDescending(i => i.dbItem.Level);
                ViewBag.player = PlayerProcedures.GetPlayerFormViewModel(id);
                return View();
            }
            else
            {
                ViewBag.Message = "You aren't allowed to do this.";
                return View(MVC.PvPAdmin.Views.Index);
            }
        }

        [Authorize]
        public virtual ActionResult ViewItemTransferLog(int id)
        {
            if (User.IsInRole(PvPStatics.Permissions_Admin) || User.IsInRole(PvPStatics.Permissions_Moderator))
            {
                ViewBag.item = ItemProcedures.GetItemViewModel(id);
                ViewBag.transferlog = ItemTransferLogProcedures.GetItemTransferLog(id);
                return View();
            }
            else
            {
                ViewBag.Message = "You aren't allowed to do this.";
                return View(MVC.PvPAdmin.Views.Index);
            }
        }

        [Authorize]
        public virtual ActionResult RenamePlayer(int id)
        {
            if (User.IsInRole(PvPStatics.Permissions_Admin) || User.IsInRole(PvPStatics.Permissions_Chaoslord))
            {

                var world = DomainRegistry.Repository.FindSingle(new GetWorld());
                if (!PvPStatics.ChaosMode && !world.TestServer)
                {
                    TempData["Error"] = "The rename tool only works in chaos mode.";
                    return RedirectToAction(MVC.PvP.Play());
                }

                PlayerNameViewModel output = new PlayerNameViewModel();

                if (id != -1)
                {
                    PlayerFormViewModel pm = PlayerProcedures.GetPlayerFormViewModel(id);
                    output.Id = id;
                    output.NewFirstName = pm.Player.FirstName;
                    output.NewLastName = pm.Player.LastName;
                    output.NewForm = pm.Player.Form;
                    output.Level = pm.Player.Level;
                    output.Money = pm.Player.Money;
                }

                return View(output);
            }
            else
            {
                return RedirectToAction(MVC.PvP.Play());
            }
        }

        [Authorize]
        public virtual ActionResult RenamePlayerSend(PlayerNameViewModel input)
        {
            if (User.IsInRole(PvPStatics.Permissions_Admin) || User.IsInRole(PvPStatics.Permissions_Chaoslord))
            {

                var world = DomainRegistry.Repository.FindSingle(new GetWorld());
                if (!PvPStatics.ChaosMode && !world.TestServer)
                {
                    TempData["Error"] = "The rename tool only works in chaos mode.";
                    return RedirectToAction(MVC.PvP.Play());
                }

                IPlayerRepository playerRepo = new EFPlayerRepository();
                Player player = playerRepo.Players.FirstOrDefault(p => p.Id == input.Id);

                string origFirstName = player.FirstName;
                string origLastName = player.LastName;

                if (input.NewFirstName != null && input.NewFirstName.Length > 0)
                {
                    player.FirstName = input.NewFirstName;
                }

                if (input.NewLastName != null && input.NewLastName.Length > 0)
                {
                    player.LastName = input.NewLastName;
                }

                if (input.Level > 0)
                {
                    player.Level = input.Level;
                }

                if (input.Money > 0)
                {
                    player.Money = input.Money;
                }

                if (input.NewForm != null && input.NewForm.Length > 0)
                {
                    IDbStaticFormRepository staticFormRepo = new EFDbStaticFormRepository();
                    DbStaticForm form = staticFormRepo.DbStaticForms.FirstOrDefault(f => f.dbName == input.NewForm);

                    if (form != null && form.MobilityType == PvPStatics.MobilityFull)
                    {
                        DomainRegistry.Repository.Execute(new ChangeForm
                        {
                            PlayerId = player.Id,
                            FormName = form.dbName
                        });

                        player.Form = form.dbName; //TODO: Legacy code
                        player.Health = 99999;
                        player.MaxHealth = 99999;
                    }

                    if (form.MobilityType == PvPStatics.MobilityFull && player.Mobility != PvPStatics.MobilityFull)
                    {
                        IItemRepository itemRepo = new EFItemRepository();
                        Item item = itemRepo.Items.FirstOrDefault(i => i.VictimName == origFirstName + " " + origLastName);
                        player.Mobility = PvPStatics.MobilityFull;
                        itemRepo.DeleteItem(item.Id);
                    }

                }

                playerRepo.SavePlayer(player);

                if (player.Mobility != PvPStatics.MobilityFull)
                {
                    IItemRepository itemRepo = new EFItemRepository();
                    Item item = itemRepo.Items.FirstOrDefault(i => i.VictimName == origFirstName + " " + origLastName);
                    item.VictimName = input.NewFirstName + " " + input.NewLastName;
                    itemRepo.SaveItem(item);
                }

                // if Donna, give player her spells
                if (player.Form == BossProcedures_Donna.DonnaDbForm)
                {
                    SkillProcedures.GiveSkillToPlayer(player.Id, BossProcedures_Donna.Spell1);
                    SkillProcedures.GiveSkillToPlayer(player.Id, BossProcedures_Donna.Spell2);
                    SkillProcedures.GiveSkillToPlayer(player.Id, BossProcedures_Donna.Spell3);
                    SkillProcedures.GiveSkillToPlayer(player.Id, BossProcedures_Donna.Spell4);
                    SkillProcedures.GiveSkillToPlayer(player.Id, BossProcedures_Donna.Spell5);
                }

                // if Valentine, give player his spells
                else if (player.Form == BossProcedures_Valentine.ValentineFormDbName)
                {
                    SkillProcedures.GiveSkillToPlayer(player.Id, BossProcedures_Valentine.BloodyCurseSpell);
                    SkillProcedures.GiveSkillToPlayer(player.Id, BossProcedures_Valentine.ValentinesPresenceSpell);
                    SkillProcedures.GiveSkillToPlayer(player.Id, BossProcedures_Valentine.SwordSpell);
                    SkillProcedures.GiveSkillToPlayer(player.Id, BossProcedures_Valentine.DayVampireFemaleSpell);
                    SkillProcedures.GiveSkillToPlayer(player.Id, BossProcedures_Valentine.NightVampireFemaleSpell);
                    SkillProcedures.GiveSkillToPlayer(player.Id, BossProcedures_Valentine.DayVampireMaleSpell);
                    SkillProcedures.GiveSkillToPlayer(player.Id, BossProcedures_Valentine.NightVampireMaleSpell);
                }

                // if plague mother, give player her spells
                else if (player.Form == BossProcedures_BimboBoss.BossFormDbName)
                {
                    SkillProcedures.GiveSkillToPlayer(player.Id, BossProcedures_BimboBoss.RegularTFSpellDbName);
                }

                // if master rat thief, give player her spells
                else if (player.Form == BossProcedures_Thieves.FemaleBossFormDbName)
                {
                    SkillProcedures.GiveSkillToPlayer(player.Id, BossProcedures_Thieves.GoldenTrophySpellDbName);
                }

                // mouse sisters have no unique spells yet.


            }
            else
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            TempData["Result"] = "Yay!";
            return RedirectToAction(MVC.PvP.Play());
        }

        [Authorize]
        public virtual ActionResult ModDeleteClassified(int id)
        {
            if (!User.IsInRole(PvPStatics.Permissions_Admin) && !User.IsInRole(PvPStatics.Permissions_Moderator))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            try
            {
                DomainRegistry.Repository.Execute(new DeleteRPClassifiedAd() { RPClassifiedAdId = id, CheckUserId = false });
            }
            catch (RPClassifiedAdException ex)
            {
                TempData["Error"] = ex.UserFriendlyError ?? ex.Message;
                TempData["SubError"] = ex.UserFriendlySubError;
                return RedirectToAction(MVC.Info.RecentRPClassifieds());
            }

            TempData["Result"] = "Delete successful.";
            return RedirectToAction(MVC.Info.RecentRPClassifieds());
        }

        [Authorize]
        public virtual ActionResult FastInanimateMe()
        {
            string myMembershipId = User.Identity.GetUserId();
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            var world = DomainRegistry.Repository.FindSingle(new GetWorld());
            if (!world.TestServer && !PvPStatics.ChaosMode)
            {
                TempData["Error"] = "Cant' do this in live non-chaos server.";
                return RedirectToAction(MVC.PvP.Play());
            }

            IPlayerRepository playerRepo = new EFPlayerRepository();
            IItemRepository itemRepo = new EFItemRepository();

            Player me = playerRepo.Players.FirstOrDefault(p => p.MembershipId == myMembershipId);
            DomainRegistry.Repository.Execute(new ChangeForm
            {
                PlayerId = me.Id,
                FormName = "form_Flirty_Three-Tiered_Skirt_Martiandawn"
            });

            // delete old item you are if you are one
            Item possibleMeItem = itemRepo.Items.FirstOrDefault(i => i.VictimName == me.FirstName + " " + me.LastName); // DO NOT use GetFullName.  It will break things here.
            if (possibleMeItem != null)
            {
                itemRepo.DeleteItem(possibleMeItem.Id);
            }

            var cmd = new CreateItem
            {
                dbLocationName = me.dbLocationName,
                dbName = "item_Flirty_Three-Tiered_Skirt_Martiandawn",
                VictimName = me.FirstName + " " + me.LastName, // DO NOT use GetFullName.  It will break things here.
                FormerPlayerId = me.Id,
                Nickname = me.Nickname,
                OwnerId = null,
                IsEquipped = false,
                Level = me.Level,
                ItemSourceId = ItemStatics.GetStaticItem("item_Flirty_Three-Tiered_Skirt_Martiandawn").Id
            };

            DomainRegistry.Repository.Execute(cmd);

            TempData["Result"] = "You are inanimate.";
            return RedirectToAction(MVC.PvP.Play());

        }

        [Authorize]
        public virtual ActionResult FastPetMe()
        {
            string myMembershipId = User.Identity.GetUserId();
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            var world = DomainRegistry.Repository.FindSingle(new GetWorld());
            if (!world.TestServer && !PvPStatics.ChaosMode)
            {
                TempData["Error"] = "Cant' do this in live non-chaos server.";
                return RedirectToAction(MVC.PvP.Play());
            }

            IPlayerRepository playerRepo = new EFPlayerRepository();
            IItemRepository itemRepo = new EFItemRepository();

            Player me = playerRepo.Players.FirstOrDefault(p => p.MembershipId == myMembershipId);
            DomainRegistry.Repository.Execute(new ChangeForm
            {
                PlayerId = me.Id,
                FormName = "form_Cuddly_Pocket_Goo_Girl_GooGirl"
            });

            // delete old item you are if you are one
            Item possibleMeItem = itemRepo.Items.FirstOrDefault(i => i.VictimName == me.FirstName + " " + me.LastName);
            if (possibleMeItem != null)
            {
                itemRepo.DeleteItem(possibleMeItem.Id);
            }

            var cmd = new CreateItem
            {
                dbLocationName = me.dbLocationName,
                dbName = "animal_Cuddly_Pocket_Goo_Girl_GooGirl",
                VictimName = me.FirstName + " " + me.LastName,
                FormerPlayerId = me.Id,
                Nickname = me.Nickname,
                OwnerId = null,
                IsEquipped = false,
                Level = me.Level,
                ItemSourceId = ItemStatics.GetStaticItem("animal_Cuddly_Pocket_Goo_Girl_GooGirl").Id
            };
            DomainRegistry.Repository.Execute(cmd);

            TempData["Result"] = "You are now a pet.";
            return RedirectToAction(MVC.PvP.Play());

        }

        [Authorize]
        public virtual ActionResult FastAnimateMe()
        {
            string myMembershipId = User.Identity.GetUserId();
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            var world = DomainRegistry.Repository.FindSingle(new GetWorld());
            if (!world.TestServer && !PvPStatics.ChaosMode)
            {
                TempData["Error"] = "Cant' do this in live non-chaos server.";
                return RedirectToAction(MVC.PvP.Play());
            }

            IPlayerRepository playerRepo = new EFPlayerRepository();

            Player me = playerRepo.Players.FirstOrDefault(p => p.MembershipId == myMembershipId);
            DomainRegistry.Repository.Execute(new ChangeForm
            {
                PlayerId = me.Id,
                FormName = me.OriginalForm
            });

            // delete old item you are if you are one
            var item = DomainRegistry.Repository.FindSingle(new GetItemByVictimName { FirstName = me.FirstName, LastName = me.LastName });
            if (item != null)
            {
                DomainRegistry.Repository.Execute(new DeleteItem { ItemId = item.Id });
            }

            TempData["Result"] = "You are now fully animate.";
            return RedirectToAction(MVC.PvP.Play());

        }

        [Authorize]
        public virtual ActionResult FastGiveTPScroll()
        {
            string myMembershipId = User.Identity.GetUserId();
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            IPvPWorldStatRepository repo = new EFPvPWorldStatRepository();
            PvPWorldStat stat = repo.PvPWorldStats.First();

            bool test = stat.TestServer;

            if (!PvPStatics.ChaosMode && !test)
            {
                TempData["Error"] = "Cannot be done on live server outside of chaos..";
                return RedirectToAction(MVC.PvP.Play());
            }

            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            var cmd = new CreateItem
            {
                dbName = "item_consumeable_teleportation_scroll",
                OwnerId = me.Id,
                dbLocationName = "",
                EquippedThisTurn = false,
                LastSouledTimestamp = DateTime.UtcNow,
                VictimName = "",
                Level = 0,
                ItemSourceId = ItemStatics.GetStaticItem("item_consumeable_teleportation_scroll").Id
            };

            DomainRegistry.Repository.Execute(cmd);

            TempData["Result"] = "You used your admin magic to give yourself a teleportation scroll.";
            return RedirectToAction(MVC.PvP.Play());

        }

        [Authorize]
        public virtual ActionResult AssignLeadersBadges()
        {

            string myMembershipId = User.Identity.GetUserId();
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            if (PvPStatics.ChaosMode)
            {
                TempData["Error"] = "Can't do this in chaos mode.";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (PvPWorldStatProcedures.GetWorldTurnNumber() != PvPStatics.RoundDuration)
            {
                TempData["Error"] = "Turn must be the final turn of the round for this to work.";
                return RedirectToAction(MVC.PvP.Play());
            }

            string output = StatsProcedures.AssignLeadersBadges();

            TempData["Result"] = output;
            return RedirectToAction(MVC.PvP.Play());
        }

        /// <summary>
        /// List all of the custom forms earned from players' contributions available for admins to edit
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public virtual ActionResult ListCustomForms()
        {
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            IContributorCustomFormRepository customFormRepo = new EFContributorCustomFormRepository();
            var output = customFormRepo.ContributorCustomForms;

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            return View(output);
        }

        [Authorize]
        public virtual ActionResult EditCustomForm(int Id)
        {
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            IContributorCustomFormRepository customFormRepo = new EFContributorCustomFormRepository();
            var output = customFormRepo.ContributorCustomForms.FirstOrDefault(c => c.Id == Id);

            if (output == null)
            {
                output = new ContributorCustomForm { Id = 0, OwnerMembershipId = "" };
            }

            return View(output);
        }

        [Authorize]
        public virtual ActionResult EditCustomFormSend(ContributorCustomForm input)
        {
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            IContributorCustomFormRepository customFormRepo = new EFContributorCustomFormRepository();
            IDbStaticFormRepository formRepo = new EFDbStaticFormRepository();

            DbStaticForm form = formRepo.DbStaticForms.FirstOrDefault(f => f.Id == input.CustomForm.Id);

            // assert form actually does exist
            if (form == null)
            {
                TempData["Error"] = "No form found for Form Id: " + input.CustomForm.Id;
                return RedirectToAction(MVC.PvPAdmin.ListCustomForms());
            }

            // assert form is not null
            if (form.MobilityType != PvPStatics.MobilityFull)
            {
                TempData["Error"] = "Form " + form.FriendlyName + " is not animate.";
                return RedirectToAction(MVC.PvPAdmin.ListCustomForms());
            }

            var editMe = customFormRepo.ContributorCustomForms.FirstOrDefault(c => c.Id == input.Id);

            if (editMe == null)
            {
                editMe = new ContributorCustomForm();
            }

            editMe.OwnerMembershipId = input.OwnerMembershipId;
            editMe.CustomForm_Id = form.Id;

            customFormRepo.SaveContributorCustomForm(editMe);

            TempData["Result"] = "Custom form successfully saved!";
            return RedirectToAction(MVC.PvPAdmin.ListCustomForms());
        }

        [Authorize]
        public virtual ActionResult DeleteCustomForm(int Id)
        {
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            IContributorCustomFormRepository customFormRepo = new EFContributorCustomFormRepository();
            customFormRepo.DeleteContributorCustomForm(Id);

            TempData["Result"] = "Deleted custom form Id " + Id;
            return RedirectToAction(MVC.PvPAdmin.ListCustomForms());
        }

        /// <summary>
        /// List all of the News posts available for admins to edit
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public virtual ActionResult ListNewsPosts()
        {
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            INewsPostRepository repo = new EFNewsPostRepository();
            var output = repo.NewsPosts.OrderByDescending(a => a.Timestamp);

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            return View(output);
        }

        /// <summary>
        /// Make changes to an existing or new NewsPost
        /// </summary>
        /// <param name="Id">Id of the news post to make changes to.  -1 indicates a new post.</param>
        /// <returns></returns>
        [Authorize]
        public virtual ActionResult EditNewsPost(int Id)
        {
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            INewsPostRepository repo = new EFNewsPostRepository();
            var output = repo.NewsPosts.FirstOrDefault(f => f.Id == Id);

            if (output == null)
            {
                output = new NewsPost();
            }

            return View(output);

        }

        /// <summary>
        /// Submit a NewsPost for revisions.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize]
        [ValidateInput(false)]
        public virtual ActionResult EditNewsPostSend(NewsPost input)
        {
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            INewsPostRepository repo = new EFNewsPostRepository();
            var saveMe = repo.NewsPosts.FirstOrDefault(f => f.Id == input.Id);

            if (saveMe == null)
            {
                saveMe = new NewsPost();
            }

            saveMe.Timestamp = input.Timestamp;
            saveMe.Text = input.Text;
            saveMe.ViewState = input.ViewState;
            repo.SaveNewsPost(saveMe);

            TempData["Result"] = "News Post " + input.Id + " saved successfully!";
            return RedirectToAction(MVC.PvPAdmin.ListNewsPosts());

        }

        /// <summary>
        /// Deletes a news post
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public virtual ActionResult DeleteNewsPost(int Id)
        {
            string myMembershipId = User.Identity.GetUserId();
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            INewsPostRepository repo = new EFNewsPostRepository();
            repo.DeleteNewsPost(Id);

            TempData["Result"] = "News Post " + Id + " deleted successfully!";
            return RedirectToAction(MVC.PvPAdmin.ListNewsPosts());
        }
    }
}