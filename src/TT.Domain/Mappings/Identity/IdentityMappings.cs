using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.DTOs.Identity;
using TT.Domain.Entities.Identity;

namespace TT.Domain.Mappings.Identity
{
    public class IdentityMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .ToTable("AspNetUsers")
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .HasOptional(p => p.Donator)
                .WithRequired(d => d.Owner)
                .Map(m => m.MapKey("OwnerMembershipId"));

        }

        protected override void Configure()
        {
            CreateMap<User, UserDetail>();
            CreateMap<User, UserDonatorDetail>();
        }
    }
}