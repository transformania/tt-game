using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.ViewModels
{
    public class BuffBox
    {

        // holds all of the bonuses obtained from equipment
        public decimal FromItems_HealthBonusPercent { get; set; }
        public decimal FromItems_ManaBonusPercent { get; set; }
        public decimal FromItems_ExtraSkillCriticalPercent { get; set; }
        public decimal FromItems_HealthRecoveryPerUpdate { get; set; }
        public decimal FromItems_ManaRecoveryPerUpdate { get; set; }
        public decimal FromItems_SneakPercent { get; set; }
        public decimal FromItems_EvasionPercent { get; set; }
        public decimal FromItems_EvasionNegationPercent { get; set; }
        public decimal FromItems_MeditationExtraMana { get; set; }
        public decimal FromItems_CleanseExtraHealth { get; set; }
        public decimal FromItems_MoveActionPointDiscount { get; set; }
        public decimal FromItems_SpellExtraTFEnergyPercent { get; set; }
        public decimal FromItems_SpellExtraHealthDamagePercent { get; set; }
        public decimal FromItems_CleanseExtraTFEnergyRemovalPercent { get; set; }
        public decimal FromItems_SpellMisfireChanceReduction { get; set; }
        public decimal FromItems_SpellHealthDamageResistance { get; set; }
        public decimal FromItems_SpellTFEnergyDamageResistance { get; set; }
        public decimal FromItems_ExtraInventorySpace { get; set; }

        public decimal FromForm_HealthBonusPercent { get; set; }
        public decimal FromForm_ManaBonusPercent { get; set; }
        public decimal FromForm_ExtraSkillCriticalPercent { get; set; }
        public decimal FromForm_HealthRecoveryPerUpdate { get; set; }
        public decimal FromForm_ManaRecoveryPerUpdate { get; set; }
        public decimal FromForm_SneakPercent { get; set; }
        public decimal FromForm_EvasionPercent { get; set; }
        public decimal FromForm_EvasionNegationPercent { get; set; }
        public decimal FromForm_MeditationExtraMana { get; set; }
        public decimal FromForm_CleanseExtraHealth { get; set; }
        public decimal FromForm_MoveActionPointDiscount { get; set; }
        public decimal FromForm_SpellExtraTFEnergyPercent { get; set; }
        public decimal FromForm_SpellExtraHealthDamagePercent { get; set; }
        public decimal FromForm_CleanseExtraTFEnergyRemovalPercent { get; set; }
        public decimal FromForm_SpellMisfireChanceReduction { get; set; }
        public decimal FromForm_SpellHealthDamageResistance { get; set; }
        public decimal FromForm_SpellTFEnergyDamageResistance { get; set; }
        public decimal FromForm_ExtraInventorySpace { get; set; }

        public decimal FromEffects_HealthBonusPercent { get; set; }
        public decimal FromEffects_ManaBonusPercent { get; set; }
        public decimal FromEffects_ExtraSkillCriticalPercent { get; set; }
        public decimal FromEffects_HealthRecoveryPerUpdate { get; set; }
        public decimal FromEffects_ManaRecoveryPerUpdate { get; set; }
        public decimal FromEffects_SneakPercent { get; set; }
        public decimal FromEffects_EvasionPercent { get; set; }
        public decimal FromEffects_EvasionNegationPercent { get; set; }
        public decimal FromEffects_MeditationExtraMana { get; set; }
        public decimal FromEffects_CleanseExtraHealth { get; set; }
        public decimal FromEffects_MoveActionPointDiscount { get; set; }
        public decimal FromEffects_SpellExtraTFEnergyPercent { get; set; }
        public decimal FromEffects_SpellExtraHealthDamagePercent { get; set; }
        public decimal FromEffects_CleanseExtraTFEnergyRemovalPercent { get; set; }
        public decimal FromEffects_SpellMisfireChanceReduction { get; set; }
        public decimal FromEffects_SpellHealthDamageResistance { get; set; }
        public decimal FromEffects_SpellTFEnergyDamageResistance { get; set; }
        public decimal FromEffects_ExtraInventorySpace { get; set; }

        public decimal HealthBonusPercent() { return FromItems_HealthBonusPercent + FromForm_HealthBonusPercent + FromEffects_HealthBonusPercent; }
        public decimal ManaBonusPercent() { return FromItems_ManaBonusPercent + FromForm_ManaBonusPercent + FromEffects_ManaBonusPercent; }
        public decimal ExtraSkillCriticalPercent() { return FromItems_ExtraSkillCriticalPercent + FromForm_ExtraSkillCriticalPercent + FromEffects_ExtraSkillCriticalPercent ; }
        public decimal HealthRecoveryPerUpdate() { return FromItems_HealthRecoveryPerUpdate + FromForm_HealthRecoveryPerUpdate + FromEffects_HealthRecoveryPerUpdate; }
        public decimal ManaRecoveryPerUpdate() { return FromItems_ManaRecoveryPerUpdate + FromForm_ManaRecoveryPerUpdate + FromEffects_ManaRecoveryPerUpdate; }
        public decimal SneakPercent() { return FromItems_SneakPercent + FromForm_SneakPercent + FromEffects_SneakPercent; }
        public decimal EvasionPercent() { return FromItems_EvasionPercent + FromForm_EvasionPercent + FromEffects_EvasionPercent; }
        public decimal EvasionNegationPercent() { return FromItems_EvasionNegationPercent + FromForm_EvasionNegationPercent + FromEffects_EvasionNegationPercent; ; }
        public decimal MeditationExtraMana() { return FromItems_MeditationExtraMana + FromForm_MeditationExtraMana + FromEffects_MeditationExtraMana; }
        public decimal CleanseExtraHealth() { return FromItems_CleanseExtraHealth + FromForm_CleanseExtraHealth + FromEffects_CleanseExtraHealth; }
        public decimal MoveActionPointDiscount() { return FromItems_MoveActionPointDiscount + FromForm_MoveActionPointDiscount + FromEffects_MoveActionPointDiscount; }
        public decimal SpellExtraTFEnergyPercent() { return FromItems_SpellExtraTFEnergyPercent + FromForm_SpellExtraTFEnergyPercent + FromEffects_SpellExtraTFEnergyPercent; }
        public decimal SpellExtraHealthDamagePercent() { return FromItems_SpellExtraHealthDamagePercent + FromForm_SpellExtraHealthDamagePercent + FromEffects_SpellExtraHealthDamagePercent; }
        public decimal CleanseExtraTFEnergyRemovalPercent() { return FromItems_CleanseExtraTFEnergyRemovalPercent + FromForm_CleanseExtraTFEnergyRemovalPercent + FromEffects_CleanseExtraTFEnergyRemovalPercent; ; }
        public decimal SpellMisfireChanceReduction() { return FromItems_SpellMisfireChanceReduction + FromForm_SpellMisfireChanceReduction + FromEffects_SpellMisfireChanceReduction; }

        public decimal SpellHealthDamageResistance() { return FromItems_SpellHealthDamageResistance + FromForm_SpellHealthDamageResistance + FromEffects_SpellHealthDamageResistance; }
        public decimal SpellTFEnergyDamageResistance() { return FromItems_SpellTFEnergyDamageResistance + FromForm_SpellTFEnergyDamageResistance + FromEffects_SpellTFEnergyDamageResistance; }

        public bool HasSearchDiscount;
        public int EnchantmentBoost;

        // there is a special consideration for extra inventory space so -1.2 from an item doesn't get rounded down to -2 later on
        public decimal ExtraInventorySpace() {
            decimal output = FromItems_ExtraInventorySpace + FromForm_ExtraInventorySpace + FromEffects_ExtraInventorySpace;

            // if the output from items is less than 0, round up
            if (FromItems_ExtraInventorySpace < 0)
            {
                output = Math.Ceiling(FromItems_ExtraInventorySpace) + FromForm_ExtraInventorySpace + FromEffects_ExtraInventorySpace;
            }
            return output;

        }

        #region non-stat perks

        

        #endregion
    }

    public class RAMBuffBox
    {
        public string dbName { get; set; }
        public float HealthBonusPercent { get; set; }
        public float ManaBonusPercent { get; set; }
        //public float ExtraSkillCriticalPercent { get; set; }
        public float HealthRecoveryPerUpdate { get; set; }
        public float ManaRecoveryPerUpdate { get; set; }
        //public float SneakPercent { get; set; }
        //public float EvasionPercent { get; set; }
        //public float EvasionNegationPercent { get; set; }
        //public float MeditationExtraMana { get; set; }
        //public float CleanseExtraHealth { get; set; }
        //public float MoveActionPointDiscount { get; set; }
        //public float SpellExtraTFEnergyPercent { get; set; }
        //public float SpellExtraHealthDamagePercent { get; set; }
        //public float CleanseExtraTFEnergyRemovalPercent { get; set; }
        //public float SpellMisfireChanceReduction { get; set; }
        //public float SpellHealthDamageResistance { get; set; }
        //public float SpellTFEnergyDamageResistance { get; set; }
        //public float ExtraInventorySpace { get; set; }
    }
}