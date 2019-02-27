using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Entities;

namespace TT.Domain.Items.Mappings
{
    public class ItemMappings : Profile, IMappingConfiguration
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

        public ItemMappings()
        {
            CreateMap<Item, ItemDetail>();
            CreateMap<Item, ItemListingDetail>();
            CreateMap<Item, ItemFormerPlayerDetail>();
            CreateMap<Item, ItemRuneDetail>();
        }
    }
}