using System.Data.Entity;
using Highway.Data;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.Mappings
{
    public class ArtistBioMappings : IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArtistBio>()
                .ToTable("AuthorArtistBios")
                .HasKey(u => u.Id);
        }
    }
}