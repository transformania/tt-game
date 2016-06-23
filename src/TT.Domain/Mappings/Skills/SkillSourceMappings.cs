using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.DTOs.Skills;
using TT.Domain.Entities.Skills;

namespace TT.Domain.Mappings.Skills
{
    public class SkillSourceMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SkillSource>()
                .ToTable("DbStaticSkills")
                .HasKey(s => s.Id);
        }

        protected override void Configure()
        {
            CreateMap<SkillSource, SkillSourceDetail>();
        }
    }
}
