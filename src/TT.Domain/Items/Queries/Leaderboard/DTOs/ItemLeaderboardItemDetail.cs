using TT.Domain.Items.Entities;

namespace TT.Domain.Items.Queries.Leaderboard.DTOs
{
    public class ItemLeaderboardItemDetail : BaseDTO<Item, int>
    {
        public int Level { get;  set; }
        public bool IsPermanent { get; set; }

        public ItemLeaderboardPlayerDetail FormerPlayer { get; set; }
        public ItemLeaderboardItemSourceDetail ItemSource { get; set; }
    }
}
