using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.Identity.DTOs;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.Mappings
{
    public class DonatorMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Donator>()
                .ToTable("Donators")
                .HasKey(u => u.Id);
        }

        public DonatorMappings()
        {
            CreateMap<Donator, DonatorDetail>();
        }
    }
}