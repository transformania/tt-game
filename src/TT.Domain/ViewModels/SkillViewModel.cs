using TT.Domain.Models;

namespace TT.Domain.ViewModels
{
    public class SkillViewModel
    {
        public Skill_VM dbSkill { get; set; }
        public StaticSkill Skill { get; set; }
        public string MobilityType { get; set; }
    }
}