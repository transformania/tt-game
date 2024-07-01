﻿using System.Data.Entity;
using Highway.Data;
using TT.Domain.Effects.Entities;

namespace TT.Domain.Effects.Mappings
{
    public class EffectMappings : IMappingConfiguration
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
    }
}
