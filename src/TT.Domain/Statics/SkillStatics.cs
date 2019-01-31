using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;

namespace TT.Domain.Statics
{
    public static class SkillStatics
    {

        public const int WeakenSkillSourceId = 312;

        public static DbStaticSkill GetStaticSkill(int skillSourceId)
        {
            ISkillRepository statSkillRepo = new EFSkillRepository();
            return statSkillRepo.DbStaticSkills.FirstOrDefault(s => s.Id == skillSourceId);
        }

        public static IEnumerable<DbStaticSkill> GetAllStaticSkills()
        {
            ISkillRepository statSkillRepo = new EFSkillRepository();
            return statSkillRepo.DbStaticSkills;
        }

        public static IEnumerable<DbStaticSkill> GetLearnablePsychopathSkills()
        {
            ISkillRepository statSkillRepo = new EFSkillRepository();
            return statSkillRepo.DbStaticSkills.Where(s => s.Id != PvPStatics.Spell_WeakenId && s.ExclusiveToFormSourceId == null && s.GivesEffectSourceId == null && (s.LearnedAtLocation != null || s.LearnedAtLocation != null) && s.MobilityType != PvPStatics.MobilityFull && s.MobilityType != PvPStatics.MobilityMindControl && s.IsLive == "live");
        }

        public static IEnumerable<DbStaticSkill> GetFormSpecificSkills(int formSourceId)
        {
            ISkillRepository statSkillRepo = new EFSkillRepository();
            return statSkillRepo.DbStaticSkills.Where(s => s.ExclusiveToFormSourceId == formSourceId);
        }

        public static IEnumerable<DbStaticSkill> GetItemSpecificSkills(string itemdbaName)
        {
            ISkillRepository statSkillRepo = new EFSkillRepository();
            return statSkillRepo.DbStaticSkills.Where(s => s.ExclusiveToItem == itemdbaName);
        }

        public static IEnumerable<DbStaticSkill> GetSkillsLearnedAtLocation(string locationName)
        {
            ISkillRepository statSkillRepo = new EFSkillRepository();
            return statSkillRepo.DbStaticSkills.Where(s => s.LearnedAtLocation == locationName && s.GivesEffectSourceId == null && s.IsLive == "live").ToList();
        }

        public static IEnumerable<DbStaticSkill> GetSkillsLearnedAtRegion(string regionName)
        {
            ISkillRepository statSkillRepo = new EFSkillRepository();
            return statSkillRepo.DbStaticSkills.Where(s => s.LearnedAtRegion == regionName && s.GivesEffectSourceId == null && s.IsLive == "live").ToList();
        }

    }
}