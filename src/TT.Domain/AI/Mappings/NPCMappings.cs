using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.AI.DTOs;
using TT.Domain.AI.Entities;

namespace TT.Domain.AI.Mappings
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