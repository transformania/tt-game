using System.Data.Entity;
using AutoMapper;
using Highway.Data;
using TT.Domain.DTOs.Players;
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
                .WithMany().HasForeignKey(m => m.MembershipId);

            modelBuilder.Entity<Player>()
                .HasOptional(cr => cr.NPC)
                .WithMany().Map(m => m.MapKey("NPC"));

            modelBuilder.Entity<Player>()
                .HasRequired(p => p.FormSource)
                .WithMany().Map(p => p.MapKey("FormSourceId"));

            modelBuilder.Entity<Player>()
               .HasOptional(i => i.ItemXP)
               .WithOptionalPrincipal(p => p.Owner).Map(m => m.MapKey("OwnerId"));

        }

        protected override void Configure()
        {
            CreateMap<Player, PlayerDetail>();
            CreateMap<Player, PlayerMessageDetail>();
            CreateMap<Player, PlayerBusDetail>();
        }
    }
}