using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.Entities.Skills;
using TT.Domain.Skills.DTOs;

namespace TT.Domain.Skills.Mappings
{
    public class SkillMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Skill>()
                .ToTable("Skills")
                .HasKey(s => s.Id);

            modelBuilder.Entity<Skill>()
               .HasRequired(s => s.SkillSource)
               .WithMany().Map(m => m.MapKey("SkillSourceId"));

            modelBuilder.Entity<Skill>()
                .HasRequired(cr => cr.Owner)
                .WithMany(s => s.Skills).Map(m => m.MapKey("OwnerId"));

        }

        protected override void Configure()
        {
            CreateMap<Skill, SkillDetail>();
            CreateMap<SkillSource, SkillSourceDetail>();
        }
    }
}
