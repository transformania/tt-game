using System.Data.Entity;
using Highway.Data;
using TT.Domain.Covenants.Entities;
using TT.Domain.Players.Entities;

namespace TT.Domain.Covenants.Mappings
{
    public class CovenantMappings : IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Covenant>()
                .ToTable("Covenants")
                .HasKey(e => e.Id);

            modelBuilder.Entity<Player>()
                .HasOptional(p => p.CovenantLed)
                .WithRequired(d => d.Leader)
                .Map(m => m.MapKey("LeaderId"));

        }
    }
}
