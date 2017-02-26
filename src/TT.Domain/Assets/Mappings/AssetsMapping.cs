using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.Assets.DTOs;
using TT.Domain.Assets.Entities;

namespace TT.Domain.Assets.Mappings
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