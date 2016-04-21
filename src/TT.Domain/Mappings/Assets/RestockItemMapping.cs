using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.DTOs.Assets;
using TT.Domain.Entities.Assets;
using TT.Domain.Entities.NPCs;

namespace TT.Domain.Mappings.Assets
{
    public class RestockItemMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RestockItem>()
                .ToTable("RestockItems")
                .HasKey(cr => cr.Id)
                .HasRequired(cr => cr.BaseItem).WithMany();

            modelBuilder.Entity<RestockItem>()
                .HasRequired(cr => cr.NPC).WithMany();
        }

        protected override void Configure()
        {
            CreateMap<RestockItem, RestockItemDetail>();
        }
    }
}