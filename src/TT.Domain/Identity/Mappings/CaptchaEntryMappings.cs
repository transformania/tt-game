using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.Identity.DTOs;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.Mappings
{
    public class CaptchaEntryMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CaptchaEntry>()
                .ToTable("CaptchaEntries")
                .HasKey(u => u.Id);

        }

        public CaptchaEntryMappings()
        {
            CreateMap<CaptchaEntry, CaptchaEntryDetail>();
        }
    }
}