﻿
using TT.Domain.Effects.Entities;
using TT.Domain.Entities;
using TT.Domain.Statics;

namespace TT.Domain.Items.Entities
{
    public class ItemSource : Entity<int>
    {

        public string FriendlyName { get; protected set; }
        public string Description { get; protected set; }
        public string PortraitUrl { get; protected set; }
        public decimal MoneyValue { get; protected set; }
        public decimal MoneyValueSell { get; protected set; }
        public string ItemType { get; protected set; }
        public int UseCooldown { get; protected set; }
        public string UsageMessage_Item { get; protected set; }
        public string UsageMessage_Player { get; protected set; }
        public bool Findable { get; protected set; }
        public double FindWeight { get; protected set; }
        public EffectSource GivesEffectSource { get; protected set; }
        public bool IsUnique { get; protected set; }
        public int? CurseTFFormSourceId { get; protected set; } // TODO: Use FormSource entity here instead

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

        public decimal InstantHealthRestore { get; protected set; }
        public decimal InstantManaRestore { get; protected set; }
        public decimal ReuseableHealthRestore { get; protected set; }
        public decimal ReuseableManaRestore { get; protected set; }

        public int? RuneLevel { get; protected set; }

        private ItemSource() { }

        public static ItemSource Create()
        {
            return new ItemSource
            {
                
            };
        }

        public bool IsPermanentFromCreation()
        {
            return this.ItemType == PvPStatics.ItemType_Consumable || this.ItemType == PvPStatics.ItemType_Rune;
        }

    }
}