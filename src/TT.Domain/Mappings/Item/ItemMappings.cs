using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.DTOs.Item;

namespace TT.Domain.Mappings.Item
{
    public class ItemMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.Items.Item>()
                .ToTable("Items")
                .HasKey(cr => cr.Id);

            modelBuilder.Entity<Entities.Items.Item>()
                .HasRequired(cr => cr.ItemSource)
                .WithMany().Map(m => m.MapKey("ItemSourceId"));

            modelBuilder.Entity<Entities.Items.Item>()
                .HasOptional(cr => cr.Owner)
                .WithMany().Map(m => m.MapKey("OwnerId"));
        }

        protected override void Configure()
        {
            CreateMap<Entities.Items.Item, ItemDetail>();
        }
    }
}