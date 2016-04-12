using TT.Domain.Models;

namespace TT.Domain.ViewModels
{
    public class SkillViewModel
    {
        public Skill_VM dbSkill { get; set; }
        public StaticSkill Skill { get; set; }
        public string MobilityType { get; set; }
    }

    public class MySkillsViewModel
    {
        public string Skill_FriendlyName { get; set; }
        public string Skill_Name { get; set; }
        public decimal Skill_Charge { get; set; }
        public decimal Skill_Duration { get; set; }
        public string Skill_MobilityType { get; set; }
        public int Skill_Id { get; set; }
        public bool Skill_IsArchived { get; set; }
        public string Skill_Description { get; set; }
        public decimal Skill_ManaCost { get; set; }
        public decimal Skill_HealthDamageAmount { get; set; }
        public decimal Skill_TFPointsAmount { get; set; }
        public string Skill_GivesEffect { get; set; }
        public string Form_FriendlyName { get; set; }
    }
}