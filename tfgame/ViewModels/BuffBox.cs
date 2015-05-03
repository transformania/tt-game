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

        public float FromItems_Discipline { get; set; }
        public float FromItems_Perception { get; set; }
        public float FromItems_Charisma { get; set; }
        public float FromItems_Submission_Dominance { get; set; }
        public float FromItems_Fortitude { get; set; }
        public float FromItems_Agility { get; set; }
        public float FromItems_Allure { get; set; }
        public float FromItems_Corruption_Purity { get; set; }
        public float FromItems_Magicka { get; set; }
        public float FromItems_Succour { get; set; }
        public float FromItems_Luck { get; set; }
        public float FromItems_Chaos_Order { get; set; }

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

        public float FromForm_Discipline { get; set; }
        public float FromForm_Perception { get; set; }
        public float FromForm_Charisma { get; set; }
        public float FromForm_Submission_Dominance { get; set; }
        public float FromForm_Fortitude { get; set; }
        public float FromForm_Agility { get; set; }
        public float FromForm_Allure { get; set; }
        public float FromForm_Corruption_Purity { get; set; }
        public float FromForm_Magicka { get; set; }
        public float FromForm_Succour { get; set; }
        public float FromForm_Luck { get; set; }
        public float FromForm_Chaos_Order { get; set; }

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

        public float FromEffects_Discipline { get; set; }
        public float FromEffects_Perception { get; set; }
        public float FromEffects_Charisma { get; set; }
        public float FromEffects_Submission_Dominance { get; set; }
        public float FromEffects_Fortitude { get; set; }
        public float FromEffects_Agility { get; set; }
        public float FromEffects_Allure { get; set; }
        public float FromEffects_Corruption_Purity { get; set; }
        public float FromEffects_Magicka { get; set; }
        public float FromEffects_Succour { get; set; }
        public float FromEffects_Luck { get; set; }
        public float FromEffects_Chaos_Order { get; set; }

        public bool HasSearchDiscount;
        public int EnchantmentBoost;

        public float Discipline() { return FromItems_Discipline + FromForm_Discipline + FromEffects_Discipline; }
        public float Perception() { return FromItems_Perception + FromForm_Perception + FromEffects_Perception; }
        public float Charisma() { return FromItems_Charisma + FromForm_Charisma + FromEffects_Charisma; }
        public float Submission_Dominance() { return FromItems_Submission_Dominance + FromForm_Submission_Dominance + FromEffects_Submission_Dominance; }
        public float Fortitude() { return FromItems_Fortitude + FromForm_Fortitude + FromEffects_Fortitude; }
        public float Agility() { return FromItems_Agility + FromForm_Agility + FromEffects_Agility; }
        public float Allure() { return FromItems_Allure + FromForm_Allure + FromEffects_Allure; }
        public float Corruption_Purity() { return FromItems_Corruption_Purity + FromForm_Corruption_Purity + FromEffects_Corruption_Purity; }
        public float Magicka() { return FromItems_Magicka + FromForm_Magicka + FromEffects_Magicka; }
        public float Succour() { return FromItems_Succour + FromForm_Succour + FromEffects_Succour; }
        public float Luck() { return FromItems_Luck + FromForm_Luck + FromEffects_Luck; }
        public float Chaos_Order() { return FromItems_Chaos_Order + FromForm_Chaos_Order + FromEffects_Chaos_Order; }

        // new system
        public decimal HealthBonusPercent()
        {
            return Convert.ToDecimal(
              0.1F * Discipline() +
              -0.1F * Allure()
             );
        }

        public decimal ManaBonusPercent()
        {
            return Convert.ToDecimal(
               .2F * Magicka() +
               -.1F * Succour()
            );
        }

        public decimal ExtraSkillCriticalPercent()
        {
            return Convert.ToDecimal(
               .1F * Luck() +
               -.1F * Discipline()
            );
        }

        public decimal HealthRecoveryPerUpdate()
        {
            return Convert.ToDecimal(
               .1F * Succour()
            );
        }

        public decimal ManaRecoveryPerUpdate()
        {
            return Convert.ToDecimal(
               .1F * Succour()
            );
        }

        public decimal SneakPercent()
        {
            return Convert.ToDecimal(
               .1F * Perception() +
               -.4F * Fortitude() + 
               .15F * Agility() +
               -.2F * Allure()
            );
        }

        public decimal EvasionPercent()
        {
            return Convert.ToDecimal(
               -.2F * Fortitude() +
               .2F * Agility()
            );
        }

        public decimal EvasionNegationPercent()
        {
            return Convert.ToDecimal(
               .3F * Perception() +
               .2F * Allure() +
               -.2F * Magicka()
            );
        }

        public decimal MeditationExtraMana()
        {
            return Convert.ToDecimal(
               -.1F * Perception() +
               .1F * Magicka()
            );
        }

        public decimal CleanseExtraHealth()
        {
            return Convert.ToDecimal(
               .05F * Discipline() +
               -.05F * Fortitude()
            );
        }

        public decimal MoveActionPointDiscount()
        {
            return Convert.ToDecimal(
               -.005F * Perception() +
               .008F * Agility()
            );
        }

        public decimal SpellExtraTFEnergyPercent()
        {
            return Convert.ToDecimal(
               .2F * Charisma() +
               .2F * Magicka() +
               -.1F * Succour()
            );
        }

        public decimal SpellExtraHealthDamagePercent()
        {
            return Convert.ToDecimal(
               .2F * Charisma() +
               -.1F * Discipline() +
               .3F * Allure()
            );
        }

        public decimal CleanseExtraTFEnergyRemovalPercent()
        {
            return Convert.ToDecimal(
               -.05F * Magicka() +
               .075F * Luck()
            );
        }

        public decimal SpellMisfireChanceReduction()
        {
            return Convert.ToDecimal(
               .15F * Perception() +
               -.05F * Charisma()
            );
        }

        public decimal SpellHealthDamageResistance()
        {
            return Convert.ToDecimal(
                .2F * Discipline() +
               -.1F * Charisma() +
               -.1F * Luck()
            );
        }

        public decimal SpellTFEnergyDamageResistance()
        {
            return Convert.ToDecimal(
                .2F * Fortitude() +
                -.1F * Agility() +
                -.075F * Luck()
            );
        }

        public decimal ExtraInventorySpace()
        {
            decimal amt = Convert.ToDecimal(
                .05F * Fortitude() +
                -.05F * Agility()
            );

            amt = Math.Floor(amt);

            return amt;
        }
        


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