using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.World.DTOs;

namespace TT.Domain.World.Mappings
{
    public class DonatorMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.World>()
                .ToTable("PvPWorldStats")
                .HasKey(u => u.Id);
        }

        protected override void Configure()
        {
            CreateMap<Entities.World, WorldDetail>();
        }
    }
}
