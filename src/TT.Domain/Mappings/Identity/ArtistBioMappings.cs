using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.DTOs.Identity;
using TT.Domain.Entities.Identity;

namespace TT.Domain.Mappings.Identies
{
    public class ArtistBIoMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArtistBio>()
                .ToTable("AuthorArtistBios")
                .HasKey(u => u.Id);
        }

        protected override void Configure()
        {
            CreateMap<ArtistBio, ArtistBioDetail>();
        }
    }
}