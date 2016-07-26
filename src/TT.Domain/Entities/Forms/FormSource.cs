namespace TT.Domain.Entities.Forms
{
    public class FormSource : Entity<int>
    {
        public string dbName { get; protected set; }
        public string FriendlyName { get; protected set; }
        public string Description { get; protected set; }
        public string TFEnergyType { get; protected set; }
        public decimal TFEnergyRequired { get; protected set; }
        public string Gender { get; protected set; }
        public string MobilityType { get; protected set; }
        public string BecomesItemDbName { get; protected set; }
        public string PortraitUrl { get; protected set; }
        public bool IsUnique { get; protected set; }

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

        public float Fortitude { get; protected set; }
        public float Agility { get; protected set; }
        public float Allure { get; protected set; }

        public float Magicka { get; protected set; }
        public float Succour { get; protected set; }
        public float Luck { get; protected set; }

        public TFMessage TfMessage { get; protected set; }

        private FormSource()
        { 

        }

        public void SetTFMessage(TFMessage tfMessage)
        {
            this.TfMessage = tfMessage;
        }
    }

    
}
