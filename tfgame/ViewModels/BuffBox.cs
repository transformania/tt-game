using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.ViewModels
{
    public class BuffStoredProc
    {
        // holds all of the bonuses
        public string Type { get; set; }
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
    }

    public class BuffDetail
    {
        public string Description { get; set; }
        public List<string> PlusIcons { get; set; }
        public List<string> MinusIcons { get; set; }
    }

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

            float output = 0;
            string substat = "HealthBonusPercent"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            output += (float)FromForm_HealthBonusPercent;


            return Convert.ToDecimal(output);

        }

        public decimal ManaBonusPercent()
        {
            float output = 0;
            string substat = "ManaBonusPercent"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            output += (float)FromForm_ManaBonusPercent;

            return Convert.ToDecimal(output);

        }

        public decimal ExtraSkillCriticalPercent()
        {
            float output = 0;
            string substat = "ExtraSkillCriticalPercent"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            return Convert.ToDecimal(output);
        }

        public decimal HealthRecoveryPerUpdate()
        {
            float output = 0;
            string substat = "HealthRecoveryPerUpdate"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            output += (float)FromForm_HealthRecoveryPerUpdate;
            output += (float)FromEffects_HealthRecoveryPerUpdate;

            return Convert.ToDecimal(output);
        }

        public decimal ManaRecoveryPerUpdate()
        {
            float output = 0;
            string substat = "ManaRecoveryPerUpdate"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            output += (float)FromForm_ManaRecoveryPerUpdate;
            output += (float)FromEffects_ManaRecoveryPerUpdate;

            return Convert.ToDecimal(output);
        }

        public decimal SneakPercent()
        {
            float output = 0;
            string substat = "SneakPercent"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            return Convert.ToDecimal(output);
        }

        public decimal EvasionPercent()
        {
            float output = 0;
            string substat = "EvasionPercent"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            return Convert.ToDecimal(output);
        }

        public decimal EvasionNegationPercent()
        {
            float output = 0;
            string substat = "EvasionNegationPercent"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            return Convert.ToDecimal(output);
        }

        public decimal MeditationExtraMana()
        {
            float output = 0;
            string substat = "MeditationExtraMana"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            return Convert.ToDecimal(output);
        }

        public decimal CleanseExtraHealth()
        {
            float output = 0;
            string substat = "CleanseExtraHealth"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            return Convert.ToDecimal(output);
        }

        public decimal MoveActionPointDiscount()
        {
            float output = 0;
            string substat = "MoveActionPointDiscount"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            output += (float)FromForm_MoveActionPointDiscount;
            output += (float)FromEffects_MoveActionPointDiscount;

            return Convert.ToDecimal(output);
        }

        public decimal SpellExtraTFEnergyPercent()
        {
            float output = 0;
            string substat = "SpellExtraTFEnergyPercent"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            return Convert.ToDecimal(output);
        }

        public decimal SpellExtraHealthDamagePercent()
        {
            float output = 0;
            string substat = "SpellExtraHealthDamagePercent"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            return Convert.ToDecimal(output);
        }

        public decimal CleanseExtraTFEnergyRemovalPercent()
        {
            float output = 0;
            string substat = "CleanseExtraTFEnergyRemovalPercent"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            return Convert.ToDecimal(output);
        }

        public decimal SpellMisfireChanceReduction()
        {
            float output = 0;
            string substat = "SpellMisfireChanceReduction"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            return Convert.ToDecimal(output);
        }

        public decimal SpellHealthDamageResistance()
        {
            float output = 0;
            string substat = "SpellHealthDamageResistance"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            return Convert.ToDecimal(output);
        }

        public decimal SpellTFEnergyDamageResistance()
        {
            float output = 0;
            string substat = "SpellTFEnergyDamageResistance"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            return Convert.ToDecimal(output);
        }

        public decimal ExtraInventorySpace()
        {
            float output = 0;
            string substat = "ExtraInventorySpace"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            output = (float)Math.Floor(output);

            return Convert.ToDecimal(output);
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

    public static class BuffMap
    {

        private const string DefenseIconClass = "icon-defense";
        private const string WPUpIconClass = "icon-health_recovery";
        private const string ManaUpIconClass = "icon-mana_recovery";
        private const string RestorationIconCLass = "icon-cleansemeditate";
        private const string AttackIconClass = "icon-timesattacking";

        public static Dictionary<string, BuffDetail> BuffDetailsMap = new Dictionary<string, BuffDetail>{

            
            { 
                "Discipline", 
                new BuffDetail {
                    Description = "Discipline affects the strength of your mental defenses; resistance to willpower attacks and ability to recover lost willpower. Improving your defense against willpower attacks reduces the power of your own willpower attacks by a small amount.", 
                    PlusIcons = new List<string> { DefenseIconClass, WPUpIconClass }, 
                    MinusIcons =  new  List<string> { AttackIconClass }
                }
            },
            { 
                "Perception", 
                new BuffDetail {
                    Description = "Perception affects the accuracy of your spell-casting and your ability to detect hidden things. The price that you pay for taking the time to observe and analyze your surroundings is slower movement speed and a reduction to the amount of mana you glean from meditation.", 
                    PlusIcons = new List<string> { AttackIconClass }, 
                    MinusIcons =  new  List<string> { RestorationIconCLass }
                }
            },
            { 
                "Charisma", 
                new BuffDetail {
                    Description = "Charisma affects the strength of your willpower and transformation attacks; it represents your ability to influence people and bend them to your design. However, the desire to persuade others leaves you vulnerable to their desires in turn, reducing your willpower defenses slightly.", 
                    PlusIcons = new List<string> { AttackIconClass }, 
                    MinusIcons =  new  List<string> { DefenseIconClass }
                }
            },
            { 
                "Fortitude", 
                new BuffDetail {
                    Description = "Fortitude affects the strength of your physical defenses; resistance to transformation attacks and the ability to carry more gear. Toughening your body reduces its limberness, inhibiting your ability to move stealthily and dodge incoming attacks.", 
                    PlusIcons = new List<string> { DefenseIconClass }, 
                    MinusIcons =  new  List<string> { "" }
                }
            },
            { 
                "Agility", 
                new BuffDetail {
                    Description = "Agility affects your ability to move quickly, which helps you dodge attacks and avoid detection. Quickness requires a light build that is not able to take a lot of physical abuse; thus your resistance to transformation damage is inhibited and you are not able to carry as much gear.", 
                    PlusIcons = new List<string> { DefenseIconClass }, 
                    MinusIcons =  new  List<string> { DefenseIconClass }
                }
            },
            { 
                "Allure", 
                new BuffDetail {
                    Description = "Allure affects your innate sex appeal or animal magnetism. The more attractive you are, the more susceptible your opponents are to your willpower attacks and the less likely they are to dodge. Yet physical beauty makes one susceptible to vanity, which increases your vulnerability to willpower damage. Your attractiveness also makes it hard to go unnoticed.", 
                    PlusIcons = new List<string> { AttackIconClass }, 
                    MinusIcons =  new  List<string> { DefenseIconClass, }
                }
            },
            { 
                "Magicka", 
                new BuffDetail {
                    Description = "Magicka reflects you raw magic potential; the stronger your connection to the ambient magical fields around you, the more mana you have at your disposal and the more transformation energy you can focus on your opponents. Yet having so much magic flowing through you makes it harder to purge transformation energy, and interferes with your ability to focus on your targets.", 
                    PlusIcons = new List<string> { ManaUpIconClass }, 
                    MinusIcons =  new  List<string> { AttackIconClass, RestorationIconCLass  }
                }
            },
            { 
                "Succour", 
                new BuffDetail {
                    Description = "Succour affects your ability to tap into ambient magical energies and manipulate them to recover mana and willpower. Some of your mana is constantly diverted into the recovery of mana and willpower, reducing your maximum pool. Focusing on the healing powers of magic invokes a natural aversion toward harming others.", 
                    PlusIcons = new List<string> { RestorationIconCLass }, 
                    MinusIcons =  new  List<string> { ManaUpIconClass, AttackIconClass }
                }
            },
            { 
                "Luck", 
                new BuffDetail {
                    Description = "Luck is a random chance that seems to operate in your favor. Or maybe the gods look kindly upon you. However you choose to rationalize it, luck affects your chance to score amazingly successful blows against your opponents. Luck also helps you shrug off accumulated transformation energy. Yet relying on luck can leave you exposed if the dice roll goes against you.", 
                    PlusIcons = new List<string> { AttackIconClass }, 
                    MinusIcons =  new  List<string> { DefenseIconClass }
                }
            },
        };

  

        public static Dictionary<string, Dictionary<string, float>> BuffsMap = new Dictionary<string, Dictionary<string, float>> {



            {
                "HealthBonusPercent",
                new Dictionary<string,float> {
                      	{"Discipline", .5F},
	                    {"Perception", 0},
	                    {"Charisma", 0},
	                    {"Fortitude", 0},
	                    {"Agility", 0},
	                    {"Allure", -.25F},
	                    {"Magicka", 0},
	                    {"Succour", 0},
	                    {"Luck", 0},
                    }
                
                },
                {
                "ManaBonusPercent",
                new Dictionary<string,float> {
                      	{"Discipline", 0},
	                    {"Perception", 0},
	                    {"Charisma", 0},
	                    {"Fortitude", 0},
	                    {"Agility", 0},
	                    {"Allure", 0},
	                    {"Magicka", 1.0F},
	                    {"Succour", -.25F},
	                    {"Luck", 0},
                    }
                },
                {
                "ExtraSkillCriticalPercent",
                new Dictionary<string,float> {
                      	{"Discipline", -.25F},
	                    {"Perception", 0},
	                    {"Charisma", 0},
	                    {"Fortitude", 0},
	                    {"Agility", 0},
	                    {"Allure", 0},
	                    {"Magicka", 0},
	                    {"Succour", 0},
	                    {"Luck", .3F},
                    }
                },
                {
                "HealthRecoveryPerUpdate",
                new Dictionary<string,float> {
                      	{"Discipline", 0},
	                    {"Perception", 0},
	                    {"Charisma", 0},
	                    {"Fortitude", 0},
	                    {"Agility", 0},
	                    {"Allure", 0},
	                    {"Magicka", 0},
	                    {"Succour", .2F},
	                    {"Luck", 0},
                    }
                },
                {
                "ManaRecoveryPerUpdate",
                new Dictionary<string,float> {
                      	{"Discipline", 0},
	                    {"Perception", 0},
	                    {"Charisma", 0},
	                    {"Fortitude", 0},
	                    {"Agility", 0},
	                    {"Allure", 0},
	                    {"Magicka", 0},
	                    {"Succour", .2F},
	                    {"Luck", 0},
                    }
                },
                {
                "SneakPercent",
                new Dictionary<string,float> {
                      	{"Discipline", 0},
	                    {"Perception", 0},
	                    {"Charisma", 0},
	                    {"Fortitude", -1.0F},
	                    {"Agility", 1.25F},
	                    {"Allure", -.5F},
	                    {"Magicka", 0},
	                    {"Succour", 0},
	                    {"Luck", 0},
                    }
                },
                {
                "EvasionPercent",
                new Dictionary<string,float> {
                      	{"Discipline", 0},
	                    {"Perception", 0},
	                    {"Charisma", 0},
	                    {"Fortitude", -.5F},
	                    {"Agility", 1.0F},
	                    {"Allure", 0},
	                    {"Magicka", 0},
	                    {"Succour", 0},
	                    {"Luck", 0},
                    }
                },
                {
                "EvasionNegationPercent",
                new Dictionary<string,float> {
                      	{"Discipline", 0},
	                    {"Perception", 1.5F},
	                    {"Charisma", 0},
	                    {"Fortitude", 0},
	                    {"Agility", 0},
	                    {"Allure", 1.0F},
	                    {"Magicka", -.5F},
	                    {"Succour", 0},
	                    {"Luck", 0},
                    }
                },
                {
                "MeditationExtraMana",
                new Dictionary<string,float> {
                      	{"Discipline", 0},
	                    {"Perception", -.25F},
	                    {"Charisma", 0},
	                    {"Fortitude", 0},
	                    {"Agility", 0},
	                    {"Allure", 0},
	                    {"Magicka", 0.5F},
	                    {"Succour", 0},
	                    {"Luck", 0},
                    }
                },
                {
                "CleanseExtraHealth",
                new Dictionary<string,float> {
                      	{"Discipline", .25F},
	                    {"Perception", 0},
	                    {"Charisma", 0},
	                    {"Fortitude", -.125F},
	                    {"Agility", 0},
	                    {"Allure", 0},
	                    {"Magicka", 0},
	                    {"Succour", 0},
	                    {"Luck", 0},
                    }
                },
                {
                "MoveActionPointDiscount",
                new Dictionary<string,float> {
                      	{"Discipline", 0},
	                    {"Perception", -.0125F},
	                    {"Charisma", 0},
	                    {"Fortitude", 0},
	                    {"Agility", .04F},
	                    {"Allure", 0},
	                    {"Magicka", 0},
	                    {"Succour", 0},
	                    {"Luck", 0},
                    }
                },
                {
                "SpellExtraTFEnergyPercent",
                new Dictionary<string,float> {
                      	{"Discipline", 0},
	                    {"Perception", 0},
	                    {"Charisma", 1.0F},
	                    {"Fortitude", 0},
	                    {"Agility", 0},
	                    {"Allure", 0},
	                    {"Magicka", 1.0F},
	                    {"Succour", -.25F},
	                    {"Luck", 0},
                    }
                },
                {
                "SpellExtraHealthDamagePercent",
                new Dictionary<string,float> {
                      	{"Discipline", -.25F},
	                    {"Perception", 0},
	                    {"Charisma", 0.7F},
	                    {"Fortitude", 0},
	                    {"Agility", 0},
	                    {"Allure", 1.1F},
	                    {"Magicka", 0},
	                    {"Succour", 0},
	                    {"Luck", 0},
                    }
                },
                {
                "CleanseExtraTFEnergyRemovalPercent",
                new Dictionary<string,float> {
                      	{"Discipline", 0},
	                    {"Perception", 0},
	                    {"Charisma", 0},
	                    {"Fortitude", 0},
	                    {"Agility", 0},
	                    {"Allure", 0},
	                    {"Magicka", -.125F},
	                    {"Succour", 0},
	                    {"Luck", .375F},
                    }
                },
                {
                "SpellMisfireChanceReduction",
                new Dictionary<string,float> {
                      	{"Discipline", 0},
	                    {"Perception", .75F},
	                    {"Charisma", -.125F},
	                    {"Fortitude", 0},
	                    {"Agility", 0},
	                    {"Allure", 0},
	                    {"Magicka", 0},
	                    {"Succour", 0},
	                    {"Luck", 0},
                    }
                },
                {
                "SpellHealthDamageResistance",
                new Dictionary<string,float> {
                      	{"Discipline", 1.0F},
	                    {"Perception", 0},
	                    {"Charisma", -.25F},
	                    {"Fortitude", 0},
	                    {"Agility", 0},
	                    {"Allure", 0},
	                    {"Magicka", 0},
	                    {"Succour", 0},
	                    {"Luck", -.25F},
                    }
                },
                {
                "SpellTFEnergyDamageResistance",
                new Dictionary<string,float> {
                      	{"Discipline", 0},
	                    {"Perception", 0},
	                    {"Charisma", 0},
	                    {"Fortitude", 1.0F},
	                    {"Agility", -.25F},
	                    {"Allure", 0},
	                    {"Magicka", 0},
	                    {"Succour", 0},
	                    {"Luck", -.1875F},
                    }
                },
                {
                "ExtraInventorySpace",
                new Dictionary<string,float> {
                      	{"Discipline", 0},
	                    {"Perception", 0},
	                    {"Charisma", 0},
	                    {"Fortitude", .10F},
	                    {"Agility", -.025F},
	                    {"Allure", 0},
	                    {"Magicka", 0},
	                    {"Succour", 0},
	                    {"Luck", 0},
                    }
                },
            };
    };
}