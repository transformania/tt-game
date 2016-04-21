using System.Data.Entity;
using Highway.Data;
using TT.Domain.Entities.NPCs;
using TT.Domain.DTOs.AI;
using AutoMapper;

namespace TT.Domain.Mappings.AI
{
    public class NPCMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NPC>()
                .ToTable("NPCs")
                .HasKey(u => u.Id);
        }

        protected override void Configure()
        {
            CreateMap<NPC, NPCDetail>();
        }
    }
}