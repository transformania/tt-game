﻿using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.DTOs.Players;
using TT.Domain.Entities.NPCs;
using TT.Domain.Entities.Players;

namespace TT.Domain.Mappings.Players
{
    public class PlayerMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>()
                .ToTable("Players")
                .HasKey(cr => cr.Id)
                .HasOptional(cr => cr.User)
                .WithMany().Map(m => m.MapKey("MembershipId"));

            modelBuilder.Entity<Player>()
                .HasOptional(cr => cr.NPC)
                .WithMany().Map(m => m.MapKey("NPC"));
        }

        protected override void Configure()
        {
            CreateMap<Player, PlayerDetail>();
        }
    }
}