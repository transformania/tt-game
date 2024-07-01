namespace TT.Domain.Effects.DTOs
{
    public class EffectSourceDetail
    {
        public string FriendlyName { get; protected internal set; }
        public string Description { get; protected internal set; }
        public int AvailableAtLevel { get; protected internal set; }
        public string PreRequesite { get; protected internal set; }

        public bool isLevelUpPerk { get; protected internal set; }
        public int Duration { get; protected internal set; }
        public int Cooldown { get; protected internal set; }

        public string ObtainedAtLocation { get; protected internal set; }

        public bool IsRemovable { get; protected internal set; }

        public int BlessingCurseStatus { get; protected internal set; }

        public string MessageWhenHit { get; protected internal set; }
        public string MessageWhenHit_M { get; protected internal set; }
        public string MessageWhenHit_F { get; protected set; }

        public string AttackerWhenHit { get; protected set; }
        public string AttackerWhenHit_M { get; protected set; }
        public string AttackerWhenHit_F { get; protected set; }

        public decimal HealthBonusPercent { get; protected set; }
        public decimal ManaBonusPercent { get; protected set; }
        public decimal ExtraSkillCriticalPercent { get; protected set; }
        public decimal HealthRecoveryPerUpdate { get; protected set; }
        public decimal ManaRecoveryPerUpdate { get; protected set; }
        public decimal SneakPercent { get; protected set; }
        public decimal EvasionPercent { get; protected set; }
        public decimal EvasionNegationPercent { get; protected set; }
        public decimal MeditationExtraMana { get; protected set; }
        public decimal CleanseExtraHealth { get; protected set; }
        public decimal MoveActionPointDiscount { get; protected set; }
        public decimal SpellExtraTFEnergyPercent { get; protected set; }
        public decimal SpellExtraHealthDamagePercent { get; protected set; }
        public decimal CleanseExtraTFEnergyRemovalPercent { get; protected set; }
        public decimal SpellMisfireChanceReduction { get; protected set; }
        public decimal SpellHealthDamageResistance { get; protected set; }
        public decimal SpellTFEnergyDamageResistance { get; protected set; }
        public decimal ExtraInventorySpace { get; protected set; }

        public float Discipline { get; protected set; }
        public float Perception { get; protected set; }
        public float Charisma { get; protected set; }
        public float Submission_Dominance { get; protected set; }

        public float Fortitude { get; protected set; }
        public float Agility { get; protected set; }
        public float Allure { get; protected set; }
        public float Corruption_Purity { get; protected set; }

        public float Magicka { get; protected set; }
        public float Succour { get; protected set; }
        public float Luck { get; protected set; }
        public float Chaos_Order { get; protected set; }
    }
}
