using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
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
            return GetSkillViewModelsOwnedByPlayer(playerId).Select(s => s.Skill).ToList();
        }

        public static IEnumerable<DbStaticSkill> GetAllLearnableSpells()
        {
            IDbStaticSkillRepository skillRepo = new EFDbStaticSkillRepository();
            return skillRepo.DbStaticSkills.Where(s => s.IsPlayerLearnable == true && s.IsLive == "live" && s.LearnedAtLocation != null && s.LearnedAtLocation != "" || s.LearnedAtRegion != null && s.LearnedAtRegion != "");
        }

        public static IEnumerable<MySkillsViewModel> GetMySkillsViewModel(int playerId)
        {
            ISkillRepository skillRepo = new EFSkillRepository();

            IEnumerable<MySkillsViewModel> output = from ds in skillRepo.Skills
                                                  where ds.OwnerId == playerId
                                                  join ss in skillRepo.DbStaticSkills on ds.Name equals ss.dbName
                                                  join sf in skillRepo.DbStaticForms on ss.FormdbName equals sf.dbName

                                                  select new MySkillsViewModel
                                                  {
                                                      Skill_Charge = ds.Charge,
                                                      Skill_Duration = ds.Duration,
                                                      Skill_IsArchived = ds.IsArchived,
                                                      Skill_Name = ds.Name,
                                                      Skill_Id = ds.Id,

                                                      Skill_MobilityType = ss.MobilityType,
                                                      Skill_FriendlyName = ss.FriendlyName,
                                                      Skill_Description = ss.Description,
                                                      Skill_ManaCost = ss.ManaCost,
                                                      Skill_HealthDamageAmount = ss.HealthDamageAmount,
                                                      Skill_GivesEffect = ss.GivesEffect,
                                                      Skill_TFPointsAmount = ss.TFPointsAmount,
                                                      Form_FriendlyName = sf.FriendlyName
                                                  };
            return output;
        }

        public static IEnumerable<SkillViewModel2> GetSkillViewModelsOwnedByPlayer(int playerId)
        {
            ISkillRepository skillRepo = new EFSkillRepository();

            IEnumerable<SkillViewModel2> output = from ds in skillRepo.Skills
                                                  where ds.OwnerId == playerId
                                                  join ss in skillRepo.DbStaticSkills on ds.Name equals ss.dbName


                                                  select new SkillViewModel2
                                                 {

                                                     MobilityType = ss.MobilityType,
                                                     dbSkill = new Skill_VM
                                                     {
                                                         Id = ds.Id,
                                                         OwnerId = ds.OwnerId,
                                                         Name = ds.Name,
                                                         Charge = ds.Charge,
                                                         Duration = ds.Duration,
                                                         TurnStamp = ds.TurnStamp,
                                                         IsArchived = ds.IsArchived,

                                                     },
                                                     Skill = new StaticSkill
                                                     {
                                                         dbName = ss.dbName,
                                                         FriendlyName = ss.FriendlyName,
                                                         FormdbName = ss.FormdbName,
                                                         Description = ss.Description,
                                                         DiscoveryMessage = ss.DiscoveryMessage,
                                                         ManaCost = ss.ManaCost,
                                                         HealthDamageAmount = ss.HealthDamageAmount,
                                                         LearnedAtLocation = ss.LearnedAtLocation,
                                                         LearnedAtRegion = ss.LearnedAtLocation,
                                                         TFPointsAmount = ss.TFPointsAmount,
                                                         ExclusiveToForm = ss.ExclusiveToForm,
                                                         ExclusiveToItem = ss.ExclusiveToItem,
                                                         GivesEffect = ss.GivesEffect,
                                                         IsPlayerLearnable = ss.IsPlayerLearnable,
                                                     }

                                                 };


            return output;
        }

        public static IEnumerable<SkillViewModel2> GetSkillViewModelsOwnedByPlayer__CursesOnly(int playerId)
        {
            ISkillRepository skillRepo = new EFSkillRepository();

            IEnumerable<SkillViewModel2> output = from ds in skillRepo.Skills
                                                  
                                                  join ss in skillRepo.DbStaticSkills on ds.Name equals ss.dbName
                                                  where ds.OwnerId == playerId && ss.MobilityType == "curse"


                                                  select new SkillViewModel2
                                                  {

                                                      MobilityType = ss.MobilityType,
                                                      dbSkill = new Skill_VM
                                                      {
                                                          Id = ds.Id,
                                                          OwnerId = ds.OwnerId,
                                                          Name = ds.Name,
                                                          Charge = ds.Charge,
                                                          Duration = ds.Duration,
                                                          TurnStamp = ds.TurnStamp,
                                                          IsArchived = ds.IsArchived,

                                                      },
                                                      Skill = new StaticSkill
                                                      {
                                                          dbName = ss.dbName,
                                                          FriendlyName = ss.FriendlyName,
                                                          FormdbName = ss.FormdbName,
                                                          Description = ss.Description,
                                                          DiscoveryMessage = ss.DiscoveryMessage,
                                                          ManaCost = ss.ManaCost,
                                                          HealthDamageAmount = ss.HealthDamageAmount,
                                                          LearnedAtLocation = ss.LearnedAtLocation,
                                                          LearnedAtRegion = ss.LearnedAtLocation,
                                                          TFPointsAmount = ss.TFPointsAmount,
                                                          ExclusiveToForm = ss.ExclusiveToForm,
                                                          ExclusiveToItem = ss.ExclusiveToItem,
                                                          GivesEffect = ss.GivesEffect,
                                                          IsPlayerLearnable = ss.IsPlayerLearnable,
                                                      }

                                                  };


            return output;
        }

        public static SkillViewModel2 GetSkillViewModel(string skilldbName, int playerId)
        {
            ISkillRepository skillRepo = new EFSkillRepository();

            IEnumerable<SkillViewModel2> output = from ds in skillRepo.Skills
                                                  where ds.OwnerId == playerId && ds.Name == skilldbName
                                                  join ss in skillRepo.DbStaticSkills on ds.Name equals ss.dbName


                                                  select new SkillViewModel2
                                                  {
                                                      MobilityType = ss.MobilityType,
                                                      dbSkill = new Skill_VM
                                                      {
                                                          Id = ds.Id,
                                                          OwnerId = ds.OwnerId,
                                                          Name = ds.Name,
                                                          Charge = ds.Charge,
                                                          Duration = ds.Duration,
                                                          TurnStamp = ds.TurnStamp,
                                                          IsArchived = ds.IsArchived,
                                                      },
                                                      Skill = new StaticSkill
                                                      {
                                                          dbName = ss.dbName,
                                                          FriendlyName = ss.FriendlyName,
                                                          FormdbName = ss.FormdbName,
                                                          Description = ss.Description,
                                                          DiscoveryMessage = ss.DiscoveryMessage,
                                                          ManaCost = ss.ManaCost,
                                                          HealthDamageAmount = ss.HealthDamageAmount,
                                                          LearnedAtLocation = ss.LearnedAtLocation,
                                                          LearnedAtRegion = ss.LearnedAtLocation,
                                                          TFPointsAmount = ss.TFPointsAmount,
                                                          ExclusiveToForm = ss.ExclusiveToForm,
                                                          ExclusiveToItem = ss.ExclusiveToItem,
                                                          GivesEffect = ss.GivesEffect,
                                                          IsPlayerLearnable = ss.IsPlayerLearnable,
                                                      }

                                                  };

            return output.FirstOrDefault();
        }

        public static SkillViewModel2 GetSkillViewModel_NotOwned(string skilldbName)
        {

            ISkillRepository skillRepo = new EFSkillRepository();

            Skill_VM tempskill = new Skill_VM
            {
                Name = skilldbName,
            };

            DbStaticSkill dbstatic = skillRepo.DbStaticSkills.FirstOrDefault(s => s.dbName == skilldbName);
            StaticSkill tempstatic = new StaticSkill
            {
                dbName = dbstatic.dbName,
                FriendlyName = dbstatic.FriendlyName,
                GivesEffect = dbstatic.GivesEffect,
                Description = dbstatic.Description,
                FormdbName = dbstatic.FormdbName,
                ManaCost = dbstatic.ManaCost,
                HealthDamageAmount = dbstatic.HealthDamageAmount,
                TFPointsAmount = dbstatic.TFPointsAmount,
                DiscoveryMessage = dbstatic.DiscoveryMessage,
                ExclusiveToForm = dbstatic.ExclusiveToForm,
                ExclusiveToItem = dbstatic.ExclusiveToItem,
                LearnedAtLocation = dbstatic.LearnedAtLocation,
                LearnedAtRegion = dbstatic.LearnedAtRegion,
                IsPlayerLearnable = dbstatic.IsPlayerLearnable
            };

            SkillViewModel2 output = new SkillViewModel2
            {
                dbSkill = tempskill,
                Skill = tempstatic,
                
            };

            return output;
        }

        public static string GiveSkillToPlayer(int playerId, string skilldbName)
        {
            ISkillRepository skillRepo = new EFSkillRepository();
            DbStaticSkill skill = skillRepo.DbStaticSkills.FirstOrDefault(s => s.dbName == skilldbName);
            return GiveSkillToPlayer(playerId, skill);
        }

        public static string GiveSkillToPlayer(int playerId, DbStaticSkill skill)
        {
            ISkillRepository skillRepo = new EFSkillRepository();

            Skill ghost = skillRepo.Skills.FirstOrDefault(s => s.OwnerId == playerId && s.Name == skill.dbName);
            if (ghost == null) //player does not have this skill yet; add it
            {

                Skill newskill = new Skill
                {
                    Name = skill.dbName,
                    OwnerId = playerId,
                    Charge = -1,
                    Duration = -1
                };
                skillRepo.SaveSkill(newskill);

                // get the skill's discovery message if it has one
                DbStaticSkill skillPlus = SkillStatics.GetStaticSkill(newskill.Name);
                string output = "";

                //if (skillPlus.DiscoveryMessage != null && skillPlus.DiscoveryMessage != "")
                //{
                //    output += skillPlus.DiscoveryMessage + "  ";
                //}

                output += skillPlus.DiscoveryMessage;

                return output + "  <b>Congratulations, you have learned a new spell, " + skill.FriendlyName + ".</b>";
            }
            else // player already has this skill, so add its charges/duration
            {
                return "You discovered the spell '" + skill.FriendlyName + "' but unfortunately you already knew it.";
            }
        }

        public static string GiveRandomFindableSkillsToPlayer(Player player, int amount)
        {
            ISkillRepository skillRepo = new EFSkillRepository();
            string output = "";

            IEnumerable<string> playerSkills = skillRepo.Skills.Where(s => s.OwnerId == player.Id).Select(s => s.Name).ToList();
            IEnumerable<string> learnableSkills = skillRepo.DbStaticSkills.Where(s => s.IsPlayerLearnable == true).Select(s => s.dbName).ToList();


            IEnumerable<string> eligibleSkills = from s in learnableSkills
                                                 let sx = playerSkills
                                     where !sx.Contains(s)
                                     select s;

            eligibleSkills = eligibleSkills.ToList();

            if (eligibleSkills.Count() == 0)
            {
                return "(Unfortunately you don't learn any new spells that you don't already know.)";
            }

            Random rand = new Random(DateTime.UtcNow.Millisecond);

            for (int i = 0; i < amount; i++)
            {
                if (eligibleSkills.Count() == 0)
                {
                    break;
                }
                else
                {
                    double max = eligibleSkills.Count();
                    int randIndex = Convert.ToInt32(Math.Floor(rand.NextDouble() * max));
                    string skillToGive = eligibleSkills.ElementAt(randIndex);
                    DbStaticSkill staticSkill = SkillStatics.GetStaticSkill(skillToGive);
                    GiveSkillToPlayer(player.Id, staticSkill);
                    eligibleSkills = eligibleSkills.Where(s => s != skillToGive);
                    output += "<b>" + staticSkill.FriendlyName + "</b>, ";
                }
            }

            output += "";
            return output;
        }

        public static void DeleteAllPlayerSkills(int playerId)
        {
            ISkillRepository skillRepo = new EFSkillRepository();
            IEnumerable<Skill> skillsToDelete = skillRepo.Skills.Where(s => s.OwnerId == playerId).ToList();

            foreach (Skill s in skillsToDelete)
            {
                skillRepo.DeleteSkill(s.Id);
            }
        }

        public static void TransferAllPlayerSkills(int oldPlayerId, int newPlayerId)
        {
            ISkillRepository skillRepo = new EFSkillRepository();
            IEnumerable<Skill> skillsToDelete = skillRepo.Skills.Where(s => s.OwnerId == oldPlayerId && s.Name != "lowerHealth").ToList();

            foreach (Skill s in skillsToDelete)
            {
                s.OwnerId = newPlayerId;
                skillRepo.SaveSkill(s);
            }

            Skill weakenSkill = skillRepo.Skills.FirstOrDefault(s => s.OwnerId == oldPlayerId && s.Name == "lowerHealth");
            if (weakenSkill != null)
            {
                skillRepo.DeleteSkill(weakenSkill.Id);
            }
        }

        public static void UpdateFormSpecificSkillsToPlayer(Player player, string oldFormDbName, string newFormDbName)
        {
            // don't care about bots
            if (player.BotId < AIStatics.ActivePlayerBotId)
            {
                return;
            }

            ISkillRepository skillRepo = new EFSkillRepository();

            // delete all of the old form specific skills
            IEnumerable<SkillViewModel2> formSpecificSkills = GetSkillViewModelsOwnedByPlayer__CursesOnly(player.Id).ToList();
            IEnumerable<int> formSpecificSkillIds = formSpecificSkills.Where(s => s.MobilityType == "curse" && s.Skill.ExclusiveToForm != newFormDbName).Select(s => s.dbSkill.Id).ToList();

            foreach (int id in formSpecificSkillIds)
            {
                skillRepo.DeleteSkill(id);
            }

            // now give the player the new form specific skills
            List<DbStaticSkill> formSpecificSkillsToGive = SkillStatics.GetFormSpecificSkills(newFormDbName).ToList();
            foreach (DbStaticSkill skill in formSpecificSkillsToGive)
            {

                // make sure player does not already have this skill due to some bug or othher
                Skill possibledbSkill = skillRepo.Skills.FirstOrDefault(s => s.OwnerId == player.Id && s.Name == skill.dbName);

                if (possibledbSkill == null)
                {
                    Skill dbSkill = new Skill
                    {
                        OwnerId = player.Id,
                        Charge = -1,
                        Duration = -1,
                        Name = skill.dbName,
                    };
                    skillRepo.SaveSkill(dbSkill);
                }
            }
        }

        public static void UpdateItemSpecificSkillsToPlayer(Player owner)
        {
            UpdateItemSpecificSkillsToPlayer(owner, "X");
        }

        /// <summary>
        /// Delete any curses/blessing gained by items/pets from a player that they no longer posses, then give them any curses unique to a new item/pet.  Psychopathic spellslingers make an exception and should never learn any new curses from items/pets they have gained.
        /// </summary>
        /// <param name="owner">The Player who owns/owned the item</param>
        /// <param name="newItemName">The name of a new item that the player is gaining, if any.</param>
        public static void UpdateItemSpecificSkillsToPlayer(Player owner, string newItemName)
        {

            ISkillRepository skillRepo = new EFSkillRepository();

            // delete all of the old item specific skills for the player
            IEnumerable<SkillViewModel2> itemSpecificSkills = GetSkillViewModelsOwnedByPlayer__CursesOnly(owner.Id).ToList();
            IEnumerable<string> equippedItemsDbNames = ItemProcedures.GetAllPlayerItems_ItemOnly(owner.Id).Where(i => i.IsEquipped == true && i.dbName != newItemName).Select(s => s.dbName).ToList();
            List<int> itemSpecificSkillsIds = new List<int>();

            foreach (SkillViewModel2 s in itemSpecificSkills)
            {
                if (s.Skill.ExclusiveToItem != null && s.Skill.ExclusiveToItem != "" && equippedItemsDbNames.Contains(s.Skill.ExclusiveToItem) == false)
                {
                    itemSpecificSkillsIds.Add(s.dbSkill.Id);
                }
            }

            foreach (int id in itemSpecificSkillsIds)
            {
                skillRepo.DeleteSkill(id);
            }

            // don't give psychos any curses.  Quit automatically
            if (owner.BotId == AIStatics.PsychopathBotId)
            {
                return;
            }

            // now give the player any skills they are missing
            List<DbStaticSkill> itemSpecificSkillsToGive = SkillStatics.GetItemSpecificSkills(newItemName).ToList();
            foreach (DbStaticSkill skill in itemSpecificSkillsToGive)
            {

                // make sure player does not already have this skill due to some bug or othher
                Skill possibledbSkill = skillRepo.Skills.FirstOrDefault(s => s.OwnerId == owner.Id && s.Name == skill.dbName);

                if (possibledbSkill == null)
                {
                    Skill dbSkill = new Skill
                    {
                        OwnerId = owner.Id,
                        Charge = -1,
                        Duration = -1,
                        Name = skill.dbName,
                    };
                    skillRepo.SaveSkill(dbSkill);
                }
            }
        }

        public static void DeleteOldXML(string xmlpath)
        {
            if (Directory.Exists(Path.GetDirectoryName(xmlpath)))
            {
                File.Delete(xmlpath);
            }
        }

        public static int GetCountOfLearnableSpells()
        {
            IDbStaticSkillRepository skillRepo = new EFDbStaticSkillRepository();
            int spellCount = skillRepo.DbStaticSkills.Where(s => s.IsPlayerLearnable == true).Count();
            return spellCount;

        }

        public static void ArchiveSpell(int id)
        {
            ISkillRepository skillRepo = new EFSkillRepository();
            Skill skill = skillRepo.Skills.FirstOrDefault(s => s.Id == id);
            skill.IsArchived = !skill.IsArchived;
            skillRepo.SaveSkill(skill);
        }

        public static void ArchiveAllSpells(int playerId, bool archiveOrNot)
        {
            string archiveBool;
            if (archiveOrNot == true)
            {
                archiveBool = "1";
            }
            else
            {
                archiveBool = "0";
            }
            using (var context = new StatsContext())
            {
                try
                {
                    context.Database.ExecuteSqlCommand("UPDATE [Stats].[dbo].[Skills] SET IsArchived = " + archiveBool + " WHERE OwnerId = " + playerId + " AND Name != 'lowerHealth'");
                }
                catch (Exception)
                {

                }
            }
        }


    }
}