using System.Data.Entity;
using Highway.Data;
using TT.Domain.AI.Entities;

namespace TT.Domain.AI.Mappings
{
    public class NPCMappings : IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NPC>()
                .ToTable("NPCs")
                .HasKey(u => u.Id);
        }
    }
}