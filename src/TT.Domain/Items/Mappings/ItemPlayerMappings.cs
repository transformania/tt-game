using TT.Domain.Items.DTOs;
using TT.Domain.Items.Queries.Leaderboard.DTOs;
using TT.Domain.Players.Entities;

namespace TT.Domain.Items.Mappings;

public static class ItemPlayerMappings
{
    public static PlayPageItemDetail.PlayPagePlayerDetail MapPlayPagePlayerDto(this Player player)
        {
            if (player == null) return null;

            return new PlayPageItemDetail.PlayPagePlayerDetail
            {
                Id = player.Id,
                FirstName = player.FirstName,
                LastName = player.LastName,
                OriginalFirstName = player.OriginalFirstName,
                OriginalLastName = player.OriginalLastName,
                Nickname = player.Nickname,
                Gender = player.Gender,
                DonatorLevel = player.DonatorLevel,
                BotId = player.BotId,
                Mobility = player.Mobility,
                TimesAttackingThisUpdate = player.TimesAttackingThisUpdate,
                ItemsUsedThisTurn = player.ItemsUsedThisTurn
            };
        }

        public static PlayerForPsycho MapToPlayerForPsycho(this Player player)
        {
            if (player == null)
            {
                return null;
            }

            return new PlayerForPsycho
            {
                Id = player.Id,
                FirstName = player.FirstName,
                LastName = player.LastName,
                Nickname = player.Nickname,
                DonatorLevel = player.DonatorLevel
            };
        }

        public static ItemLeaderboardPlayerDetail MapToLeaderboardPlayerDto(this Player player)
        {
            return new ItemLeaderboardPlayerDetail
            {
                Id = player.Id,
                FirstName = player.FirstName,
                LastName = player.LastName,
                Nickname = player.Nickname,
                Gender = player.Gender,
                DonatorLevel = player.DonatorLevel,
                Mobility = player.Mobility,
                BotId = player.BotId,
                ItemXP = player.ItemXP == null ? null : new ItemLeaderboardInanimateXPDetail
                {
                    Id = player.ItemXP.Id,
                    Amount = player.ItemXP.Amount
                }
            };
        }
}