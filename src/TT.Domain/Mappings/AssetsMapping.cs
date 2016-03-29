using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.DTOs.Assets;
using TT.Domain.Entities.Assets;

namespace TT.Domain.Mappings
{
    public class AssetsMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tome>()
                .ToTable("Tomes")
                .HasKey(cr => cr.Id)
                .HasRequired(cr => cr.BaseItem).WithMany();
        }

        protected override void Configure()
        {
            CreateMap<Tome, TomeDetail>();
        }
    }
}