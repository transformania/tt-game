using System;
using System.Collections.Generic;

namespace TT.Domain.ViewModels
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
        public float Allure { get; set; } // Renamed to Restoration
        public float Corruption_Purity { get; set; }
        public float Magicka { get; set; }
        public float Succour { get; set; } // Renamed to Regeneration
        public float Luck { get; set; }
        public float Chaos_Order { get; set; }
    }

    public class BuffDetail
    {
        public string DisplayName { get; set; }
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
        public bool FindSpellsOnly;
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

        public float ItemBuffs()
        {
            return FromItems_Discipline + FromItems_Perception + FromItems_Charisma + FromItems_Submission_Dominance +
                FromItems_Fortitude + FromItems_Agility + FromItems_Allure + FromItems_Corruption_Purity + FromItems_Magicka +
                FromItems_Succour + FromItems_Luck + FromItems_Chaos_Order;
        }

        // new system
        public decimal HealthBonusPercent()
        {

            float output = 0;
            var substat = "HealthBonusPercent"; ;

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
            output += (float)FromEffects_HealthBonusPercent;

            return Convert.ToDecimal(output);

        }

        public decimal ManaBonusPercent()
        {
            float output = 0;
            var substat = "ManaBonusPercent"; ;

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
            output += (float)FromEffects_ManaBonusPercent;

            return Convert.ToDecimal(output);

        }

        public decimal ExtraSkillCriticalPercent()
        {
            float output = 0;
            var substat = "ExtraSkillCriticalPercent"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            output += (float)FromForm_ExtraSkillCriticalPercent;
            output += (float)FromEffects_ExtraSkillCriticalPercent;

            return Convert.ToDecimal(output);
        }

        public decimal HealthRecoveryPerUpdate()
        {
            float output = 0;
            var substat = "HealthRecoveryPerUpdate"; ;

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
            var substat = "ManaRecoveryPerUpdate"; ;

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
            var substat = "SneakPercent"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            output += (float)FromForm_SneakPercent;
            output += (float)FromEffects_SneakPercent;

            return Convert.ToDecimal(output);
        }

        public decimal AntiSneakPercent()
        {
            float output = 0;
            var substat = "AntiSneakPercent"; ;

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
            var substat = "EvasionPercent"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            output += (float)FromForm_EvasionPercent;
            output += (float)FromEffects_EvasionPercent;

            return Convert.ToDecimal(output);
        }

        public decimal EvasionNegationPercent()
        {
            float output = 0;
            var substat = "EvasionNegationPercent"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            output += (float)FromForm_EvasionNegationPercent;
            output += (float)FromEffects_EvasionNegationPercent;

            return Convert.ToDecimal(output);
        }

        public decimal MeditationExtraMana()
        {
            float output = 0;
            var substat = "MeditationExtraMana"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            output += (float)FromForm_MeditationExtraMana;
            output += (float)FromEffects_MeditationExtraMana;

            return Convert.ToDecimal(output);
        }

        public decimal CleanseExtraHealth()
        {
            float output = 0;
            var substat = "CleanseExtraHealth"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            output += (float)FromForm_CleanseExtraHealth;
            output += (float)FromEffects_CleanseExtraHealth;

            return Convert.ToDecimal(output);
        }

        public decimal MoveActionPointDiscount()
        {
            float output = 0;
            var substat = "MoveActionPointDiscount"; ;

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
            var substat = "SpellExtraTFEnergyPercent"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            output += (float)FromForm_SpellExtraTFEnergyPercent;
            output += (float)FromEffects_SpellExtraTFEnergyPercent;

            return Convert.ToDecimal(output);
        }

        public decimal SpellExtraHealthDamagePercent()
        {
            float output = 0;
            var substat = "SpellExtraHealthDamagePercent"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            output += (float)FromForm_SpellExtraHealthDamagePercent;
            output += (float)FromEffects_SpellExtraHealthDamagePercent;

            return Convert.ToDecimal(output);
        }

        public decimal CleanseExtraTFEnergyRemovalPercent()
        {
            float output = 0;
            var substat = "CleanseExtraTFEnergyRemovalPercent"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            output += (float)FromForm_CleanseExtraTFEnergyRemovalPercent;
            output += (float)FromEffects_CleanseExtraTFEnergyRemovalPercent;

            return Convert.ToDecimal(output);
        }

        public decimal SpellMisfireChanceReduction()
        {
            float output = 0;
            var substat = "SpellMisfireChanceReduction"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            output += (float)FromForm_SpellMisfireChanceReduction;
            output += (float)FromEffects_SpellMisfireChanceReduction;

            return Convert.ToDecimal(output);
        }

        public decimal SpellHealthDamageResistance()
        {
            float output = 0;
            var substat = "SpellHealthDamageResistance"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            output += (float)FromForm_SpellHealthDamageResistance;
            output += (float)FromEffects_SpellHealthDamageResistance;

            return Convert.ToDecimal(output);
        }

        public decimal SpellTFEnergyDamageResistance()
        {
            float output = 0;
            var substat = "SpellTFEnergyDamageResistance"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            output += (float)FromForm_SpellTFEnergyDamageResistance;
            output += (float)FromEffects_SpellTFEnergyDamageResistance;

            return Convert.ToDecimal(output);
        }

        public int ExtraInventorySpace()
        {
            float output = 0;
            var substat = "ExtraInventorySpace"; ;

            output += Discipline() * BuffMap.BuffsMap[substat]["Discipline"];
            output += Perception() * BuffMap.BuffsMap[substat]["Perception"];
            output += Charisma() * BuffMap.BuffsMap[substat]["Charisma"];
            output += Fortitude() * BuffMap.BuffsMap[substat]["Fortitude"];
            output += Agility() * BuffMap.BuffsMap[substat]["Agility"];
            output += Allure() * BuffMap.BuffsMap[substat]["Allure"];
            output += Magicka() * BuffMap.BuffsMap[substat]["Magicka"];
            output += Succour() * BuffMap.BuffsMap[substat]["Succour"];
            output += Luck() * BuffMap.BuffsMap[substat]["Luck"];

            output += (float)FromForm_ExtraInventorySpace;
            output += (float)FromEffects_ExtraInventorySpace;

            return (int)Math.Floor(output);
        }
    }

    public static class BuffMap
    {

        private const string DefenseIconClass = "icon icon-defense";
        private const string WPUpIconClass = "icon icon-health_recovery";
        private const string ManaUpIconClass = "icon icon-mana_recovery";
        private const string RestorationIconCLass = "icon icon-cleansemeditate";
        private const string AttackIconClass = "icon icon-timesattacking";

        public static Dictionary<string, BuffDetail> BuffDetailsMap = new Dictionary<string, BuffDetail>{

            
            { 
                "Discipline", 
                new BuffDetail {
                    DisplayName = "Discipline",
                    Description = "Discipline affects the strength of your mental defenses, reducing the effectiveness of all spells cast against you, while also giving you more will to resist those spells.", 
                    PlusIcons = new List<string> { DefenseIconClass, WPUpIconClass }, 
                    MinusIcons =  new  List<string> { }
                }
            },
            { 
                "Perception", 
                new BuffDetail {
                    DisplayName = "Perception",
                    Description = "Perception affects the accuracy of your spell-casting and your ability to detect hidden things.  It is also quite useful to have so you know when a spell cast is starting off poorly, reducing the chance of it exploding in your own face.", 
                    PlusIcons = new List<string> { AttackIconClass }, 
                    MinusIcons =  new  List<string> { }
                }
            },
            { 
                "Charisma", 
                new BuffDetail {
                    DisplayName = "Charisma",
                    Description = "Charisma affects the strength of your willpower and transformative attacks; it represents your ability to influence people and bend them to your design.", 
                    PlusIcons = new List<string> { AttackIconClass }, 
                    MinusIcons =  new  List<string> { }
                }
            },
            { 
                "Fortitude", 
                new BuffDetail {
                    DisplayName = "Fortitude",
                    Description = "Although brute force is generally looked down on in the wizarding world, having some extra strength allows you to carry more posessions at a time, plus the extra bulk of your body will make your opponent expend a little more effort to transform.", 
                    PlusIcons = new List<string> { DefenseIconClass, WPUpIconClass }, 
                    MinusIcons =  new  List<string> { }
                }
            },
            { 
                "Agility", 
                new BuffDetail {
                    DisplayName = "Agility",
                    Description = "Agility affects your ability to move quickly, which helps you dodge attacks and move more quickly while making less noise.", 
                    PlusIcons = new List<string> { DefenseIconClass }, 
                    MinusIcons =  new  List<string> { }
                }
            },
            { 
                "Allure", 
                new BuffDetail {
                    DisplayName = "Restoration",
                    Description = "Restoration increases the amount of willpower and mana you recover when cleansing or meditating, as well as purge a bit of extra transformation energies from your body.", 
                    PlusIcons = new List<string> { RestorationIconCLass }, 
                    MinusIcons =  new  List<string> { }
                }
            },
            { 
                "Magicka", 
                new BuffDetail {
                    DisplayName = "Magicka",
                    Description = "Magicka reflects your raw magic potential; the stronger your connection to the ambient magical fields around you, the more mana you have at your disposal. You will also find that your increased magical prowess allows you greater control over your opponent&apos;s bodies.", 
                    PlusIcons = new List<string> { ManaUpIconClass, AttackIconClass }, 
                    MinusIcons =  new  List<string> { }
                }
            },
            { 
                "Succour", 
                new BuffDetail {
                    DisplayName = "Regeneration",
                    Description = "Regeneration affects your ability to tap into ambient magical energies and manipulate them to recover mana and willpower without even having to think about it, leaving your time and energy better spent on other matters like how best to avoid being turned into panties.", 
                    PlusIcons = new List<string> { RestorationIconCLass }, 
                    MinusIcons =  new  List<string> { }
                }
            },
            { 
                "Luck", 
                new BuffDetail {
                    DisplayName = "Luck",
                    Description = "Luck is random chance seeming to operate in your favor. Or, maybe the gods look kindly upon you. However you choose to rationalize it, luck affects your chance to score amazingly successful blows against your opponents and helps to slightly reduce bad luck of your own in the form of reduced misfires.", 
                    PlusIcons = new List<string> { AttackIconClass }, 
                    MinusIcons =  new  List<string> { }
                }
            },
        };

  

        public static Dictionary<string, Dictionary<string, float>> BuffsMap = new Dictionary<string, Dictionary<string, float>> {



            {
                "HealthBonusPercent",
                new Dictionary<string,float> {
                      	{"Discipline", 0.5F},
	                    {"Perception", 0},
	                    {"Charisma", 0},
	                    {"Fortitude", 1F},
	                    {"Agility", 0},
	                    {"Allure", 0},
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
	                    {"Fortitude", 0F},
	                    {"Agility", 0},
	                    {"Allure", 0},
	                    {"Magicka", 0.75F},
	                    {"Succour", 0},
	                    {"Luck", 0},
                    }
                },
                {
                "ExtraSkillCriticalPercent",
                new Dictionary<string,float> {
                      	{"Discipline", 0},
	                    {"Perception", 0},
	                    {"Charisma", 0},
	                    {"Fortitude", 0},
	                    {"Agility", 0.075F},
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
	                    {"Succour", 0.9F}, // regeneration
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
	                    {"Succour",0.26F}, // regeneration
	                    {"Luck", 0},
                    }
                },
                {
                "SneakPercent",
                new Dictionary<string,float> {
                      	{"Discipline", 0},
	                    {"Perception", 0},
	                    {"Charisma", 0},
	                    {"Fortitude", 0},
	                    {"Agility", 0.25F},
	                    {"Allure", 0},
	                    {"Magicka", 0},
	                    {"Succour", 0},
	                    {"Luck", 0},
                    }
                },
                {
                "AntiSneakPercent",
                new Dictionary<string,float> {
                      	{"Discipline", 0},
	                    {"Perception", 0.5F},
	                    {"Charisma", 0},
	                    {"Fortitude", 0},
	                    {"Agility", 0},
	                    {"Allure", 0},
	                    {"Magicka", 0},
	                    {"Succour", 0},
	                    {"Luck", 0},
                    }
                },
                {
                "EvasionPercent",
                new Dictionary<string,float> {
                      	{"Discipline", -0.1F},
	                    {"Perception", 0},
	                    {"Charisma", 0},
	                    {"Fortitude", 0},
	                    {"Agility", 0.30F},
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
	                    {"Perception", 0.15F},
	                    {"Charisma", 0},
	                    {"Fortitude", 0},
	                    {"Agility", 0},
	                    {"Allure", 0},
	                    {"Magicka", 0},
	                    {"Succour", 0},
	                    {"Luck", 0},
                    }
                },
                {
                "MeditationExtraMana",
                new Dictionary<string,float> {
                      	{"Discipline", 0},
	                    {"Perception", 0},
	                    {"Charisma", 0},
	                    {"Fortitude", 0},
	                    {"Agility", 0},
	                    {"Allure", .15F}, // restoration
	                    {"Magicka", 0},
	                    {"Succour", 0}, 
                        {"Luck", 0},
                    }
                },
                {
                "CleanseExtraHealth",
                new Dictionary<string,float> {
                      	{"Discipline", 0},
	                    {"Perception", 0},
	                    {"Charisma", 0},
	                    {"Fortitude", 0},
	                    {"Agility", 0},
	                    {"Allure", .5F}, // restoration
	                    {"Magicka", 0},
	                    {"Succour", 0}, 
	                    {"Luck", 0},
                    }
                },
                {
                "MoveActionPointDiscount",
                new Dictionary<string,float> {
                      	{"Discipline", 0},
	                    {"Perception", 0},
	                    {"Charisma", 0},
	                    {"Fortitude", 0},
	                    {"Agility", .0025F},
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
	                    {"Perception", 0.5F},
	                    {"Charisma", 0F},
	                    {"Fortitude", 0},
	                    {"Agility", 0},
	                    {"Allure", 0},
	                    {"Magicka", 1F},
	                    {"Succour", 0},
	                    {"Luck", 0},
                    }
                },
                {
                "SpellExtraHealthDamagePercent",
                new Dictionary<string,float> {
                      	{"Discipline", 0},
	                    {"Perception", 0.375F},
	                    {"Charisma", .75F},
	                    {"Fortitude", 0},
	                    {"Agility", 0},
	                    {"Allure", 0},
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
	                    {"Allure", .02F}, // restoration
	                    {"Magicka", 0},
	                    {"Succour", 0},
	                    {"Luck", 0},
                    }
                },
                {
                "SpellMisfireChanceReduction",
                new Dictionary<string,float> {
                      	{"Discipline", 0.08F},
	                    {"Perception", 0.08F},
	                    {"Charisma", 0},
	                    {"Fortitude", 0},
	                    {"Agility", 0},
	                    {"Allure", 0}, // restoration
	                    {"Magicka", 0},
	                    {"Succour", 0},
	                    {"Luck", 0.08F},
                    }
                },
                {
                "SpellHealthDamageResistance",
                new Dictionary<string,float> {
                      	{"Discipline", 0.5F},
	                    {"Perception", 0},
	                    {"Charisma", 0},
	                    {"Fortitude", 0F},
	                    {"Agility", 0},
	                    {"Allure", 0},
	                    {"Magicka", 0},
	                    {"Succour", 0},
	                    {"Luck", 0},
                    }
                },
                {
                "SpellTFEnergyDamageResistance",
                new Dictionary<string,float> {
                      	{"Discipline", 0.25F},
	                    {"Perception", 0},
	                    {"Charisma", 0},
	                    {"Fortitude", 0F},
	                    {"Agility", 0},
	                    {"Allure", 0},
	                    {"Magicka", 0},
	                    {"Succour", 0},
	                    {"Luck", 0},
                    }
                },
                {
                "ExtraInventorySpace",
                new Dictionary<string,float> {
                      	{"Discipline", 0},
	                    {"Perception", 0},
	                    {"Charisma", 0},
	                    {"Fortitude", .04F},
	                    {"Agility", 0},
	                    {"Allure", 0},
	                    {"Magicka", 0},
	                    {"Succour", 0},
	                    {"Luck", 0},
                    }
                },
            };
    };
}