using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.World.DTOs;
using TT.Domain.World.Entities;

namespace TT.Domain.World.Mappings
{
    public class LeaderboardMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.PvPLeaderboardEntry>()
                .ToTable("PvPLeaderboards")
                .HasKey(u => u.Id);

            modelBuilder.Entity<Entities.PvPLeaderboardEntry>()
               .HasRequired(p => p.FormSource)
               .WithMany().Map(p => p.MapKey("FormSourceId"));

            modelBuilder.Entity<Entities.XpLeaderboardEntry>()
                .ToTable("XPLeaderboards")
                .HasKey(u => u.Id);

            modelBuilder.Entity<Entities.XpLeaderboardEntry>()
               .HasRequired(p => p.FormSource)
               .WithMany().Map(p => p.MapKey("FormSourceId"));

            modelBuilder.Entity<Entities.ItemLeaderboardEntry>()
                .ToTable("ItemLeaderboards")
                .HasKey(u => u.Id);

            modelBuilder.Entity<Entities.ItemLeaderboardEntry>()
               .HasRequired(p => p.ItemSource)
               .WithMany().Map(p => p.MapKey("ItemSourceId"));
        }

        public LeaderboardMappings()
        {
            CreateMap<PvPLeaderboardEntry, PvPLeaderboardEntryDetail>();
            CreateMap<ItemLeaderboardEntry, ItemLeaderboardEntryDetail>();
            CreateMap<XpLeaderboardEntry, XpLeaderboardEntryDetail>();
        }
    }
}
