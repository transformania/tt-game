using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.Identity.DTOs;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.Mappings
{
    public class ArtistBioMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArtistBio>()
                .ToTable("AuthorArtistBios")
                .HasKey(u => u.Id);
        }

        public ArtistBioMappings()
        {
            CreateMap<ArtistBio, ArtistBioDetail>();
        }
    }
}