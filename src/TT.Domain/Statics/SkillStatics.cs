using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;

namespace TT.Domain.Statics
{
    public static class SkillStatics
    {

        public static DbStaticSkill GetStaticSkill(string name)
        {
            ISkillRepository statSkillRepo = new EFSkillRepository();
            return statSkillRepo.DbStaticSkills.FirstOrDefault(s => s.dbName == name);
        }

        public static IEnumerable<DbStaticSkill> GetAllStaticSkills()
        {
            ISkillRepository statSkillRepo = new EFSkillRepository();
            return statSkillRepo.DbStaticSkills;
        }

        public static IEnumerable<DbStaticSkill> GetLearnablePsychopathSkills()
        {
            ISkillRepository statSkillRepo = new EFSkillRepository();
            return statSkillRepo.DbStaticSkills.Where(s => s.dbName != "" && s.dbName != "lowerHealth" && s.ExclusiveToForm == null && s.GivesEffect == null && (s.LearnedAtLocation != null || s.LearnedAtLocation != null) && s.MobilityType != PvPStatics.MobilityFull && s.MobilityType != PvPStatics.MobilityMindControl && s.IsLive == "live");
        }

        public static IEnumerable<DbStaticSkill> GetFormSpecificSkills(string formdbName)
        {
            ISkillRepository statSkillRepo = new EFSkillRepository();
            return statSkillRepo.DbStaticSkills.Where(s => s.ExclusiveToForm == formdbName);
        }

        public static IEnumerable<DbStaticSkill> GetItemSpecificSkills(string itemdbaName)
        {
            ISkillRepository statSkillRepo = new EFSkillRepository();
            return statSkillRepo.DbStaticSkills.Where(s => s.ExclusiveToItem == itemdbaName);
        }

        public static IEnumerable<DbStaticSkill> GetSkillsLearnedAtLocation(string locationName)
        {
            ISkillRepository statSkillRepo = new EFSkillRepository();
            return statSkillRepo.DbStaticSkills.Where(s => s.LearnedAtLocation == locationName && s.GivesEffect == null && s.IsLive == "live").ToList();
        }

        public static IEnumerable<DbStaticSkill> GetSkillsLearnedAtRegion(string regionName)
        {
            ISkillRepository statSkillRepo = new EFSkillRepository();
            return statSkillRepo.DbStaticSkills.Where(s => s.LearnedAtRegion == regionName && s.GivesEffect == null && s.IsLive == "live").ToList();
        }

    }
}