using TT.Domain.Identity.DTOs;
using TT.Domain.Items.DTOs;
using TT.Domain.Players.DTOs;

namespace TT.Domain.Items.Queries.Leaderboard.DTOs
{
    public class ItemLeaderboardDetail
    {
        public ItemLeaderboardItemSourceDetail ItemSource { get; set; }

        public ItemLeaderboardInanimateXPDetail ItemXP { get; set; }

        public ItemLeaderboardPlayerDetail FormerPlayer { get; set; }

        public ItemLeaderboardItemDetail Item { get; set; }

        public ItemFormerPlayerDetail MapToFormerPlayerDto()
        {
            return new ItemFormerPlayerDetail
            {
                Id = Item.Id,
                FormerPlayer = new PlayerDetail
                {
                    Id = FormerPlayer.Id,
                    FirstName = FormerPlayer.FirstName,
                    LastName = FormerPlayer.LastName,
                    Nickname = FormerPlayer.Nickname,
                    Gender = FormerPlayer.Gender,
                    DonatorLevel = FormerPlayer.DonatorLevel,
                    Mobility = FormerPlayer.Mobility,
                    BotId = FormerPlayer.BotId
                },
                ItemSource = new ItemSourceDetail
                {
                    FriendlyName = ItemSource.FriendlyName,
                    PortraitUrl = ItemSource.PortraitUrl,
                    ItemType = ItemSource.ItemType
                },
                Level = Item.Level,
                IsPermanent = Item.IsPermanent
            };
        }
    }
}
