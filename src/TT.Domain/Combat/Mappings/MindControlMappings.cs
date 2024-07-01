using System.Data.Entity;
using Highway.Data;
using TT.Domain.Entities.MindControl;

namespace TT.Domain.Combat.Mappings
{
    public class MindControlMappings : IMappingConfiguration
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
    }
}
