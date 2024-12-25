using System;
using System.Data.Entity;
using Highway.Data;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Entities;
using TT.Domain.Items.Queries.Leaderboard.DTOs;

namespace TT.Domain.Items.Mappings
{
    public class ItemSourceMappings : IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ItemSource>()
                .ToTable("DbStaticItems")
                .HasKey(cr => cr.Id);

            modelBuilder.Entity<ItemSource>()
                .HasOptional(cr => cr.GivesEffectSource)
                .WithMany().Map(m => m.MapKey("GivesEffectSourceId"));
        }
    }

    public static class ItemSourceMappingExtensions
    {
        public static ItemSourceListingDetail MapToSourceListingDto(this ItemSource itemSource)
        {
            if (itemSource == null) throw new ArgumentNullException(nameof(itemSource));

            return new ItemSourceListingDetail
            {
                Id = itemSource.Id,
                FriendlyName = itemSource.FriendlyName,
                Description = itemSource.Description,
                PortraitUrl = itemSource.PortraitUrl,
                MoneyValue = itemSource.MoneyValue,
                ItemType = itemSource.ItemType,
                UseCooldown = itemSource.UseCooldown,
                HealthBonusPercent = itemSource.HealthBonusPercent,
                ManaBonusPercent = itemSource.ManaBonusPercent,
                ExtraSkillCriticalPercent = itemSource.ExtraSkillCriticalPercent,
                HealthRecoveryPerUpdate = itemSource.HealthRecoveryPerUpdate,
                ManaRecoveryPerUpdate = itemSource.ManaRecoveryPerUpdate,
                SneakPercent = itemSource.SneakPercent,
                EvasionPercent = itemSource.EvasionPercent,
                EvasionNegationPercent = itemSource.EvasionNegationPercent,
                MeditationExtraMana = itemSource.MeditationExtraMana,
                CleanseExtraHealth = itemSource.CleanseExtraHealth,
                MoveActionPointDiscount = itemSource.MoveActionPointDiscount,
                SpellExtraTFEnergyPercent = itemSource.SpellExtraTFEnergyPercent,
                SpellExtraHealthDamagePercent = itemSource.SpellExtraHealthDamagePercent,
                CleanseExtraTFEnergyRemovalPercent = itemSource.CleanseExtraTFEnergyRemovalPercent,
                SpellMisfireChanceReduction = itemSource.SpellMisfireChanceReduction,
                SpellHealthDamageResistance = itemSource.SpellHealthDamageResistance,
                SpellTFEnergyDamageResistance = itemSource.SpellTFEnergyDamageResistance,
                ExtraInventorySpace = itemSource.ExtraInventorySpace,
                Discipline = itemSource.Discipline,
                Perception = itemSource.Perception,
                Charisma = itemSource.Charisma,
                Fortitude = itemSource.Fortitude,
                Agility = itemSource.Agility,
                Allure = itemSource.Allure,
                Magicka = itemSource.Magicka,
                Succour = itemSource.Succour,
                Luck = itemSource.Luck,
                InstantHealthRestore = itemSource.InstantHealthRestore,
                InstantManaRestore = itemSource.InstantManaRestore,
                ReuseableHealthRestore = itemSource.ReuseableHealthRestore,
                ReuseableManaRestore = itemSource.ReuseableManaRestore
            };
        }

        public static ItemSourceDetail MapToItemSourceDto(this ItemSource itemSource)
        {
            return new ItemSourceDetail
            {
                Id = itemSource.Id,
                FriendlyName = itemSource.FriendlyName,
                Description = itemSource.Description,
                PortraitUrl = itemSource.PortraitUrl,
                MoneyValue = itemSource.MoneyValue,
                MoneyValueSell = itemSource.MoneyValueSell,
                ItemType = itemSource.ItemType,
                UseCooldown = itemSource.UseCooldown,
                UsageMessage_Item = itemSource.UsageMessage_Item,
                UsageMessage_Player = itemSource.UsageMessage_Player,
                Findable = itemSource.Findable,
                FindWeight = itemSource.FindWeight,
                GivesEffectSource = itemSource.GivesEffectSource?.MapToDto(),
                IsUnique = itemSource.IsUnique,
                HealthBonusPercent = itemSource.HealthBonusPercent,
                ManaBonusPercent = itemSource.ManaBonusPercent,
                ExtraSkillCriticalPercent = itemSource.ExtraSkillCriticalPercent,
                HealthRecoveryPerUpdate = itemSource.HealthRecoveryPerUpdate,
                ManaRecoveryPerUpdate = itemSource.ManaRecoveryPerUpdate,
                SneakPercent = itemSource.SneakPercent,
                EvasionPercent = itemSource.EvasionPercent,
                EvasionNegationPercent = itemSource.EvasionNegationPercent,
                MeditationExtraMana = itemSource.MeditationExtraMana,
                CleanseExtraHealth = itemSource.CleanseExtraHealth,
                MoveActionPointDiscount = itemSource.MoveActionPointDiscount,
                SpellExtraTFEnergyPercent = itemSource.SpellExtraTFEnergyPercent,
                SpellExtraHealthDamagePercent = itemSource.SpellExtraHealthDamagePercent,
                CleanseExtraTFEnergyRemovalPercent = itemSource.CleanseExtraTFEnergyRemovalPercent,
                SpellMisfireChanceReduction = itemSource.SpellMisfireChanceReduction,
                SpellHealthDamageResistance = itemSource.SpellHealthDamageResistance,
                SpellTFEnergyDamageResistance = itemSource.SpellTFEnergyDamageResistance,
                ExtraInventorySpace = itemSource.ExtraInventorySpace,

                Discipline = itemSource.Discipline,
                Perception = itemSource.Perception,
                Charisma = itemSource.Charisma,
                Submission_Dominance = itemSource.Submission_Dominance,

                Fortitude = itemSource.Fortitude,
                Agility = itemSource.Agility,
                Allure = itemSource.Allure,
                Corruption_Purity = itemSource.Corruption_Purity,

                Magicka = itemSource.Magicka,
                Succour = itemSource.Succour,
                Luck = itemSource.Luck,
                Chaos_Order = itemSource.Chaos_Order,

                InstantHealthRestore = itemSource.InstantHealthRestore,
                InstantManaRestore = itemSource.InstantManaRestore,
                ReuseableHealthRestore = itemSource.ReuseableHealthRestore,
                ReuseableManaRestore = itemSource.ReuseableManaRestore,

                RuneLevel = itemSource.RuneLevel,
                ConsumableSubItemType = itemSource.ConsumableSubItemType
            };
        }

        public static PlayPageItemDetail.PlayPageItemSourceDetail MapToPayPageItemSourceDto(this ItemSource itemSource)
        {
            return new PlayPageItemDetail.PlayPageItemSourceDetail
            {
                FriendlyName = itemSource.FriendlyName,
                ItemType = itemSource.ItemType,
                PortraitUrl = itemSource.PortraitUrl
            };
        }

        public static ItemLeaderboardItemSourceDetail MapToLeaderboardItemSourceDetail(this ItemSource itemSource)
        {
            return new ItemLeaderboardItemSourceDetail
            {
                Id = itemSource.Id,
                FriendlyName = itemSource.FriendlyName,
                ItemType = itemSource.ItemType,
                PortraitUrl = itemSource.PortraitUrl
            };
        }
    }
}