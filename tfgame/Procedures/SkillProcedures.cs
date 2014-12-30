using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.Statics;
using tfgame.ViewModels;

namespace tfgame.Procedures
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
            };

            SkillViewModel2 output = new SkillViewModel2
            {
                dbSkill = tempskill,
                Skill = tempstatic,
            };

            return output;
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
            if (player.MembershipId < 0)
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

        public static void UpdateItemSpecificSkillsToPlayer(int ownerId)
        {
            UpdateItemSpecificSkillsToPlayer(ownerId, "X");
        }

        public static void UpdateItemSpecificSkillsToPlayer(int ownerId, string newItemName)
        {

            ISkillRepository skillRepo = new EFSkillRepository();

            // delete all of the old item specific skills for the player
            IEnumerable<SkillViewModel2> itemSpecificSkills = GetSkillViewModelsOwnedByPlayer__CursesOnly(ownerId).ToList();
            IEnumerable<string> equippedItemsDbNames = ItemProcedures.GetAllPlayerItems_ItemOnly(ownerId).Where(i => i.IsEquipped == true && i.dbName != newItemName).Select(s => s.dbName).ToList();
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



            // now give the player any skills they are missing
            List<DbStaticSkill> itemSpecificSkillsToGive = SkillStatics.GetItemSpecificSkills(newItemName).ToList();
            foreach (DbStaticSkill skill in itemSpecificSkillsToGive)
            {

                // make sure player does not already have this skill due to some bug or othher
                Skill possibledbSkill = skillRepo.Skills.FirstOrDefault(s => s.OwnerId == ownerId && s.Name == skill.dbName);

                if (possibledbSkill == null)
                {
                    Skill dbSkill = new Skill
                    {
                        OwnerId = ownerId,
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
            int spellCount = skillRepo.DbStaticSkills.Where(s => ((s.LearnedAtLocation != "" && s.LearnedAtLocation != null) || (s.LearnedAtRegion != "" && s.LearnedAtRegion != null)) && (s.MobilityType == "full" || s.MobilityType == "inanimate" || s.MobilityType == "animal")).Count();
            return spellCount;

        }


    }
}