using TT.Domain.Items.Entities;

namespace TT.Domain.Items.Queries.Leaderboard.DTOs
{
    public class ItemLeaderboardItemSourceDetail : BaseDTO<ItemSource, int>
    {
        public string FriendlyName { get; set; }
        public string PortraitUrl { get; set; }
        public string ItemType { get; set; }
    }
}
