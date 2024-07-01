﻿using System.Data.Entity;
using Highway.Data;
using TT.Domain.Entities.Skills;

namespace TT.Domain.Skills.Mappings
{
    public class SkillSourceMappings : IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SkillSource>()
                .ToTable("DbStaticSkills")
                .HasKey(s => s.Id);

            modelBuilder.Entity<SkillSource>()
                .HasOptional(cr => cr.FormSource)
                .WithMany().Map(m => m.MapKey("FormSourceId"));

            modelBuilder.Entity<SkillSource>()
                .HasOptional(cr => cr.GivesEffectSource)
                .WithMany().Map(m => m.MapKey("GivesEffectSourceId"));

            modelBuilder.Entity<SkillSource>()
                .HasOptional(cr => cr.ExclusiveToFormSource)
                .WithMany().Map(m => m.MapKey("ExclusiveToFormSourceId"));

            modelBuilder.Entity<SkillSource>()
                .HasOptional(cr => cr.ExclusiveToItemSource)
                .WithMany().Map(m => m.MapKey("ExclusiveToItemSourceId"));
        }
    }
}
