using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.DTOs.Effects;
using TT.Domain.Entities.Effects;

namespace TT.Domain.Mappings.Effects
{
    public class EffectSourceMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EffectSource>()
                .ToTable("DbStaticEffects")
                .HasKey(e => e.Id);
        }

        protected override void Configure()
        {
            CreateMap<EffectSource, EffectSourceDetail>();
        }
    }
}
