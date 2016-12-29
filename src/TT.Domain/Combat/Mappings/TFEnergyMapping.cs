using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.Combat.DTOs;
using TT.Domain.Entities.TFEnergies;

namespace TT.Domain.Combat.Mappings
{
    public class TFEnergyMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<TFEnergy>()
                            .ToTable("TFEnergies")
                            .HasKey(cr => cr.Id);

            modelBuilder.Entity<TFEnergy>()
                .HasRequired(p => p.Owner)
                .WithMany(p => p.TFEnergies).Map(p => p.MapKey("PlayerId"));

            modelBuilder.Entity<TFEnergy>()
                .HasOptional(p => p.Caster)
                .WithMany(p => p.TFEnergiesCast).Map(p => p.MapKey("CasterId"));

            modelBuilder.Entity<TFEnergy>()
                .HasRequired(p => p.FormSource)
                .WithMany().Map(p => p.MapKey("FormSourceId"));

        }

        protected override void Configure()
        {
            CreateMap<TFEnergy, TFEnergyDetail>();
        }
    }
}