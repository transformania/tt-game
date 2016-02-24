using System.Data.Entity;
using Highway.Data;
using TT.Domain.Entities.Identity;

namespace TT.Domain.Mappings
{
    public class IdentityMappings : IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .ToTable("AspNetUsers")
                .HasKey(u => u.Id);
        }
    }
}