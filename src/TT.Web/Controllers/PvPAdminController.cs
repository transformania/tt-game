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
using TT.Domain.Identity.Queries;
using TT.Domain.Items.Commands;
using TT.Domain.Items.Queries;
using TT.Domain.Legacy.Procedures.BossProcedures;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Queries;
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
            return View(MVC.PvPAdmin.Views.Index);
        }

        /// <summary>
        /// Iterates through all locations in the world and makes sure all connections hook up properly, ie a location that connections to the east does in fact have a location to the that connections back.
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult CheckLocationConsistency()
        {
            #region location checks

            var output = "<h1>Location Errors: </h1><br>";
            var places = LocationsStatics.LocationList.GetLocation.ToList();

            foreach (var place in places)
            {
                if (place.Name_North != null)
                {
                    var x = places.FirstOrDefault(l => l.dbName == place.Name_North);
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
                    var x = places.FirstOrDefault(l => l.dbName == place.Name_East);
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
                    var x = places.FirstOrDefault(l => l.dbName == place.Name_West);
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
                    var x = places.FirstOrDefault(l => l.dbName == place.Name_South);
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

            var output = repo.PvPWorldStats.First();

            return View(MVC.PvPAdmin.Views.ChangeWorldStats, output);

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

            return View(MVC.PvPAdmin.Views.ChangeRoundText, game);
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

            var data = repo.PvPWorldStats.FirstOrDefault(i => i.Id == input.Id);

            data.TurnNumber = input.TurnNumber;
            data.RoundDuration = input.RoundDuration;
            data.ChaosMode = input.ChaosMode;
            data.TestServer = input.TestServer;
            data.RoundStartsAt = input.RoundStartsAt;
            data.TurnTimeConfiguration = input.TurnTimeConfiguration;

            repo.SavePvPWorldStat(data);

            PvPStatics.ChaosMode = data.ChaosMode;
            PvPStatics.RoundDuration = data.RoundDuration;

            if (TurnTimesStatics.IsValidConfiguration(data.TurnTimeConfiguration))
            {
                TurnTimesStatics.ActiveConfiguration = data.TurnTimeConfiguration;
            }
            else
            {
                TurnTimesStatics.ActiveConfiguration = TurnTimesStatics.FiveMinuteTurns;
                TempData["Error"] = $"'{data.TurnTimeConfiguration}' is not a valid configuration.  Falling back to five minute turns.";
                return RedirectToAction(MVC.PvP.Play());
            }
            

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

            return View(MVC.PvPAdmin.Views.ApproveEffectContributionList, output);
        }

        public virtual ActionResult ApproveEffectContribution(int id)
        {
            // assert only admins can view this
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            IEffectContributionRepository effectConRepo = new EFEffectContributionRepository();

            var OldCopy = effectConRepo.EffectContributions.FirstOrDefault(c => c.Id == id);

            var ProofreadCopy = effectConRepo.EffectContributions.FirstOrDefault(c => c.ProofreadingCopy && c.Effect_FriendlyName == OldCopy.Effect_FriendlyName);

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
            ProofreadCopy.Skill_UniqueToFormSourceId = OldCopy.Skill_UniqueToFormSourceId;
            ProofreadCopy.Skill_UniqueToItemSourceId = OldCopy.Skill_UniqueToItemSourceId;
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
                var output = new PublicBroadcastViewModel();
                return View(MVC.PvPAdmin.Views.PublicBroadcast, output);
            }
        }

        public virtual ActionResult SendPublicBroadcast(PublicBroadcastViewModel input)
        {
            // assert only admins can perform this
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }
            else
            {
                IPlayerRepository playerRepo = new EFPlayerRepository();
                IPlayerLogRepository logRepo = new EFPlayerLogRepository();

                var msg = "<span class='bad'>PUBLIC SERVER NOTE:  " + input.Message + "</span>";

                var players = playerRepo.Players.Where(p => p.BotId == AIStatics.ActivePlayerBotId).ToList();

                var errors = "";

                foreach (var p in players)
                {
                    try
                    {
                        PlayerLogProcedures.AddPlayerLog(p.Id, msg, true);
                    }
                    catch (Exception e)
                    {
                        errors += "<p>" + p.GetFullName() + "encountered:  " + e.ToString() + "</p><br>";
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
                var output = new PublicBroadcastViewModel();
                IPvPWorldStatRepository repo = new EFPvPWorldStatRepository();

                var stats = repo.PvPWorldStats.FirstOrDefault(p => p.Id != -1);

                if (stats != null)
                {
                    output.Message = stats.GameNewsDate;
                }

                return View(MVC.PvPAdmin.Views.ChangeGameDate, output);
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

                var stats = repo.PvPWorldStats.FirstOrDefault(p => p.Id != -1);
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

            var forms = formRepo.DbStaticForms.Where(f => f.MobilityType == PvPStatics.MobilityInanimate || f.MobilityType == PvPStatics.MobilityPet).ToList();

            foreach (var form in forms)
            {

                var item = itemRepo.DbStaticItems.FirstOrDefault(i => i.Id == form.BecomesItemSourceId);

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

            var locations = LocationsStatics.LocationList.GetLocation.Where(l => !l.Region.Equals("dungeon")).ToList();


            // conceal some data about dungeon location in case whoever pulls this JSON is trying to make a map
            foreach (var l in locations)
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
            return View(MVC.PvPAdmin.Views.ApproveContributionList, output);

        }

        public virtual ActionResult ApproveContribution(int id)
        {
            // assert only admins or spell approvers can view this
            if (!User.IsInRole(PvPStatics.Permissions_Admin) && !User.IsInRole(PvPStatics.Permissions_SpellApprover))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            IContributionRepository contributionRepo = new EFContributionRepository();

            var OldCopy = contributionRepo.Contributions.FirstOrDefault(c => c.Id == id);

            var ProofreadCopy = contributionRepo.Contributions.FirstOrDefault(c => c.ProofreadingCopy && c.Skill_FriendlyName == OldCopy.Skill_FriendlyName);


            var owner = PlayerProcedures.GetPlayerFromMembership(OldCopy.OwnerMembershipId);

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

            ProofreadCopy.CursedTF_FormSourceId = OldCopy.CursedTF_FormSourceId;
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

            var contribution = contRepo.Contributions.FirstOrDefault(i => i.Id == id);
            contribution.IsReadyForReview = false;
            contRepo.SaveContribution(contribution);

            var owner = PlayerProcedures.GetPlayerFromMembership(contribution.OwnerMembershipId);

            if (owner != null)
            {
                PlayerLogProcedures.AddPlayerLog(owner.Id, "<b>A contribution you have submitted has been rejected.  You should have received or will soon a message explaining why or what else needs to be done before this contribution can be accepted.  If you do not, please message Judoo on the forums.</b>", true);
            }


            return RedirectToAction(MVC.PvPAdmin.ApproveContributionList());
        }

        public virtual ActionResult ServerBalance_Forms()
        {
            var output = new List<BalancePageViewModel>();

            foreach (var form in FormStatics.GetAllAnimateForms())
            {
                var bbox = new BalanceBox();
                bbox.LoadBalanceBox(form);
                var balance = bbox.GetBalance();
                var absolute = bbox.GetPointTotal();
                var addme = new BalancePageViewModel
                {
                    Id = form.Id,
                    FriendlyName = form.FriendlyName,
                    Balance = balance,
                    AbsolutePoints = absolute,
                    IsUnique = form.IsUnique
                };
                output.Add(addme);
            }

            ViewBag.Text = "Forms";
            return View(MVC.PvPAdmin.Views.ServerBalance, output.OrderByDescending(s => s.Balance));

        }

        public virtual ActionResult ServerBalance_Items()
        {
            var output = new List<BalancePageViewModel>();

            foreach (var item in ItemStatics.GetAllNonPetItems())
            {
                var bbox = new BalanceBox();
                bbox.LoadBalanceBox(item);
                var balance = bbox.GetBalance();
                var absolute = bbox.GetPointTotal();
                var addme = new BalancePageViewModel
                {
                    Id = item.Id,
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
            var output = new List<BalancePageViewModel>();

            foreach (var item in ItemStatics.GetAllPetItems())
            {
                var bbox = new BalanceBox();
                bbox.LoadBalanceBox(item);
                var balance = bbox.GetBalance();
                var absolute = bbox.GetPointTotal();
                var addme = new BalancePageViewModel
                {
                    Id = item.Id,
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
            var output = new List<BalancePageViewModel>();

            IEffectContributionRepository effectContributionRepo = new EFEffectContributionRepository();
            var effectsToAnalyze = effectContributionRepo.EffectContributions.Where(e => e.IsLive && e.ProofreadingCopy).ToList();

            foreach (var effect in effectsToAnalyze)
            {
                var bbox = new BalanceBox();
                bbox.LoadBalanceBox(effect);
                var balance = bbox.GetBalance__NoModifiersOrCaps();
                var absolute = bbox.GetPointTotal();
                var addme = new BalancePageViewModel
                {
                    Id = effect.Id,
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
            var log = serverLogRepo.ServerLogs.FirstOrDefault(t => t.TurnNumber == turn);

            if (log == null)
            {
                TempData["Error"] = "Turn log does not exist.";
                return RedirectToAction(MVC.PvPAdmin.ViewUpdateLogs());
            }

            return View(MVC.PvPAdmin.Views.ViewServerLog, log);
        }

        public virtual ActionResult ViewUpdateLogs()
        {
            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            IServerLogRepository serverLogRepo = new EFServerLogRepository();
            return View(MVC.PvPAdmin.Views.ViewUpdateLogs, serverLogRepo.ServerLogs);
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

            return View(MVC.PvPAdmin.Views.FaeList, encounters);

        }

        public virtual ActionResult WriteFae(int id)
        {
            // assert only admins can view this
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            IJewdewfaeEncounterRepository repo = new EFJewdewfaeEncounterRepository();
            var encounter = repo.JewdewfaeEncounters.FirstOrDefault(e => e.Id == id);

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

            var loc = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == encounter.dbLocationName);

            if (loc == null)
            {
                ViewBag.LocationExists = "<span class='bad'>LOCATION " + encounter.dbLocationName + " DOES NOT EXIST.</span>";
            }
            else
            {
                ViewBag.LocationExists = "<span class='good'>LOCATION " + encounter.dbLocationName + " EXISTs.</span>";
            }

            return View(MVC.PvPAdmin.Views.WriteFae, encounter);
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

            input.dbLocationName = input.dbLocationName.Trim();

            IJewdewfaeEncounterRepository repo = new EFJewdewfaeEncounterRepository();

            var encounter = repo.JewdewfaeEncounters.FirstOrDefault(f => f.Id == input.Id);

            if (encounter == null)
            {
                encounter = new JewdewfaeEncounter();
            }

            //  encounter.Id = input.Id;
            encounter.dbLocationName = input.dbLocationName;
            encounter.RequiredFormSourceId = input.RequiredFormSourceId;
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


            var output = new FairyChallengeBag();

            try
            {
                // load data from the xml
                var filename = AppDomain.CurrentDomain.BaseDirectory + "XMLs/FairyChallengeText/fae_temp.xml";
                var reader = new System.Xml.Serialization.XmlSerializer(typeof(FairyChallengeBag));
                var file = new System.IO.StreamReader(filename);
                output = (FairyChallengeBag)reader.Deserialize(file);
            }
            catch
            {

            }


            return View(MVC.PvPAdmin.Views.WriteFaeEncounter, output);
        }

        public virtual ActionResult LoadSpecificEncounter(string filename)
        {

            var output = new FairyChallengeBag();

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

            var path = Server.MapPath("~/XMLs/FairyChallengeText/");

            // input.title = "Serialization Overview";
            var writer =
                new System.Xml.Serialization.XmlSerializer(typeof(FairyChallengeBag));

            var file = new System.IO.StreamWriter(
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
            var iAmModerator = User.IsInRole(PvPStatics.Permissions_Moderator);
            if (!iAmModerator)
            {
                ViewBag.Message = "You aren't allowed to do this.";
                return View(MVC.PvPAdmin.Views.Index);
            }


            IPlayerRepository playerRepo = new EFPlayerRepository();
            var players = playerRepo.Players.Where(p => p.IpAddress == address).ToList();
            foreach (var p in players)
            {
                p.IpAddress = "reset";
                playerRepo.SavePlayer(p);
                PlayerLogProcedures.AddPlayerLog(p.Id, "<b class='good'>Server notice:  Your IP address has been reset.</b>", true);
            }

            return View(MVC.PvPAdmin.Views.Index);

        }

        [Authorize]
        public virtual ActionResult ToggleBanOnGlobalChat(int id)
        {

            // assert only admin can view this
            var iAmModerator = User.IsInRole(PvPStatics.Permissions_Moderator);
            if (!iAmModerator)
            {
                ViewBag.Message = "You aren't allowed to do this.";
                return View(MVC.PvPAdmin.Views.Index);
            }


            IPlayerRepository playerRepo = new EFPlayerRepository();
            var bannedPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == id);

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
            return View(MVC.PvPAdmin.Views.Scratchpad);
        }

        public virtual ActionResult AuditDonators()
        {

            // assert only admin can view this
            var iAmAdmin = User.IsInRole(PvPStatics.Permissions_Admin);
            if (!iAmAdmin)
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            var output = "";

            IDonatorRepository repo = new EFDonatorRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();

            foreach (var d in repo.Donators.ToList())
            {
                var player = playerRepo.Players.FirstOrDefault(p => p.MembershipId == d.OwnerMembershipId);

                if (player != null && player.DonatorLevel > 0)
                {
                    output += "Looking at " + player.GetFullName() + ".";

                    if (player.DonatorLevel > d.Tier)
                    {
                        player.DonatorLevel = d.Tier;
                        output += "  Knocking down to tier " + d.Tier + ".  </br>";
                        var message = "<span class='bad'>MESSAGE FROM SERVER:  Your Patreon donation tier has been changed to " + d.Tier + ".  If you feel this is in error, please send a private message to Judoo on the forums or through Patreon.  Thank you for your past support!</span>";
                        PlayerLogProcedures.AddPlayerLog(player.Id, message, true);
                    }
                    else
                    {
                        output += "  Okay at tier " + d.Tier + ".  </br>";
                        var message = "<span class='good'>MESSAGE FROM SERVER:  Your Patreon donation has been processed and remains at Tier " + d.Tier + ".  Thank you for your support!</span>";
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
                var stats = repo.PvPWorldStats.FirstOrDefault(p => p.Id != -1);
                stats.LastUpdateTimestamp = stats.LastUpdateTimestamp.AddDays(1);
                repo.SavePvPWorldStat(stats);

                IPlayerLogRepository logRepo = new EFPlayerLogRepository();
                var me = PlayerProcedures.GetPlayerFromMembership("69");
                var newlog = new PlayerLog
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
                var stats = repo.PvPWorldStats.FirstOrDefault(p => p.Id != -1);
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
                return View(MVC.PvPAdmin.Views.FindMissingThumbnails);
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
                var playeritems = DomainRegistry.Repository.Find(new GetItemsOwnedByPlayer {OwnerId = id}).OrderByDescending(i => i.Level);
                ViewBag.player = PlayerProcedures.GetPlayerFormViewModel(id);
                return View(MVC.PvPAdmin.Views.ViewPlayerItems, playeritems);
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
                return View(MVC.PvPAdmin.Views.ViewItemTransferLog);
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

                var person = PlayerProcedures.GetPlayer(id);
                if (person.BotId == AIStatics.ActivePlayerBotId && !DomainRegistry.Repository.FindSingle(new IsChaosChangesEnabled { UserId = person.MembershipId}))
                {
                    TempData["Error"] = "This player does not have chaos mode changes enabled.";
                    return RedirectToAction(MVC.PvP.Play());
                }

                var output = new PlayerNameViewModel();

                if (id != -1)
                {
                    var pm = PlayerProcedures.GetPlayerFormViewModel(id);
                    output.Id = id;
                    output.NewFirstName = pm.Player.FirstName;
                    output.NewLastName = pm.Player.LastName;
                    output.NewFormSourceId = person.FormSourceId;
                    output.Level = pm.Player.Level;
                    output.Money = pm.Player.Money;
                }

                return View(MVC.PvPAdmin.Views.RenamePlayer, output);
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
                var myMembershipId = User.Identity.GetUserId();
                var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
                var world = DomainRegistry.Repository.FindSingle(new GetWorld());
                if (!PvPStatics.ChaosMode && !world.TestServer)
                {
                    TempData["Error"] = "The rename tool only works in chaos mode.";
                    return RedirectToAction(MVC.PvP.Play());
                }

                IPlayerRepository playerRepo = new EFPlayerRepository();
                var player = playerRepo.Players.FirstOrDefault(p => p.Id == input.Id);

                if (player.BotId == AIStatics.ActivePlayerBotId && !DomainRegistry.Repository.FindSingle(new IsChaosChangesEnabled { UserId = player.MembershipId }))
                {
                    TempData["Error"] = "This player does not have chaos mode changes enabled.";
                    return RedirectToAction(MVC.PvP.Play());
                }

                string changed_name = null;
                string changed_level = null;
                string changed_money = null;
                string changed_form = null;

                if (!string.IsNullOrEmpty(input.NewFirstName) && input.NewFirstName != player.FirstName)
                {
                    changed_name = " name,";
                    player.FirstName = input.NewFirstName;
                }

                if (!string.IsNullOrEmpty(input.NewLastName) && input.NewLastName != player.LastName)
                {
                    if (changed_name == null)
                    {
                        changed_name = " name,";
                    }
                    player.LastName = input.NewLastName;
                }

                if (input.Level > 0 && input.Level != player.Level)
                {
                    changed_level = " level,";
                    player.Level = input.Level;
                }

                if (input.Money > 0 && input.Money != player.Money)
                {
                    changed_money = " money,";
                    player.Money = input.Money;
                }

                if (input.NewFormSourceId > 0 && input.NewFormSourceId != player.FormSourceId)
                {
                    changed_form = " form,";
                    IDbStaticFormRepository staticFormRepo = new EFDbStaticFormRepository();
                    var form = staticFormRepo.DbStaticForms.FirstOrDefault(f => f.Id == input.NewFormSourceId);

                    if (form != null && form.MobilityType == PvPStatics.MobilityFull)
                    {
                        DomainRegistry.Repository.Execute(new ChangeForm
                        {
                            PlayerId = player.Id,
                            FormSourceId = form.Id
                        });

                        player.Health = 99999;
                        player.MaxHealth = 99999;
                    }

                    if (form.MobilityType == PvPStatics.MobilityFull && player.Mobility != PvPStatics.MobilityFull)
                    {
                        var item = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer {PlayerId = player.Id});
                        player.Mobility = PvPStatics.MobilityFull;
                        DomainRegistry.Repository.Execute(new UnbembedRunesOnItem { ItemId = item.Id });
                        DomainRegistry.Repository.Execute(new DeleteItem {ItemId = item.Id});
                    }
                }

                playerRepo.SavePlayer(player);

                // if Donna, give player her spells
                if (input.NewFormSourceId == BossProcedures_Donna.DonnaFormSourceId)
                {
                    SkillProcedures.GiveSkillToPlayer(player.Id, BossProcedures_Donna.Spell1);
                    SkillProcedures.GiveSkillToPlayer(player.Id, BossProcedures_Donna.Spell2);
                    SkillProcedures.GiveSkillToPlayer(player.Id, BossProcedures_Donna.Spell3);
                    SkillProcedures.GiveSkillToPlayer(player.Id, BossProcedures_Donna.Spell4);
                    SkillProcedures.GiveSkillToPlayer(player.Id, BossProcedures_Donna.Spell5);
                }

                // if Valentine, give player his spells
                else if (input.NewFormSourceId == BossProcedures_Valentine.ValentineFormSourceId)
                {
                    SkillProcedures.GiveSkillToPlayer(player.Id, BossProcedures_Valentine.BloodyCurseSpellSourceId);
                    SkillProcedures.GiveSkillToPlayer(player.Id, BossProcedures_Valentine.ValentinesPresenceSpellSourceId);
                    SkillProcedures.GiveSkillToPlayer(player.Id, BossProcedures_Valentine.SwordSpellSourceId);
                    SkillProcedures.GiveSkillToPlayer(player.Id, BossProcedures_Valentine.DayVampireFemaleSpellSourceId);
                    SkillProcedures.GiveSkillToPlayer(player.Id, BossProcedures_Valentine.NightVampireFemaleSpellSourceId);
                    SkillProcedures.GiveSkillToPlayer(player.Id, BossProcedures_Valentine.DayVampireMaleSpellSourceId);
                    SkillProcedures.GiveSkillToPlayer(player.Id, BossProcedures_Valentine.NightVampireMaleSpellSourceId);
                }

                // if plague mother, give player her spells
                else if (input.NewFormSourceId == BossProcedures_BimboBoss.BimboBossFormSourceId)
                {
                    SkillProcedures.GiveSkillToPlayer(player.Id, BossProcedures_BimboBoss.RegularTFSpellSourceId);
                }

                // if master rat thief, give player her spells
                else if (input.NewFormSourceId == BossProcedures_Thieves.FemaleBossFormSourceId)
                {
                    SkillProcedures.GiveSkillToPlayer(player.Id, BossProcedures_Thieves.GoldenTrophySpellSourceId);
                }

                // if it's mouse sisters, then give them spells as well
                else if (input.NewFormSourceId == BossProcedures_Sisters.BimboBossFormSourceId)
                {
                    SkillProcedures.GiveSkillToPlayer(player.Id, BossProcedures_Sisters.MakeupKitSpellSourceId);
                }

                else if (input.NewFormSourceId == BossProcedures_Sisters.NerdBossFormSourceId)
                {
                    SkillProcedures.GiveSkillToPlayer(player.Id, BossProcedures_Sisters.MicroscopeSpellSourceId);
                }

                var cm = changed_name + changed_form + changed_level + changed_money;
                cm = cm.TrimEnd(cm[cm.Length - 1]);

                // if chaoslord changes themself, they won't get a notification that they changed themself.
                if (player.Id != me.Id)
                {
                    PlayerLogProcedures.AddPlayerLog(player.Id, $"Player <b>\"{me.GetFullName()}\"</b> has changed your{cm}.", false);
                }
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
            var myMembershipId = User.Identity.GetUserId();
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

            var me = playerRepo.Players.FirstOrDefault(p => p.MembershipId == myMembershipId);
            DomainRegistry.Repository.Execute(new ChangeForm
            {
                PlayerId = me.Id,
                FormSourceId = 137 // Flirty 3-tiered skirt
            });

            // delete old item you are if you are one
            var possibleMeItem = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer {PlayerId = me.Id});
            if (possibleMeItem != null)
            {
                DomainRegistry.Repository.Execute(new UnbembedRunesOnItem { ItemId = possibleMeItem.Id });
                DomainRegistry.Repository.Execute(new DeleteItem {ItemId = possibleMeItem.Id});
            }

            var cmd = new CreateItem
            {
                dbLocationName = me.dbLocationName,
                FormerPlayerId = me.Id,
                OwnerId = null,
                IsEquipped = false,
                Level = me.Level,
                ItemSourceId = ItemStatics.GetStaticItem(88).Id // flirty 3-tiered skirt
            };

            DomainRegistry.Repository.Execute(cmd);

            TempData["Result"] = "You are inanimate.";
            return RedirectToAction(MVC.PvP.Play());

        }

        [Authorize]
        public virtual ActionResult FastPetMe()
        {
            var myMembershipId = User.Identity.GetUserId();
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

            var me = playerRepo.Players.FirstOrDefault(p => p.MembershipId == myMembershipId);
            DomainRegistry.Repository.Execute(new ChangeForm
            {
                PlayerId = me.Id,
                FormSourceId = 152 // Cuddly pocket goo girl
            });

            // delete old item you are if you are one
            var possibleMeItem = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer {PlayerId = me.Id});
            if (possibleMeItem != null)
            {
                DomainRegistry.Repository.Execute(new UnbembedRunesOnItem { ItemId = possibleMeItem.Id });
                DomainRegistry.Repository.Execute(new DeleteItem {ItemId = possibleMeItem.Id});
            }

            var cmd = new CreateItem
            {
                dbLocationName = me.dbLocationName,
                FormerPlayerId = me.Id,
                OwnerId = null,
                IsEquipped = false,
                Level = me.Level,
                ItemSourceId = ItemStatics.GetStaticItem(96).Id // cuddly pocket goo
            };
            DomainRegistry.Repository.Execute(cmd);

            TempData["Result"] = "You are now a pet.";
            return RedirectToAction(MVC.PvP.Play());

        }

        [Authorize]
        public virtual ActionResult FastAnimateMe()
        {
            var myMembershipId = User.Identity.GetUserId();
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

            var me = playerRepo.Players.FirstOrDefault(p => p.MembershipId == myMembershipId);
            DomainRegistry.Repository.Execute(new ChangeForm
            {
                PlayerId = me.Id,
                FormSourceId = me.OriginalFormSourceId
            });

            // delete old item you are if you are one
            var item = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer { PlayerId = me.Id });
            if (item != null)
            {
                DomainRegistry.Repository.Execute(new UnbembedRunesOnItem { ItemId = item.Id });
                DomainRegistry.Repository.Execute(new DeleteItem { ItemId = item.Id });
            }

            TempData["Result"] = "You are now fully animate.";
            return RedirectToAction(MVC.PvP.Play());

        }

        [Authorize]
        public virtual ActionResult FastGiveTPScroll()
        {
            var myMembershipId = User.Identity.GetUserId();
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            IPvPWorldStatRepository repo = new EFPvPWorldStatRepository();
            var stat = repo.PvPWorldStats.First();

            var test = stat.TestServer;

            if (!PvPStatics.ChaosMode && !test)
            {
                TempData["Error"] = "Cannot be done on live server outside of chaos..";
                return RedirectToAction(MVC.PvP.Play());
            }

            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            var cmd = new CreateItem
            {
                OwnerId = me.Id,
                dbLocationName = "",
                EquippedThisTurn = false,
                LastSouledTimestamp = DateTime.UtcNow,
                Level = 0,
                ItemSourceId = ItemStatics.GetStaticItem(ItemStatics.TeleportationScrollItemSourceId).Id
            };

            DomainRegistry.Repository.Execute(cmd);

            TempData["Result"] = "You used your admin magic to give yourself a teleportation scroll.";
            return RedirectToAction(MVC.PvP.Play());

        }

        [Authorize]
        public virtual ActionResult AssignLeadersBadges()
        {

            var myMembershipId = User.Identity.GetUserId();
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

            var output = StatsProcedures.AssignLeadersBadges();

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

            return View(MVC.PvPAdmin.Views.ListCustomForms, output);
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

            return View(MVC.PvPAdmin.Views.EditCustomForm, output);
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

            var form = formRepo.DbStaticForms.FirstOrDefault(f => f.Id == input.CustomForm.Id);

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

        
        public virtual JsonResult GetMembershipIdFromUsername(string name)
        {
            // assert only admins can view this
            if (!User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return Json("Admin permissions required", JsonRequestBehavior.AllowGet);
            }

            var id = DomainRegistry.Repository.FindSingle(new GetMembershipIdFromUsername { Username = name.Trim() });
            return Json(id, JsonRequestBehavior.AllowGet);

        }

        [Authorize]
        public virtual ActionResult SetEveryoneToSP()
        {
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

            using (var context = new StatsContext())
            {
                context.Database.ExecuteSqlCommand($"UPDATE [dbo].[Players] SET GameMode = {(int)GameModeStatics.GameModes.Superprotection} WHERE BotId = 0");
            }

            TempData["Result"] = "All human players have been set to SuperProtection game mode.";
            return RedirectToAction(MVC.PvP.Play());
        }
    }
}