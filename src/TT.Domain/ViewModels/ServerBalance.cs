using System;
using System.Collections.Generic;
using TT.Domain.Models;

namespace TT.Domain.ViewModels
{ //public decimal FromItems_HealthBonusPercent { get; set; }
//     public decimal FromItems_ManaBonusPercent { get; set; }
//     public decimal FromItems_ExtraSkillCriticalPercent { get; set; }
//     public decimal FromItems_HealthRecoveryPerUpdate { get; set; }
//     public decimal FromItems_ManaRecoveryPerUpdate { get; set; }
//     public decimal FromItems_SneakPercent { get; set; }
//     public decimal FromItems_EvasionPercent { get; set; }
//     public decimal FromItems_EvasionNegationPercent { get; set; }
//     public decimal FromItems_MeditationExtraMana { get; set; }
//     public decimal FromItems_CleanseExtraHealth { get; set; }
//     public decimal FromItems_MoveActionPointDiscount { get; set; }
//     public decimal FromItems_SpellExtraTFEnergyPercent { get; set; }
//     public decimal FromItems_SpellExtraHealthDamagePercent { get; set; }
//     public decimal FromItems_CleanseExtraTFEnergyRemovalPercent { get; set; }
//     public decimal FromItems_SpellMisfireChanceReduction { get; set; }
//     public decimal FromItems_SpellHealthDamageResistance { get; set; }
//     public decimal FromItems_SpellTFEnergyDamageResistance { get; set; }
//     public decimal FromItems_ExtraInventorySpace { get; set; }

    public static class BalanceStatics
    {
        public const decimal HealthBonusPercent__Value = 5;
        public const decimal HealthBonusPercent__NegativeModifier = 0;
        public const decimal HealthBonusPercent__NegativeCap = -99999;

        public const decimal ManaBonusPercent__Value = 2.5M;
        public const decimal ManaBonusPercent__NegativeModifier = .5M;
        public const decimal ManaBonusPercent__NegativeCap = -99999;

        public const decimal ExtraSkillCriticalPercent__Value = 16;
        public const decimal ExtraSkillCriticalPercent__NegativeModifier = 0;
        public const decimal ExtraSkillCriticalPercent__NegativeCap = -32;

        public const decimal HealthRecoveryPerUpdate__Value = 12;
        public const decimal HealthRecoveryPerUpdate__NegativeModifier = 0;
        public const decimal HealthRecoveryPerUpdate__NegativeCap = -60;

        public const decimal ManaRecoveryPerUpdate__Value = 12;
        public const decimal ManaRecoveryPerUpdate__NegativeModifier = 0;
        public const decimal ManaRecoveryPerUpdate__NegativeCap = -60;

        public const decimal SneakPercent__Value = 4;
        public const decimal SneakPercent__NegativeModifier = .5M;
        public const decimal SneakPercent__NegativeCap = -15;

        public const decimal EvasionPercent__Value = 7;
        public const decimal EvasionPercent__NegativeModifier = .5M;
        public const decimal EvasionPercent__NegativeCap = -21;

        public const decimal EvasionNegationPercent__Value = 4;
        public const decimal EvasionNegationPercent__NegativeModifier = .5M;
        public const decimal EvasionNegationPercent__NegativeCap = -99999;

        public const decimal MeditationExtraMana__Value = 9;
        public const decimal MeditationExtraMana__NegativeModifier = 0;
        public const decimal MeditationExtraMana__NegativeCap = -99999;

        public const decimal CleanseExtraHealth__Value = 25;
        public const decimal CleanseExtraHealth__NegativeModifier = .5M;
        public const decimal CleanseExtraHealth__NegativeCap = -99999;

        public const decimal MoveActionPointDiscount__Value = 250;
        public const decimal MoveActionPointDiscount__NegativeModifier = .7M;
        public const decimal MoveActionPointDiscount__NegativeCap = -50;

        public const decimal SpellExtraTFEnergyPercent__Value = 7;
        public const decimal SpellExtraTFEnergyPercent__NegativeModifier = 0;
        public const decimal SpellExtraTFEnergyPercent__NegativeCap = -70;

        public const decimal SpellExtraHealthDamagePercent__Value = 7;
        public const decimal SpellExtraHealthDamagePercent__NegativeModifier = 0;
        public const decimal SpellExtraHealthDamagePercent__NegativeCap = -70;

        public const decimal CleanseExtraTFEnergyRemovalPercent__Value = 20;
        public const decimal CleanseExtraTFEnergyRemovalPercent__NegativeModifier = 0;
        public const decimal CleanseExtraTFEnergyRemovalPercent__NegativeCap = -20;

        public const decimal SpellMisfireChanceReduction__Value = 10;
        public const decimal SpellMisfireChanceReduction__NegativeModifier = .75M;
        public const decimal SpellMisfireChanceReduction__NegativeCap = -40;

        public const decimal SpellHealthDamageResistance__Value = 9;
        public const decimal SpellHealthDamageResistance__NegativeModifier = 0;
        public const decimal SpellHealthDamageResistance__NegativeCap = -99999;

        public const decimal SpellTFEnergyDamageResistance__Value = 9;
        public const decimal SpellTFEnergyDamageResistance__NegativeModifier = 0;
        public const decimal SpellTFEnergyDamageResistance__NegativeCap = -99999;

        public const decimal ExtraInventorySpace__Value = 40;
        public const decimal ExtraInventorySpace__NegativeModifier = 0;
        public const decimal ExtraInventorySpace__NegativeCap = -80;

    }

    public class BalanceBox
    {
        
        List<BuffStat> BuffStats { get; set; }

        public decimal GetBalance() {

            decimal total = 0;

            foreach (var buff in this.BuffStats) {

                var value = buff.Amount * buff.Value;

                if (value < 0 && buff.NegativeModifier != 0) {
                    value = value*buff.NegativeModifier;
                }

                if (value < buff.NegativeCap)
                {
                    value = buff.NegativeCap;
                }

                total += value;
            }
            return total;
        }

        public decimal GetBalance__NoModifiersOrCaps()
        {

            decimal total = 0;

            foreach (var buff in this.BuffStats)
            {
                var value = buff.Amount * buff.Value;
                total += value;
            }
            return total;
        }

        public decimal GetPointTotal()
        {
            decimal total = 0;

            foreach (var buff in this.BuffStats)
            {

                var value = buff.Amount * buff.Value;

                if (value < 0 && buff.NegativeModifier != 0)
                {
                    value = value * buff.NegativeModifier;
                }

                if (value < buff.NegativeCap)
                {
                    value = buff.NegativeCap;
                }

                value = Math.Abs(value);

                total += value;
            }
            return total;
        }

        public void LoadBalanceBox(DbStaticForm input)
        {
            BuffStats = new List<BuffStat>();
            var addme = new BuffStat();

            if (input.Discipline != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Discipline),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Perception != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Perception),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Charisma != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Charisma),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Fortitude != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Fortitude),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Agility != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Agility),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Allure != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Allure),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Magicka != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Magicka),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Succour != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Succour),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Luck != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Luck),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

        }

        public void LoadBalanceBox(DbStaticItem input)
        {
            BuffStats = new List<BuffStat>();
            var addme = new BuffStat();

            if (input.Discipline != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Discipline),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Perception != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Perception),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Charisma != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Charisma),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Fortitude != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Fortitude),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Agility != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Agility),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Allure != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Allure),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Magicka != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Magicka),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Succour != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Succour),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Luck != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Luck),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }
        }

        public void LoadBalanceBox(Contribution input)
        {
            BuffStats = new List<BuffStat>();
            var addme = new BuffStat();

            if (input.Discipline != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Discipline),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Perception != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Perception),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Charisma != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Charisma),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Fortitude != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Fortitude),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Agility != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Agility),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Allure != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Allure),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Magicka != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Magicka),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Succour != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Succour),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Luck != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Luck),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }
        }

        public void LoadBalanceBox(EffectContribution input)
        {
            BuffStats = new List<BuffStat>();
            var addme = new BuffStat();

            if (input.Discipline != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Discipline),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Perception != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Perception),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Charisma != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Charisma),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Fortitude != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Fortitude),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Agility != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Agility),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Allure != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Allure),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Magicka != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Magicka),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Succour != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Succour),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }

            if (input.Luck != 0)
            {
                addme = new BuffStat
                {
                    Amount = Convert.ToDecimal(input.Luck),
                    Value = 1,
                    NegativeModifier = 0,
                    NegativeCap = -99999,
                };
                this.BuffStats.Add(addme);
            }
        }

        public void LoadBalanceBox(DbStaticEffect input)
        {
            BuffStats = new List<BuffStat>();
            var addme = new BuffStat();
            if (input.HealthBonusPercent != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.HealthBonusPercent,
                    Value = BalanceStatics.HealthBonusPercent__Value,
                    NegativeModifier = BalanceStatics.HealthBonusPercent__NegativeModifier,
                    NegativeCap = BalanceStatics.HealthBonusPercent__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }
            if (input.ManaBonusPercent != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.ManaBonusPercent,
                    Value = BalanceStatics.ManaBonusPercent__Value,
                    NegativeModifier = BalanceStatics.ManaBonusPercent__NegativeModifier,
                    NegativeCap = BalanceStatics.ManaBonusPercent__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }
            if (input.ExtraSkillCriticalPercent != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.ExtraSkillCriticalPercent,
                    Value = BalanceStatics.ExtraSkillCriticalPercent__Value,
                    NegativeModifier = BalanceStatics.ExtraSkillCriticalPercent__NegativeModifier,
                    NegativeCap = BalanceStatics.ExtraSkillCriticalPercent__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }
            if (input.HealthRecoveryPerUpdate != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.HealthRecoveryPerUpdate,
                    Value = BalanceStatics.HealthRecoveryPerUpdate__Value,
                    NegativeModifier = BalanceStatics.HealthRecoveryPerUpdate__NegativeModifier,
                    NegativeCap = BalanceStatics.HealthRecoveryPerUpdate__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }
            if (input.ManaRecoveryPerUpdate != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.ManaRecoveryPerUpdate,
                    Value = BalanceStatics.ManaRecoveryPerUpdate__Value,
                    NegativeModifier = BalanceStatics.ManaRecoveryPerUpdate__NegativeModifier,
                    NegativeCap = BalanceStatics.ManaRecoveryPerUpdate__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }
            if (input.SneakPercent != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.SneakPercent,
                    Value = BalanceStatics.SneakPercent__Value,
                    NegativeModifier = BalanceStatics.SneakPercent__NegativeModifier,
                    NegativeCap = BalanceStatics.SneakPercent__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }
            if (input.EvasionPercent != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.EvasionPercent,
                    Value = BalanceStatics.EvasionPercent__Value,
                    NegativeModifier = BalanceStatics.EvasionPercent__NegativeModifier,
                    NegativeCap = BalanceStatics.EvasionPercent__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }
            if (input.EvasionNegationPercent != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.EvasionNegationPercent,
                    Value = BalanceStatics.EvasionNegationPercent__Value,
                    NegativeModifier = BalanceStatics.EvasionNegationPercent__NegativeModifier,
                    NegativeCap = BalanceStatics.EvasionNegationPercent__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }
            if (input.MeditationExtraMana != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.MeditationExtraMana,
                    Value = BalanceStatics.MeditationExtraMana__Value,
                    NegativeModifier = BalanceStatics.MeditationExtraMana__NegativeModifier,
                    NegativeCap = BalanceStatics.MeditationExtraMana__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }
            if (input.CleanseExtraHealth != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.CleanseExtraHealth,
                    Value = BalanceStatics.CleanseExtraHealth__Value,
                    NegativeModifier = BalanceStatics.CleanseExtraHealth__NegativeModifier,
                    NegativeCap = BalanceStatics.CleanseExtraHealth__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }
            if (input.MoveActionPointDiscount != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.MoveActionPointDiscount,
                    Value = BalanceStatics.MoveActionPointDiscount__Value,
                    NegativeModifier = BalanceStatics.MoveActionPointDiscount__NegativeModifier,
                    NegativeCap = BalanceStatics.MoveActionPointDiscount__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }
            if (input.SpellExtraTFEnergyPercent != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.SpellExtraTFEnergyPercent,
                    Value = BalanceStatics.SpellExtraTFEnergyPercent__Value,
                    NegativeModifier = BalanceStatics.SpellExtraTFEnergyPercent__NegativeModifier,
                    NegativeCap = BalanceStatics.SpellExtraTFEnergyPercent__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }
            if (input.SpellExtraHealthDamagePercent != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.SpellExtraHealthDamagePercent,
                    Value = BalanceStatics.SpellExtraHealthDamagePercent__Value,
                    NegativeModifier = BalanceStatics.SpellExtraHealthDamagePercent__NegativeModifier,
                    NegativeCap = BalanceStatics.SpellExtraHealthDamagePercent__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }
            if (input.CleanseExtraTFEnergyRemovalPercent != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.CleanseExtraTFEnergyRemovalPercent,
                    Value = BalanceStatics.CleanseExtraTFEnergyRemovalPercent__Value,
                    NegativeModifier = BalanceStatics.CleanseExtraTFEnergyRemovalPercent__NegativeModifier,
                    NegativeCap = BalanceStatics.CleanseExtraTFEnergyRemovalPercent__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }
            if (input.SpellMisfireChanceReduction != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.SpellMisfireChanceReduction,
                    Value = BalanceStatics.SpellMisfireChanceReduction__Value,
                    NegativeModifier = BalanceStatics.SpellMisfireChanceReduction__NegativeModifier,
                    NegativeCap = BalanceStatics.SpellMisfireChanceReduction__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }

            if (input.SpellHealthDamageResistance != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.SpellHealthDamageResistance,
                    Value = BalanceStatics.SpellHealthDamageResistance__Value,
                    NegativeModifier = BalanceStatics.SpellHealthDamageResistance__NegativeModifier,
                    NegativeCap = BalanceStatics.SpellHealthDamageResistance__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }

            if (input.SpellTFEnergyDamageResistance != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.SpellTFEnergyDamageResistance,
                    Value = BalanceStatics.SpellTFEnergyDamageResistance__Value,
                    NegativeModifier = BalanceStatics.SpellTFEnergyDamageResistance__NegativeModifier,
                    NegativeCap = BalanceStatics.SpellTFEnergyDamageResistance__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }

            if (input.ExtraInventorySpace != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.ExtraInventorySpace,
                    Value = BalanceStatics.ExtraInventorySpace__Value,
                    NegativeModifier = BalanceStatics.ExtraInventorySpace__NegativeModifier,
                    NegativeCap = BalanceStatics.ExtraInventorySpace__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }
        }


    }

    public class BuffStat
    {
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public decimal Value { get; set; }
        public decimal NegativeModifier { get; set; }
        public decimal NegativeCap { get; set; }

        //public BuffStat(string type, decimal amount)
        //{
        //    this.Type = type;
        //    this.Amount = amount;

        //   // if (type == "")
        //}
    }
}