using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.Assets.DTOs;
using TT.Domain.Assets.Entities;

namespace TT.Domain.Assets.Mappings
{
    public class RestockItemMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RestockItem>()
                .ToTable("RestockItems")
                .HasKey(cr => cr.Id)
                .HasRequired(cr => cr.BaseItem).WithMany();

        }

        public RestockItemMappings()
        {
            CreateMap<RestockItem, RestockItemDetail>();
        }
    }
}