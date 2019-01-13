using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.Players.DTOs;
using TT.Domain.Players.Entities;

namespace TT.Domain.Players.Mappings
{
    public class PlayerMappings : Profile, IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>()
                .ToTable("Players")
                .HasKey(cr => cr.Id)
                .HasOptional(cr => cr.User)
                .WithMany().HasForeignKey(m => m.MembershipId);

            modelBuilder.Entity<Player>()
                .HasOptional(cr => cr.NPC)
                .WithMany().Map(m => m.MapKey("NPC"));

            modelBuilder.Entity<Player>()
                .HasRequired(p => p.FormSource)
                .WithMany().Map(p => p.MapKey("FormSourceId"));

            modelBuilder.Entity<Player>()
                .HasRequired(p => p.OriginalFormSource)
                .WithMany().Map(p => p.MapKey("OriginalFormSourceId"));

            modelBuilder.Entity<Player>()
               .HasOptional(i => i.ItemXP)
               .WithOptionalPrincipal(p => p.Owner).Map(m => m.MapKey("OwnerId"));

            modelBuilder.Entity<Player>()
                .HasOptional(i => i.Covenant)
                .WithMany(c => c.Players).Map(c => c.MapKey("Covenant"));

        }

        public PlayerMappings()
        {
            CreateMap<Player, PlayerDetail>();
            CreateMap<Player, PlayerMessageDetail>();
            CreateMap<Player, PlayerBusDetail>();
        }
    }
}