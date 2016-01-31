using System;
using System.Collections.Generic;
using TT.Domain.Models;

//public decimal FromItems_HealthBonusPercent { get; set; }
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

namespace TT.Domain.Statics
{
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
}

namespace TT.Domain.ViewModels
{
    public class BalanceBox
    {
        
        List<BuffStat> BuffStats { get; set; }

        public decimal GetBalance() {

            decimal total = 0;

            foreach (BuffStat buff in this.BuffStats) {

                decimal value = buff.Amount * buff.Value;

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

            foreach (BuffStat buff in this.BuffStats)
            {
                decimal value = buff.Amount * buff.Value;
                total += value;
            }
            return total;
        }

        public decimal GetPointTotal()
        {
            decimal total = 0;

            foreach (BuffStat buff in this.BuffStats)
            {

                decimal value = buff.Amount * buff.Value;

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
            BuffStat addme = new BuffStat();

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
            BuffStat addme = new BuffStat();

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
            BuffStat addme = new BuffStat();

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
            BuffStat addme = new BuffStat();

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
            BuffStat addme = new BuffStat();
            if (input.HealthBonusPercent != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.HealthBonusPercent,
                    Value = Statics.BalanceStatics.HealthBonusPercent__Value,
                    NegativeModifier = Statics.BalanceStatics.HealthBonusPercent__NegativeModifier,
                    NegativeCap = Statics.BalanceStatics.HealthBonusPercent__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }
            if (input.ManaBonusPercent != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.ManaBonusPercent,
                    Value = Statics.BalanceStatics.ManaBonusPercent__Value,
                    NegativeModifier = Statics.BalanceStatics.ManaBonusPercent__NegativeModifier,
                    NegativeCap = Statics.BalanceStatics.ManaBonusPercent__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }
            if (input.ExtraSkillCriticalPercent != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.ExtraSkillCriticalPercent,
                    Value = Statics.BalanceStatics.ExtraSkillCriticalPercent__Value,
                    NegativeModifier = Statics.BalanceStatics.ExtraSkillCriticalPercent__NegativeModifier,
                    NegativeCap = Statics.BalanceStatics.ExtraSkillCriticalPercent__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }
            if (input.HealthRecoveryPerUpdate != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.HealthRecoveryPerUpdate,
                    Value = Statics.BalanceStatics.HealthRecoveryPerUpdate__Value,
                    NegativeModifier = Statics.BalanceStatics.HealthRecoveryPerUpdate__NegativeModifier,
                    NegativeCap = Statics.BalanceStatics.HealthRecoveryPerUpdate__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }
            if (input.ManaRecoveryPerUpdate != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.ManaRecoveryPerUpdate,
                    Value = Statics.BalanceStatics.ManaRecoveryPerUpdate__Value,
                    NegativeModifier = Statics.BalanceStatics.ManaRecoveryPerUpdate__NegativeModifier,
                    NegativeCap = Statics.BalanceStatics.ManaRecoveryPerUpdate__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }
            if (input.SneakPercent != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.SneakPercent,
                    Value = Statics.BalanceStatics.SneakPercent__Value,
                    NegativeModifier = Statics.BalanceStatics.SneakPercent__NegativeModifier,
                    NegativeCap = Statics.BalanceStatics.SneakPercent__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }
            if (input.EvasionPercent != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.EvasionPercent,
                    Value = Statics.BalanceStatics.EvasionPercent__Value,
                    NegativeModifier = Statics.BalanceStatics.EvasionPercent__NegativeModifier,
                    NegativeCap = Statics.BalanceStatics.EvasionPercent__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }
            if (input.EvasionNegationPercent != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.EvasionNegationPercent,
                    Value = Statics.BalanceStatics.EvasionNegationPercent__Value,
                    NegativeModifier = Statics.BalanceStatics.EvasionNegationPercent__NegativeModifier,
                    NegativeCap = Statics.BalanceStatics.EvasionNegationPercent__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }
            if (input.MeditationExtraMana != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.MeditationExtraMana,
                    Value = Statics.BalanceStatics.MeditationExtraMana__Value,
                    NegativeModifier = Statics.BalanceStatics.MeditationExtraMana__NegativeModifier,
                    NegativeCap = Statics.BalanceStatics.MeditationExtraMana__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }
            if (input.CleanseExtraHealth != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.CleanseExtraHealth,
                    Value = Statics.BalanceStatics.CleanseExtraHealth__Value,
                    NegativeModifier = Statics.BalanceStatics.CleanseExtraHealth__NegativeModifier,
                    NegativeCap = Statics.BalanceStatics.CleanseExtraHealth__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }
            if (input.MoveActionPointDiscount != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.MoveActionPointDiscount,
                    Value = Statics.BalanceStatics.MoveActionPointDiscount__Value,
                    NegativeModifier = Statics.BalanceStatics.MoveActionPointDiscount__NegativeModifier,
                    NegativeCap = Statics.BalanceStatics.MoveActionPointDiscount__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }
            if (input.SpellExtraTFEnergyPercent != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.SpellExtraTFEnergyPercent,
                    Value = Statics.BalanceStatics.SpellExtraTFEnergyPercent__Value,
                    NegativeModifier = Statics.BalanceStatics.SpellExtraTFEnergyPercent__NegativeModifier,
                    NegativeCap = Statics.BalanceStatics.SpellExtraTFEnergyPercent__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }
            if (input.SpellExtraHealthDamagePercent != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.SpellExtraHealthDamagePercent,
                    Value = Statics.BalanceStatics.SpellExtraHealthDamagePercent__Value,
                    NegativeModifier = Statics.BalanceStatics.SpellExtraHealthDamagePercent__NegativeModifier,
                    NegativeCap = Statics.BalanceStatics.SpellExtraHealthDamagePercent__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }
            if (input.CleanseExtraTFEnergyRemovalPercent != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.CleanseExtraTFEnergyRemovalPercent,
                    Value = Statics.BalanceStatics.CleanseExtraTFEnergyRemovalPercent__Value,
                    NegativeModifier = Statics.BalanceStatics.CleanseExtraTFEnergyRemovalPercent__NegativeModifier,
                    NegativeCap = Statics.BalanceStatics.CleanseExtraTFEnergyRemovalPercent__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }
            if (input.SpellMisfireChanceReduction != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.SpellMisfireChanceReduction,
                    Value = Statics.BalanceStatics.SpellMisfireChanceReduction__Value,
                    NegativeModifier = Statics.BalanceStatics.SpellMisfireChanceReduction__NegativeModifier,
                    NegativeCap = Statics.BalanceStatics.SpellMisfireChanceReduction__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }

            if (input.SpellHealthDamageResistance != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.SpellHealthDamageResistance,
                    Value = Statics.BalanceStatics.SpellHealthDamageResistance__Value,
                    NegativeModifier = Statics.BalanceStatics.SpellHealthDamageResistance__NegativeModifier,
                    NegativeCap = Statics.BalanceStatics.SpellHealthDamageResistance__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }

            if (input.SpellTFEnergyDamageResistance != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.SpellTFEnergyDamageResistance,
                    Value = Statics.BalanceStatics.SpellTFEnergyDamageResistance__Value,
                    NegativeModifier = Statics.BalanceStatics.SpellTFEnergyDamageResistance__NegativeModifier,
                    NegativeCap = Statics.BalanceStatics.SpellTFEnergyDamageResistance__NegativeCap,
                };
                this.BuffStats.Add(addme);
            }

            if (input.ExtraInventorySpace != 0)
            {
                addme = new BuffStat
                {
                    Amount = input.ExtraInventorySpace,
                    Value = Statics.BalanceStatics.ExtraInventorySpace__Value,
                    NegativeModifier = Statics.BalanceStatics.ExtraInventorySpace__NegativeModifier,
                    NegativeCap = Statics.BalanceStatics.ExtraInventorySpace__NegativeCap,
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