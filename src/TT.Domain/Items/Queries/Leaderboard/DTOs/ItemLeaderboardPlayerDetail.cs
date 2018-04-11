using TT.Domain.Players.Entities;

namespace TT.Domain.Items.Queries.Leaderboard.DTOs
{
    public class ItemLeaderboardPlayerDetail : BaseDTO<Player, int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nickname { get; set; }
        public string Gender { get; set; }
        public int DonatorLevel { get; set; }
        public string Mobility { get; set; }
        public int BotId { get; set; }

        public ItemLeaderboardInanimateXPDetail ItemXP { get; set; }
    }
}
