using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.DTOs.Item;
using TT.Domain.Entities.Item;

namespace TT.Domain.Mappings.Item
{
    public class ItemSourceMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ItemSource>()
                .ToTable("DbStaticItems")
                .HasKey(cr => cr.Id);
        }

        protected override void Configure()
        {
            CreateMap<ItemSource, ItemSourceDetail>();
        }
    }
}