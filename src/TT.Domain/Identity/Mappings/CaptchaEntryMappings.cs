using System.Data.Entity;
using Highway.Data;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.Mappings
{
    public class CaptchaEntryMappings : IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CaptchaEntry>()
                .ToTable("CaptchaEntries")
                .HasKey(u => u.Id);

        }
    }
}