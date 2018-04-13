using TT.Domain.Items.Entities;

namespace TT.Domain.Items.Queries.Leaderboard.DTOs
{
    public class ItemLeaderboardInanimateXPDetail : BaseDTO<InanimateXP, int>
    {
        public decimal Amount { get; set; }
    }
}
