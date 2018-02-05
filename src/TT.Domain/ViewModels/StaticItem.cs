namespace TT.Domain.ViewModels
{
    public class StaticItem
    {
        public int Id { get; set; }
        public string dbName { get; set; }
        public string FriendlyName { get; set; }
        public string Description { get; set; }
        public string PortraitUrl { get; set; }
        public decimal MoneyValue { get; set; }
        public decimal MoneyValueSell { get; set; }
        public string ItemType { get; set; }
        public int UseCooldown { get; set; }
        public string UsageMessage_Item { get; set; }
        public string UsageMessage_Player { get; set; }
        public bool Findable { get; set; }
        public double FindWeight { get; set; }
        public string GivesEffect { get; set; }


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

        public decimal InstantHealthRestore { get; set; }
        public decimal InstantManaRestore { get; set; }
        public decimal ReuseableHealthRestore { get; set; }
        public decimal ReuseableManaRestore { get; set; }

        public string CurseTFFormdbName { get; set; }

    }
}