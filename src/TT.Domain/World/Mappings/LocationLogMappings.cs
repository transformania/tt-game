using System.Data.Entity;
using Highway.Data;
using TT.Domain.Entities.LocationLogs;

namespace TT.Domain.World.Mappings
{
    public class LocationLogMappings : IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LocationLog>()
               .ToTable("LocationLogs")
               .HasKey(l => l.Id);

        }
    }
}
