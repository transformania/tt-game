using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.DTOs.Effects;
using TT.Domain.Entities.Effects;

namespace TT.Domain.Mappings.Effects
{
    public class EffectMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Effect>()
                .ToTable("Effects")
                .HasKey(e => e.Id);

            modelBuilder.Entity<Effect>()
                .HasRequired(e => e.EffectSource)
                .WithMany().Map(m => m.MapKey("EffectSourceId"));

            modelBuilder.Entity<Effect>()
                .HasOptional(e => e.Owner)
                .WithMany(e => e.Effects).Map(m => m.MapKey("OwnerId"));
        }

        protected override void Configure()
        {
            CreateMap<Effect, EffectDetail>();
        }
    }
}
