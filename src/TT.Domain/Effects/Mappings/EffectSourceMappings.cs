using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.Effects.DTOs;
using TT.Domain.Effects.Entities;

namespace TT.Domain.Effects.Mappings
{
    public class EffectSourceMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EffectSource>()
                .ToTable("DbStaticEffects")
                .HasKey(e => e.Id);
        }

        public EffectSourceMappings()
        {
            CreateMap<EffectSource, EffectSourceDetail>();
        }
    }
}
