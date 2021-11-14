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

        public static IEnumerable<DbStaticSkill> GetItemSpecificSkills(int itemSourceId)
        {
            ISkillRepository statSkillRepo = new EFSkillRepository();
            return statSkillRepo.DbStaticSkills.Where(s => s.ExclusiveToItemSourceId == itemSourceId);
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

        public static int GetNumPlayerLearnableSkills()
        {
            ISkillRepository statSkillRepo = new EFSkillRepository();
            var locations = LocationsStatics.LocationList.GetLocation.Select(l => l.dbName).ToHashSet();
            var regions = LocationsStatics.LocationList.GetLocation.Select(l => l.Region).ToHashSet();
            var counts = statSkillRepo.DbStaticSkills.Where(s => s.GivesEffectSourceId == null && s.IsLive == "live")
                                                     .GroupBy(s => new { s.LearnedAtRegion, s.LearnedAtLocation })
                                                     .Select(g => new { Spell = g.Key, Count = g.Count() }).ToList();

            return counts.Where(g => regions.Contains(g.Spell.LearnedAtRegion) || locations.Contains(g.Spell.LearnedAtLocation))
                         .Sum(g => g.Count);
        }

    }
}