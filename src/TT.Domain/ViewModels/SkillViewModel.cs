using tfgame.dbModels.Models;

namespace tfgame.ViewModels
{
    public class SkillViewModel
    {
        // this class bundles a skill that the player has with all of its definitions from the file, such as description, cost, etc.

        public Skill dbSkill { get; set; }
        public StaticSkill Skill { get; set; }
        public string MobilityType { get; set; }
        
    }

    public class SkillViewModel2
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