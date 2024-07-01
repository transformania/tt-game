﻿using System.Data.Entity;
using Highway.Data;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.Mappings
{
    public class IdentityMappings : IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .ToTable("AspNetUsers")
                .HasKey(u => u.Id);

            modelBuilder.Entity<UserSecurityStamp>()
                .ToTable("AspNetUsers")
                .HasKey(u => u.Id)
                .HasRequired(uss => uss.User)
                .WithRequiredPrincipal(u => u.SecurityStamp);

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

            modelBuilder.Entity<Report>()
                .HasRequired(cr => cr.Reporter)
                .WithMany(s => s.ReportsGiven).Map(m => m.MapKey("Reporter"));

            modelBuilder.Entity<Report>()
                .HasRequired(cr => cr.Reported)
                .WithMany(s => s.ReportsReceived).Map(m => m.MapKey("Reported"));

            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany(ur => ur.Users)
                .Map(m => {
                    m.MapLeftKey("UserId");
                    m.MapRightKey("RoleId");
                    m.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity<Role>()
                .ToTable("AspNetRoles")
                .HasKey(r => r.Id);
        }
    }
}