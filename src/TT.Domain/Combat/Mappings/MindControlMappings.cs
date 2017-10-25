using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.Combat.DTOs;
using TT.Domain.Entities.MindControl;

namespace TT.Domain.Combat.Mappings
{
    public class MindControlMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<VictimMindControl>()
               .ToTable("MindControls")
               .HasKey(s => s.Id);

            modelBuilder.Entity<VictimMindControl>()
                .HasRequired(cr => cr.Victim)
                .WithMany(m => m.VictimMindControls).Map(m => m.MapKey("VictimId"));

        }

        public MindControlMappings()
        {
            CreateMap<VictimMindControl, VictimMindControlDetail>();
        }
    }
}
