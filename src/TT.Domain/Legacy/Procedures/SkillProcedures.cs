using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Procedures.BossProcedures;
using TT.Domain.Skills.Commands;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Domain.Procedures
{
    public static class SkillProcedures
    {
        public static IEnumerable<Skill> GetSkillsOwnedByPlayer(int playerId)
        {
            ISkillRepository skillRepo = new EFSkillRepository();
            return skillRepo.Skills.Where(s => s.OwnerId == playerId).ToList();
        }

        public static IEnumerable<StaticSkill> GetStaticSkillsOwnedByPlayer(int playerId)
        {
            return GetSkillViewModelsOwnedByPlayer(playerId).Select(s => s.StaticSkill).ToList();
        }

        public static IEnumerable<SkillViewModel> GetSkillViewModelsOwnedByPlayer(int playerId)
        {
            ISkillRepository skillRepo = new EFSkillRepository();

            IEnumerable<SkillViewModel> output = from skill in skillRepo.Skills
                                                  where skill.OwnerId == playerId
                                                  join skillSource in skillRepo.DbStaticSkills on skill.SkillSourceId equals skillSource.Id


                                                  select new SkillViewModel
                                                 {

                                                     MobilityType = skillSource.MobilityType,
                                                     dbSkill = new Skill_VM
                                                     {
                                                         Id = skill.Id,
                                                         OwnerId = skill.OwnerId,
                                                         SkillSourceId = skill.SkillSourceId,
                                                         Charge = skill.Charge,
                                                         Duration = skill.Duration,
                                                         TurnStamp = skill.TurnStamp,
                                                         IsArchived = skill.IsArchived,

                                                     },
                                                     StaticSkill = new StaticSkill
                                                     {
                                                         Id = skillSource.Id,
                                                         FriendlyName = skillSource.FriendlyName,
                                                         FormSourceId = skillSource.FormSourceId,
                                                         Description = skillSource.Description,
                                                         DiscoveryMessage = skillSource.DiscoveryMessage,
                                                         ManaCost = skillSource.ManaCost,
                                                         HealthDamageAmount = skillSource.HealthDamageAmount,
                                                         LearnedAtLocation = skillSource.LearnedAtLocation,
                                                         LearnedAtRegion = skillSource.LearnedAtLocation,
                                                         TFPointsAmount = skillSource.TFPointsAmount,
                                                         ExclusiveToFormSourceId = skillSource.ExclusiveToFormSourceId,
                                                         ExclusiveToItemSourceId = skillSource.ExclusiveToItemSourceId,
                                                         GivesEffectSourceId = skillSource.GivesEffectSourceId,
                                                         IsPlayerLearnable = skillSource.IsPlayerLearnable
                                                     }

                                                 };


            return output;
        }

        public static IEnumerable<SkillViewModel> GetSkillViewModelsOwnedByPlayer__CursesOnly(int playerId)
        {
            ISkillRepository skillRepo = new EFSkillRepository();

            IEnumerable<SkillViewModel> output = from skill in skillRepo.Skills
                                                  
                                                  join skillSource in skillRepo.DbStaticSkills on skill.SkillSourceId equals skillSource.Id
                                                  where skill.OwnerId == playerId && skillSource.MobilityType == "curse"


                                                  select new SkillViewModel
                                                  {

                                                      MobilityType = skillSource.MobilityType,
                                                      dbSkill = new Skill_VM
                                                      {
                                                          Id = skill.Id,
                                                          OwnerId = skill.OwnerId,
                                                          SkillSourceId = skill.SkillSourceId,
                                                          Charge = skill.Charge,
                                                          Duration = skill.Duration,
                                                          TurnStamp = skill.TurnStamp,
                                                          IsArchived = skill.IsArchived,

                                                      },
                                                      StaticSkill = new StaticSkill
                                                      {
                                                          Id = skillSource.Id,
                                                          FriendlyName = skillSource.FriendlyName,
                                                          FormSourceId = skillSource.FormSourceId,
                                                          Description = skillSource.Description,
                                                          DiscoveryMessage = skillSource.DiscoveryMessage,
                                                          ManaCost = skillSource.ManaCost,
                                                          HealthDamageAmount = skillSource.HealthDamageAmount,
                                                          LearnedAtLocation = skillSource.LearnedAtLocation,
                                                          LearnedAtRegion = skillSource.LearnedAtLocation,
                                                          TFPointsAmount = skillSource.TFPointsAmount,
                                                          ExclusiveToFormSourceId = skillSource.ExclusiveToFormSourceId,
                                                          ExclusiveToItemSourceId = skillSource.ExclusiveToItemSourceId,
                                                          GivesEffectSourceId = skillSource.GivesEffectSourceId,
                                                          IsPlayerLearnable = skillSource.IsPlayerLearnable,
                                                      }

                                                  };


            return output;
        }

        public static SkillViewModel GetSkillViewModel(int skillSourceId, int playerId)
        {
            ISkillRepository skillRepo = new EFSkillRepository();

            IEnumerable<SkillViewModel> output = from skill in skillRepo.Skills
                                                  where skill.OwnerId == playerId && skill.SkillSourceId == skillSourceId
                                                 join skillSource in skillRepo.DbStaticSkills on skill.SkillSourceId equals skillSource.Id


                                                  select new SkillViewModel
                                                  {
                                                      MobilityType = skillSource.MobilityType,
                                                      dbSkill = new Skill_VM
                                                      {
                                                          Id = skill.Id,
                                                          OwnerId = skill.OwnerId,
                                                          SkillSourceId = skill.SkillSourceId,
                                                          Charge = skill.Charge,
                                                          Duration = skill.Duration,
                                                          TurnStamp = skill.TurnStamp,
                                                          IsArchived = skill.IsArchived,
                                                      },
                                                      StaticSkill = new StaticSkill
                                                      {
                                                          Id = skillSource.Id,
                                                          FriendlyName = skillSource.FriendlyName,
                                                          FormSourceId = skillSource.FormSourceId,
                                                          Description = skillSource.Description,
                                                          DiscoveryMessage = skillSource.DiscoveryMessage,
                                                          ManaCost = skillSource.ManaCost,
                                                          HealthDamageAmount = skillSource.HealthDamageAmount,
                                                          LearnedAtLocation = skillSource.LearnedAtLocation,
                                                          LearnedAtRegion = skillSource.LearnedAtLocation,
                                                          TFPointsAmount = skillSource.TFPointsAmount,
                                                          ExclusiveToFormSourceId = skillSource.ExclusiveToFormSourceId,
                                                          ExclusiveToItemSourceId = skillSource.ExclusiveToItemSourceId,
                                                          GivesEffectSourceId = skillSource.GivesEffectSourceId,
                                                          IsPlayerLearnable = skillSource.IsPlayerLearnable,
                                                      }

                                                  };

            return output.FirstOrDefault();
        }

        public static SkillViewModel GetSkillViewModel_NotOwned(int skillSourceId)
        {

            ISkillRepository skillRepo = new EFSkillRepository();

            var tempskill = new Skill_VM
            {
                Id = skillSourceId
            };

            var dbstatic = skillRepo.DbStaticSkills.FirstOrDefault(s => s.Id == skillSourceId);
            var tempstatic = new StaticSkill
            {
                Id = dbstatic.Id,
                FriendlyName = dbstatic.FriendlyName,
                GivesEffectSourceId = dbstatic.GivesEffectSourceId,
                Description = dbstatic.Description,
                FormSourceId = dbstatic.FormSourceId,
                ManaCost = dbstatic.ManaCost,
                HealthDamageAmount = dbstatic.HealthDamageAmount,
                TFPointsAmount = dbstatic.TFPointsAmount,
                DiscoveryMessage = dbstatic.DiscoveryMessage,
                ExclusiveToFormSourceId = dbstatic.ExclusiveToFormSourceId,
                ExclusiveToItemSourceId = dbstatic.ExclusiveToItemSourceId,
                LearnedAtLocation = dbstatic.LearnedAtLocation,
                LearnedAtRegion = dbstatic.LearnedAtRegion,
                IsPlayerLearnable = dbstatic.IsPlayerLearnable
            };

            var output = new SkillViewModel
            {
                dbSkill = tempskill,
                StaticSkill = tempstatic,
                
            };

            return output;
        }

        public static string GiveSkillToPlayer(int playerId, int skillSourceId)
        {
            ISkillRepository skillRepo = new EFSkillRepository();
            IDbStaticSkillRepository skillSourceRepo = new EFDbStaticSkillRepository();
            DbStaticSkill skill = skillSourceRepo.DbStaticSkills.SingleOrDefault(s => s.Id == skillSourceId);

            var ghost = skillRepo.Skills.FirstOrDefault(s => s.OwnerId == playerId && s.SkillSourceId == skill.Id);
            if (ghost == null) //player does not have this skill yet; add it
            {
                DomainRegistry.Repository.Execute(new CreateSkill {ownerId = playerId, skillSourceId = skill.Id});

                var output = "";
                output += skill.DiscoveryMessage;
                return output + "  <b>Congratulations, you have learned a new spell, " + skill.FriendlyName + ".</b>";
            }
            else // player already has this skill, so add its charges/duration
            {
                return "You discovered the spell '" + skill.FriendlyName + "' but unfortunately you already knew it.";
            }
        }

        public static List<string> GiveRandomFindableSkillsToPlayer(Player player, int amount)
        {
            ISkillRepository skillRepo = new EFSkillRepository();
            var output = new List<string>();

            IEnumerable<int> playerSkillSourceIds = skillRepo.Skills.Where(s => s.OwnerId == player.Id).Select(s => s.SkillSourceId.Value).ToList();
            IEnumerable<int> learnableSkills = skillRepo.DbStaticSkills.Where(s => s.IsPlayerLearnable).Select(s => s.Id).ToList();


            var eligibleSkills = from s in learnableSkills
                                                 let sx = playerSkillSourceIds
                                 where !sx.Contains(s)
                                     select s;

            eligibleSkills = eligibleSkills.ToList();

            if (!eligibleSkills.Any())
            {
                return output;
            }

            var rand = new Random(DateTime.UtcNow.Millisecond);

            for (var i = 0; i < amount; i++)
            {
                if (!eligibleSkills.Any())
                {
                    break;
                }
                else
                {
                    double max = eligibleSkills.Count();
                    var randIndex = Convert.ToInt32(Math.Floor(rand.NextDouble() * max));
                    var skillToGive = eligibleSkills.ElementAt(randIndex);
                    var staticSkill = SkillStatics.GetStaticSkill(skillToGive);
                    GiveSkillToPlayer(player.Id, staticSkill.Id);
                    eligibleSkills = eligibleSkills.Where(s => s != skillToGive);
                    output.Add(staticSkill.FriendlyName);
                }
            }

            return output;
        }

        public static void DeleteAllPlayerSkills(int playerId)
        {
            ISkillRepository skillRepo = new EFSkillRepository();
            IEnumerable<Skill> skillsToDelete = skillRepo.Skills.Where(s => s.OwnerId == playerId).ToList();

            foreach (var s in skillsToDelete)
            {
                skillRepo.DeleteSkill(s.Id);
            }
        }

        public static void TransferAllPlayerSkills(int oldPlayerId, int newPlayerId)
        {
            ISkillRepository skillRepo = new EFSkillRepository();
            IEnumerable<Skill> skillsToDelete = skillRepo.Skills.Where(s => s.OwnerId == oldPlayerId && s.SkillSourceId != PvPStatics.Spell_WeakenId).ToList();

            foreach (var s in skillsToDelete)
            {
                s.OwnerId = newPlayerId;
                skillRepo.SaveSkill(s);
            }

            var weakenSkill = skillRepo.Skills.FirstOrDefault(s => s.OwnerId == oldPlayerId && s.Id == PvPStatics.Spell_WeakenId);
            if (weakenSkill != null)
            {
                skillRepo.DeleteSkill(weakenSkill.Id);
            }
        }

        public static void UpdateFormSpecificSkillsToPlayer(Player player, int newFormSourceId)
        {
            // don't care about bots
            if (player.BotId < AIStatics.ActivePlayerBotId)
            {
                return;
            }

            ISkillRepository skillRepo = new EFSkillRepository();

            // delete all of the old form specific skills
            IEnumerable<SkillViewModel> formSpecificSkills = GetSkillViewModelsOwnedByPlayer__CursesOnly(player.Id).ToList();
            IEnumerable<int> formSpecificSkillIds = formSpecificSkills.Where(s => s.MobilityType == "curse" && s.StaticSkill.ExclusiveToFormSourceId != newFormSourceId).Select(s => s.dbSkill.Id).ToList();

            foreach (var id in formSpecificSkillIds)
            {
                skillRepo.DeleteSkill(id);
            }

            // now give the player the new form specific skills
            var formSpecificSkillsToGive = SkillStatics.GetFormSpecificSkills(newFormSourceId).ToList();
            foreach (var skill in formSpecificSkillsToGive)
            {

                // make sure player does not already have this skill due to some bug or othher
                var possibledbSkill = skillRepo.Skills.FirstOrDefault(s => s.OwnerId == player.Id && s.SkillSourceId == skill.Id);

                if (possibledbSkill == null)
                {
                    DomainRegistry.Repository.Execute(new CreateSkill {ownerId = player.Id, skillSourceId = skill.Id});
                }
            }
        }

        public static void UpdateItemSpecificSkillsToPlayer(Player owner)
        {
            UpdateItemSpecificSkillsToPlayer(owner, null);
        }

        /// <summary>
        /// Delete any curses/blessing gained by items/pets from a player that they no longer posses, then give them any curses unique to a new item/pet.  Psychopathic spellslingers make an exception and should never learn any new curses from items/pets they have gained.
        /// </summary>
        /// <param name="owner">The Player who owns/owned the item</param>
        /// <param name="newItemSourceId">The id of a new item that the player is gaining, if any.</param>
        public static void UpdateItemSpecificSkillsToPlayer(Player owner, int? newItemSourceId)
        {

            ISkillRepository skillRepo = new EFSkillRepository();

            // delete all of the old item specific skills for the player
            IEnumerable<SkillViewModel> itemSpecificSkills = GetSkillViewModelsOwnedByPlayer__CursesOnly(owner.Id).ToList();
            List<int> equippedItemSourceIds = ItemProcedures.GetAllPlayerItems_ItemOnly(owner.Id).Where(i => i.IsEquipped && i.ItemSourceId != newItemSourceId).Select(s => s.ItemSourceId).ToList();
            var itemSpecificSkillsIds = new List<int>();

            foreach (var s in itemSpecificSkills)
            {
                if (s.StaticSkill.ExclusiveToItemSourceId != null && !equippedItemSourceIds.Contains(s.StaticSkill.ExclusiveToItemSourceId.Value))
                {
                    itemSpecificSkillsIds.Add(s.dbSkill.Id);
                }
            }

            foreach (var id in itemSpecificSkillsIds)
            {
                skillRepo.DeleteSkill(id);
            }

            // don't give psychos any curses.  Quit automatically
            if (owner.BotId == AIStatics.PsychopathBotId)
            {
                return;
            }

            // now give the player any skills they are missing
            if (newItemSourceId != null)
            {
                var itemSpecificSkillsToGive = SkillStatics.GetItemSpecificSkills(newItemSourceId.Value).ToList();
                foreach (var skill in itemSpecificSkillsToGive)
                {

                    // make sure player does not already have this skill due to some bug or othher
                    var possibledbSkill = skillRepo.Skills.FirstOrDefault(s => s.OwnerId == owner.Id && s.SkillSourceId == skill.Id);

                    if (possibledbSkill == null)
                    {
                        DomainRegistry.Repository.Execute(new CreateSkill { ownerId = owner.Id, skillSourceId = skill.Id });
                    }
                }
            }
            
        }

        public static int GetCountOfLearnableSpells()
        {
            IDbStaticSkillRepository skillRepo = new EFDbStaticSkillRepository();
            var spellCount = skillRepo.DbStaticSkills.Count(s => s.IsPlayerLearnable);
            return spellCount;
        }

        public static void ArchiveSpell(int id)
        {
            ISkillRepository skillRepo = new EFSkillRepository();
            var skill = skillRepo.Skills.FirstOrDefault(s => s.Id == id);
            skill.IsArchived = !skill.IsArchived;
            skillRepo.SaveSkill(skill);
        }

        public static void ArchiveAllSpells(int playerId, bool archiveOrNot)
        {
            string archiveBool;
            if (archiveOrNot)
            {
                archiveBool = "1";
            }
            else
            {
                archiveBool = "0";
            }
            using (var context = new StatsContext())
            {
                context.Database.ExecuteSqlCommand($"UPDATE [dbo].[Skills] SET IsArchived = {archiveBool} WHERE OwnerId = {playerId} AND SkillSourceId != {PvPStatics.Spell_WeakenId}");
            }
        }

        public static IEnumerable<SkillViewModel> AvailableSkills(Player attacker, Player target, bool includeArchived)
        {
            IEnumerable<SkillViewModel> output = SkillProcedures.GetSkillViewModelsOwnedByPlayer(attacker.Id);
            
            if (!includeArchived)
            {
                output = output.Where(s => !s.dbSkill.IsArchived);
            }

            // filter out spells that you can't use on your target
            if (FriendProcedures.PlayerIsMyFriend(attacker, target) || target.BotId < AIStatics.ActivePlayerBotId)
            {
                // do nothing, all spells are okay
            }

            // both players are in protection; only allow animate spells
            else if (attacker.GameMode == (int)GameModeStatics.GameModes.Protection && target.GameMode == (int)GameModeStatics.GameModes.Protection)
            {
                output = output.Where(s => s.MobilityType == PvPStatics.MobilityFull);
            }

            // attack or the target is in superprotection and not a friend or bot; no spells work
            else if (target.GameMode == (int)GameModeStatics.GameModes.Superprotection || (attacker.GameMode == (int)GameModeStatics.GameModes.Superprotection && target.BotId == AIStatics.ActivePlayerBotId))
            {
                output = output.Where(s => s.MobilityType == "NONEXISTANT");
            }

            // filter out MC spells for bots
            if (target.BotId < AIStatics.ActivePlayerBotId)
            {
                output = output.Where(s => s.MobilityType != PvPStatics.MobilityMindControl);
            }

            // only show inanimates for rat thieves
            if (target.BotId == AIStatics.MaleRatBotId || target.BotId == AIStatics.FemaleRatBotId)
            {
                output = output.Where(s => s.MobilityType == PvPStatics.MobilityInanimate);
            }

            // only show Weaken for valentine
            if (target.BotId == AIStatics.ValentineBotId)
            {
                output = output.Where(s => s.dbSkill.SkillSourceId == PvPStatics.Spell_WeakenId);
            }

            // only bimbo spell works on nerd mouse boss
            if (target.BotId == AIStatics.MouseNerdBotId)
            {
                output = output.Where(s => s.StaticSkill.Id == BossProcedures_Sisters.BimboSpellSourceId);
            }

            // only nerd spell works on nerd bimbo boss
            if (target.BotId == AIStatics.MouseBimboBotId)
            {
                output = output.Where(s => s.StaticSkill.Id == BossProcedures_Sisters.NerdSpellSourceId);
            }

            // Vanquish and weaken only works against dungeon demons
            if (target.BotId == AIStatics.DemonBotId)
            {
                output = output.Where(s => s.StaticSkill.Id == PvPStatics.Dungeon_VanquishSpellSourceId || s.StaticSkill.Id == PvPStatics.Spell_WeakenId);
            }

            // Filter out Vanquish when attacking non-Dungeon Demon player
            if (target.BotId != AIStatics.DemonBotId)
            {
                output = output.Where(s => s.StaticSkill.Id != PvPStatics.Dungeon_VanquishSpellSourceId);
            }

            // Fae-In-A-Bottle only works against Narcissa
            if (target.BotId == AIStatics.FaebossBotId)
            {
                output = output.Where(s => s.StaticSkill.Id == BossProcedures_FaeBoss.SpellUsedAgainstNarcissaSourceId);
            }

            // Filter out Fae-In-A-Bottle when attacking non-Narcissa player
            if (target.BotId != AIStatics.FaebossBotId)
            {
                output = output.Where(s => s.StaticSkill.Id != BossProcedures_FaeBoss.SpellUsedAgainstNarcissaSourceId);
            }

            // only inanimate and animal spells work on minibosses, donna, and lovebringer
            if (AIStatics.IsAMiniboss(target.BotId) ||
                target.BotId == AIStatics.MotorcycleGangLeaderBotId ||
                target.BotId == AIStatics.BimboBossBotId)
            {
                output = output.Where(s => s.MobilityType == PvPStatics.MobilityInanimate || s.MobilityType == PvPStatics.MobilityPet);
            }

            return output;
        }

    }
}