using System.Data.Entity;
using Highway.Data;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.Mappings
{
    public class DonatorMappings : IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Donator>()
                .ToTable("Donators")
                .HasKey(u => u.Id);
        }
    }
}