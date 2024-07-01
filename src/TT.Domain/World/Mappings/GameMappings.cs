using System.Data.Entity;
using Highway.Data;

namespace TT.Domain.World.Mappings
{
    public class DonatorMappings : IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.World>()
                .ToTable("PvPWorldStats")
                .HasKey(u => u.Id);
        }
    }
}
