﻿using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.Identity.DTOs;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.Mappings
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

            modelBuilder.Entity<User>()
                .HasOptional(p => p.Donator)
                .WithRequired(d => d.Owner)
                .Map(m => m.MapKey("OwnerMembershipId"));

            modelBuilder.Entity<User>()
               .HasOptional(p => p.ArtistBio)
               .WithRequired(d => d.Owner)
               .Map(m => m.MapKey("OwnerMembershipId"));

            modelBuilder.Entity<Stat>()
                .ToTable("Achievements")
                .HasKey(u => u.Id);

            modelBuilder.Entity<Stat>()
                .HasRequired(cr => cr.Owner)
                .WithMany(s => s.Stats).Map(m => m.MapKey("OwnerMembershipId"));

            modelBuilder.Entity<Strike>()
                .ToTable("Strikes")
                .HasKey(u => u.Id);

            modelBuilder.Entity<Strike>()
               .HasRequired(cr => cr.User)
               .WithMany(s => s.Strikes).Map(m => m.MapKey("UserMembershipId"));

            modelBuilder.Entity<Strike>()
               .HasRequired(cr => cr.FromModerator)
               .WithMany(s => s.StrikesGiven).Map(m => m.MapKey("FromModerator"));

        }

        protected override void Configure()
        {
            CreateMap<User, UserDetail>();
            CreateMap<User, UserDonatorDetail>();
            CreateMap<Stat, StatDetail>();
        }
    }
}