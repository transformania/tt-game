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
            //ISkillRepository skillRepo = new EFSkillRepository();
            //IEnumerable<Skill> mySkills = skillRepo.Skills.Where(s => s.OwnerId == playerId);

            //List<DbStaticSkill> myStaticSkills = new List<DbStaticSkill>();

            //foreach (Skill skill in mySkills)
            //{
            //    myStaticSkills.Add(SkillStatics.GetStaticSkill(skill.Name));
            //}



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


            //IEnumerable<SkillViewModel> output = null;

            //    using (var context = new StatsContext())
            //    {
            //       // ObjectParameter name = new ObjectParameter("OwnerId", typeof(int));
            //      //  var query = context.Database.SqlQuery("exec GetPlayerSkillViewModels", playerId);



            //    }


            return output;
        }


        //public static SkillViewModel GetSkillViewModel(string skilldbName, int playerId)
        //{
        //    ISkillRepository skillRepo = new EFSkillRepository();
        //    SkillViewModel output = new SkillViewModel
        //    {
        //        dbSkill = skillRepo.Skills.FirstOrDefault(s => s.Name == skilldbName && s.OwnerId == playerId),
        //        Skill = SkillStatics.GetStaticSkill.FirstOrDefault(s => s.dbName == skilldbName)
        //    };

        //    try
        //    {
        //        output.MobilityType = FormStatics.GetForm.FirstOrDefault(f => f.dbName == output.Skill.FormdbName).MobilityType;
        //    }
        //    catch
        //    {
        //        // if there is an effect for this skill, it's a curse or blessing type
        //        StaticEffect possibleEffect = EffectStatics.GetStaticEffect.FirstOrDefault(e => e.dbName == output.Skill.GivesEffect);

        //        if (possibleEffect != null)
        //        {
        //            output.MobilityType = "curse";
        //        }
        //        else
        //        {
        //            output.MobilityType = "weaken";
        //        }
        //    }

        //    return output;
        //}

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

        public static void GiveFormSpecificSkillsToPlayer(Player player, string formdbName)
        {
            ISkillRepository skillRepo = new EFSkillRepository();

            List<DbStaticSkill> formSpecificSkills = SkillStatics.GetFormSpecificSkills(formdbName).ToList();

            foreach (DbStaticSkill skill in formSpecificSkills)
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

        public static void RemoveFormSpecificSkillsToPlayer(Player player, string formdbName)
        {
            ISkillRepository skillRepo = new EFSkillRepository();

            List<DbStaticSkill> formSpecificSkills = SkillStatics.GetFormSpecificSkills(formdbName).ToList();


            foreach (DbStaticSkill skill in formSpecificSkills)
            {
                Skill dbSkill = skillRepo.Skills.FirstOrDefault(s => s.OwnerId == player.Id && s.Name == skill.dbName);

                if (dbSkill != null)
                {
                    skillRepo.DeleteSkill(dbSkill.Id);
                }

            }

            // what happens if a player is double-transformed?  Do they get double skills?

        }

        public static void DeleteOldXML(string xmlpath)
        {
            if (Directory.Exists(Path.GetDirectoryName(xmlpath)))
            {
                File.Delete(xmlpath);
            }
        }


    }
}