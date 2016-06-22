
using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.DTOs.Players;
using TT.Domain.DTOs.Skills;
using TT.Domain.Entities.Skills;

namespace TT.Domain.Mappings.Skills
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
