using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.DTOs.Identity;
using TT.Domain.Entities.Identities;

namespace TT.Domain.Mappings.Identies
{
    public class CaptchaEntryMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CaptchaEntry>()
                .ToTable("CaptchaEntries")
                .HasKey(u => u.Id);

        }

        protected override void Configure()
        {
            CreateMap<CaptchaEntry, CaptchaEntryDetail>();
        }
    }
}