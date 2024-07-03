using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Identity.DTOs;
using TT.Domain.Players.DTOs;
using TT.Domain.Players.Entities;

namespace TT.Domain.Players.Mappings
{
    public class PlayerMappings : IMappingConfiguration
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
    }

    public static class PlayerMappingExtensions
    {
        public static PlayerMessageDetail MapToPlayerMessageDto(this Player player)
        {
            if (player == null) return null;

            return new PlayerMessageDetail
            {
                Id = player.Id,
                FirstName = player.FirstName,
                LastName = player.LastName,
                DonatorLevel = player.DonatorLevel,
                Nickname = player.Nickname
            };
        }

        public static PlayerUserStrikesDetail MapToPlayerStrikesDto(this Player player)
        {
            if (player == null) return null;

            return new PlayerUserStrikesDetail
            {
                FirstName = player.FirstName,
                LastName = player.LastName,
                User = new UserStrikeDetail
                {

                    UserName = player.User.UserName,
                    Strikes = player.User.Strikes.Select(s => new StrikeDetail
                    {
                        Id = s.Id,
                        User = s.User.MapToDto(),
                        FromModerator = s.FromModerator.MapToDto(),
                        Timestamp = s.Timestamp,
                        Reason = s.Reason,
                        Round = s.Round
                    })
                },
            };
        }
    }

}