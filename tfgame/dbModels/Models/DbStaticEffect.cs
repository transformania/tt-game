using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.ViewModels;

namespace tfgame.dbModels.Models
{
    public class DbStaticEffect
    {
        public int Id { get; set; }

        public string dbName { get; set; }
        public string FriendlyName { get; set; }
        public string Description { get; set; }
        public int AvailableAtLevel { get; set; }
        public string PreRequesite { get; set; }

        public bool isLevelUpPerk { get; set; }
        public int Duration { get; set; }
        public int Cooldown { get; set; }

        public string ObtainedAtLocation { get; set; }

        public bool IsRemovable { get; set; }

        public string MessageWhenHit { get; set; }
        public string MessageWhenHit_M { get; set; }
        public string MessageWhenHit_F { get; set; }

        public string AttackerWhenHit { get; set; }
        public string AttackerWhenHit_M { get; set; }
        public string AttackerWhenHit_F { get; set; }

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

        public StaticEffect ToStaticEffect()
        {
            StaticEffect output = new StaticEffect
            {
                dbName = this.dbName,
                FriendlyName = this.FriendlyName,
                Description = this.Description,
                AvailableAtLevel = this.AvailableAtLevel,
                PreRequesite = this.PreRequesite,
                isLevelUpPerk = this.isLevelUpPerk,
                Duration = this.Duration,
                Cooldown = this.Cooldown,
                ObtainedAtLocation = this.ObtainedAtLocation,
                MessageWhenHit = this.MessageWhenHit,
                MessageWhenHit_M = this.MessageWhenHit_M,
                MessageWhenHit_F = this.MessageWhenHit_F,
                AttackerWhenHit = this.AttackerWhenHit,
                AttackerWhenHit_M = this.AttackerWhenHit_M,
                AttackerWhenHit_F = this.AttackerWhenHit_F,

                HealthBonusPercent = this.HealthBonusPercent,
                ManaBonusPercent = this.ManaBonusPercent,
                ExtraSkillCriticalPercent = this.ExtraSkillCriticalPercent,
                HealthRecoveryPerUpdate = this.HealthRecoveryPerUpdate,
                ManaRecoveryPerUpdate = this.ManaRecoveryPerUpdate,
                SneakPercent = this.SneakPercent,
                EvasionPercent = this.EvasionPercent,
                EvasionNegationPercent = this.EvasionNegationPercent,
                MeditationExtraMana = this.MeditationExtraMana,
                CleanseExtraHealth = this.CleanseExtraHealth,
                MoveActionPointDiscount = this.MoveActionPointDiscount,
                SpellExtraTFEnergyPercent = this.SpellExtraTFEnergyPercent,
                SpellExtraHealthDamagePercent = this.SpellExtraHealthDamagePercent,
                CleanseExtraTFEnergyRemovalPercent = this.CleanseExtraTFEnergyRemovalPercent,
                SpellMisfireChanceReduction = this.SpellMisfireChanceReduction,
                SpellHealthDamageResistance = this.SpellHealthDamageResistance,
                SpellTFEnergyDamageResistance = this.SpellTFEnergyDamageResistance,
                ExtraInventorySpace = this.ExtraInventorySpace,

                

        //                public float Discipline { get; set; }
        //public float Perception { get; set; }
        //public float Charisma { get; set; }
        //public float Submission_Dominance { get; set; }

        //public float Fortitude { get; set; }
        //public float Agility { get; set; }
        //public float Allure { get; set; }
        //public float Corruption_Purity { get; set; }

        //public float Magicka { get; set; }
        //public float Succour { get; set; }
        //public float Luck { get; set; }
        //public float Chaos_Order { get; set; }

            };

            return output;
        }


    }
}