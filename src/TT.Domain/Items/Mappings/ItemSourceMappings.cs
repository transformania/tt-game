﻿using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Entities;

namespace TT.Domain.Items.Mappings
{
    public class ItemSourceMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ItemSource>()
                .ToTable("DbStaticItems")
                .HasKey(cr => cr.Id);

            modelBuilder.Entity<ItemSource>()
                .HasOptional(cr => cr.GivesEffectSource)
                .WithMany().Map(m => m.MapKey("GivesEffectSourceId"));
        }

        public ItemSourceMappings()
        {
            CreateMap<ItemSource, ItemSourceDetail>();
            CreateMap<ItemSource, ItemSourceListingDetail>();
        }
    }
}