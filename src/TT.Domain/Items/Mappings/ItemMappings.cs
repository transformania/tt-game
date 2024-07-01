using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Entities;

// TODO: Fix this horrible mapping mess
// Yes, this is a mess but because we can't upgrade AutoMapper right now, we have to do manual mappings.
// Because of the state of item DTOs at the moment, this will be needed until a significant refactoring of the
// Detail DTOS and their usages is undertaken.

namespace TT.Domain.Items.Mappings
{
    public class ItemMappings : IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>()
                .ToTable("Items")
                .HasKey(cr => cr.Id);

            modelBuilder.Entity<Item>()
                .HasRequired(cr => cr.ItemSource)
                .WithMany().Map(m => m.MapKey("ItemSourceId"));

            modelBuilder.Entity<Item>()
                .HasOptional(cr => cr.Owner)
                .WithMany(cr => cr.Items).Map(m => m.MapKey("OwnerId"));

            modelBuilder.Entity<Item>()
                .HasOptional(i => i.FormerPlayer)
                .WithOptionalDependent(p => p.Item).Map(m => m.MapKey("FormerPlayerId"));

            modelBuilder.Entity<Item>()
                .HasOptional(i => i.EmbeddedOnItem)
                .WithMany(i => i.Runes).Map(m => m.MapKey("EmbeddedOnItemId"));

            modelBuilder.Entity<Item>()
                .HasOptional(i => i.SoulboundToPlayer)
                .WithMany().Map(m => m.MapKey("SoulboundToPlayerId"));

        }
    }

    public static class ItemMappingExtensions
    {
        public static ItemDetail MapToItemDto(this Item item)
        {
            if (item == null) return null;

            return new ItemDetail
            {
                Id = item.Id,
                dbName = item.dbName,
                ItemSource = item.ItemSource?.MapToInventoryItemSourceDto(),
                Owner = item.Owner?.MapPlayPagePlayerDto(),
                FormerPlayer = item.FormerPlayer?.MapPlayPagePlayerDto(),
                dbLocationName = item.dbLocationName,
                IsEquipped = item.IsEquipped,
                TurnsUntilUse = item.TurnsUntilUse,
                Level = item.Level,
                EquippedThisTurn = item.EquippedThisTurn,
                PvPEnabled = item.PvPEnabled,
                IsPermanent = item.IsPermanent,
                LastSouledTimestamp = item.LastSouledTimestamp,
                LastSold = item.LastSold,
                Runes = item.Runes.Select(r => r.MapToItemRuneDto()).ToList(),   // Assign this property based on your logic
                EmbeddedOnItem = item.EmbeddedOnItem?.MapToItemRuneDto(),  // Assign this property based on your logic
                SoulboundToPlayer = item.SoulboundToPlayer?.MapPlayPagePlayerDto(),  // Assign this property based on your logic
                ConsentsToSoulbinding = item.ConsentsToSoulbinding,
            };
        }

        public static ItemRuneDetail MapToItemRuneDto(this Item item)
        {
            if (item == null) return null;

            return new ItemRuneDetail
            {
                Id = item.Id,
                ItemSource = item.ItemSource?.MapToInventoryItemSourceDto(),
                Owner = item.Owner?.MapPlayPagePlayerDto(),
                FormerPlayer = item.FormerPlayer?.MapPlayPagePlayerDto(),
                Level = item.Level,
                Runes = item.Runes.Select(r => new ItemRuneDetail.ItemRuneObject { ItemSource = new ItemRuneDetail.ItemRuneSource
                {
                    FriendlyName = r.ItemSource.FriendlyName
                } }).ToList()
            };
        }

        public static ItemListingDetail MapToListingDto(this Item item)
        {
            if (item == null) return null;

            return new ItemListingDetail
            {
                Id = item.Id,
                ItemSource = item.ItemSource?.MapToSourceListingDto(), // We will have to use properties of this.ItemSource to populate this DTO. Properties are not available in the provided code
                dbLocationName = item.dbLocationName,
                IsEquipped = item.IsEquipped,
                Level = item.Level,
                PvPEnabled = item.PvPEnabled,
                IsPermanent = item.IsPermanent,
                LastSouledTimestamp = item.LastSouledTimestamp
            };
        }

        public static ItemFormerPlayerDetail MapToFormerPlayerDto(this Item item)
        {
            if (item == null) return null;

            return new ItemFormerPlayerDetail
            {
                Id = item.Id,
                ItemSource = item.ItemSource?.MapToItemSourceDto(),
                FormerPlayer = item.FormerPlayer?.MapToDto(),
                Level = item.Level,
                IsPermanent = item.IsPermanent,
            };
        }

        public static PlayPageItemDetail MapPlayPageItemDto(this Item item)
        {
            if (item == null) return null;

            return new PlayPageItemDetail
            {
                Id = item.Id,

                ItemSource = item.ItemSource?.MapToPayPageItemSourceDto(),
                FormerPlayer = item.FormerPlayer?.MapPlayPagePlayerDto(),
                SoulboundToPlayer = item.SoulboundToPlayer != null ? new PlayPageItemDetail.PlayPageSoulboundToPlayerDetail
                {
                    Id = item.SoulboundToPlayer.Id
                } : null,
                dbLocationName = item.dbLocationName,
                Runes = item.Runes?.Select(r => new PlayPageItemDetail.PlayPageItemRuneDetail
                {
                    ItemSource = r.ItemSource?.MapToPayPageItemSourceDto()
                }).ToList(),
                Level = item.Level,
                PvPEnabled = item.PvPEnabled,
                ItemSourceId = item.ItemSource?.Id ?? 0
            };
        }


    }
}