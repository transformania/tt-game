using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.Entities.RPClassifiedAds;
using TT.Domain.DTOs.RPClassifiedAds;

namespace TT.Domain.Mappings.RPClassifiedAds
{
    public class RPClassifiedAdMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RPClassifiedAd>()
                .ToTable("RPClassifiedAds")
                .HasKey(cr => cr.Id)
                .HasRequired(cr => cr.User).WithMany();
        }

        protected override void Configure()
        {
            CreateMap<RPClassifiedAd, RPClassifiedAdDetail>();
        }
    }
}
