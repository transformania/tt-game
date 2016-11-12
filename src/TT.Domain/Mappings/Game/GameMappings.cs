
using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.DTOs.Game;
using TT.Domain.Entities.Game;

namespace TT.Domain.Mappings.Game
{
    public class DonatorMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<World>()
                .ToTable("PvPWorldStats")
                .HasKey(u => u.Id);
        }

        protected override void Configure()
        {
            CreateMap<World, WorldDetail>();
        }
    }
}
