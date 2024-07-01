﻿using System.Data.Entity;
using Highway.Data;

namespace TT.Domain.TFEnergies.Mappings
{
    public class TFEnergyMappings : IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Entities.TFEnergy>()
                            .ToTable("TFEnergies")
                            .HasKey(cr => cr.Id);

            modelBuilder.Entity<Entities.TFEnergy>()
                .HasRequired(p => p.Owner)
                .WithMany(p => p.TFEnergies).Map(p => p.MapKey("PlayerId"));

            modelBuilder.Entity<Entities.TFEnergy>()
                .HasOptional(p => p.Caster)
                .WithMany(p => p.TFEnergiesCast).Map(p => p.MapKey("CasterId"));

            modelBuilder.Entity<Entities.TFEnergy>()
                .HasRequired(p => p.FormSource)
                .WithMany().Map(p => p.MapKey("FormSourceId"));

        }
    }
}