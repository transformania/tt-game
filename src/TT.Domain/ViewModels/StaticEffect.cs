namespace TT.Domain.ViewModels
{
    public class StaticEffect
    {
        public int Id { get; set; }
        public string FriendlyName { get; set; }
        public string Description { get; set; }
        public int AvailableAtLevel { get; set; }
        public int? PreRequisiteEffectSourceId { get; set; }

        public bool isLevelUpPerk { get; set; }
        public int Duration { get; set; }
        public int Cooldown { get; set; }

        public string ObtainedAtLocation { get; set; }
        public bool IsRemovable { get; set; }

        public int BlessingCurseStatus { get; set; }

        public string MessageWhenHit { get; set; }
        public string MessageWhenHit_M { get; set; }
        public string MessageWhenHit_F { get; set; }

        public string AttackerWhenHit { get; set; }
        public string AttackerWhenHit_M { get; set; }
        public string AttackerWhenHit_F { get; set; }

        public decimal HealthBonusPercent { get; set; }
        public decimal ManaBonusPercent { get; set; }
        public decimal ExtraSkillCriticalPercent { get; set; }
        public decimal HealthRecoveryPerUpdate { get; set; }
        public decimal ManaRecoveryPerUpdate { get; set; }
        public decimal SneakPercent { get; set; }
        public decimal EvasionPercent { get; set; }
        public decimal EvasionNegationPercent { get; set; }
        public decimal MeditationExtraMana { get; set; }
        public decimal CleanseExtraHealth { get; set; }
        public decimal MoveActionPointDiscount { get; set; }
        public decimal SpellExtraTFEnergyPercent { get; set; }
        public decimal SpellExtraHealthDamagePercent { get; set; }
        public decimal CleanseExtraTFEnergyRemovalPercent { get; set; }
        public decimal SpellMisfireChanceReduction { get; set; }
        public decimal SpellHealthDamageResistance { get; set; }
        public decimal SpellTFEnergyDamageResistance { get; set; }
        public decimal ExtraInventorySpace { get; set; }

        public decimal InstantHealthRestore { get; set; }
        public decimal InstantManaRestore { get; set; }

        public float Discipline { get; set; }
        public float Perception { get; set; }
        public float Charisma { get; set; }
        public float Submission_Dominance { get; set; }

        public float Fortitude { get; set; }
        public float Agility { get; set; }
        public float Allure { get; set; }
        public float Corruption_Purity { get; set; }

        public float Magicka { get; set; }
        public float Succour { get; set; }
        public float Luck { get; set; }
        public float Chaos_Order { get; set; }

     

    }
}