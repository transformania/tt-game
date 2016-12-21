using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.DTOs.MindControl;
using TT.Domain.Entities.MindControl;

namespace TT.Domain.Mappings.MindControl
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

        protected override void Configure()
        {
            CreateMap<VictimMindControl, VictimMindControlDetail>();
        }
    }
}
