using TT.Domain.Entities;
using TT.Domain.Forms.Entities;
using TT.Domain.Players.Entities;

namespace TT.Domain.World.Entities
{
    public class XpLeaderboardEntry : Entity<int>
    {

        public int Rank { get; protected set; }
        public int RoundNumber { get; protected set; }
        public string PlayerName { get; protected set; }
        public int GameMode { get; protected set; }
        public string Sex { get; protected set; }
        public string CovenantName { get; protected set; }
        public string FormName { get; protected set; }
        public string Mobility { get; protected set; }
        public FormSource FormSource { get; protected set; }
        public int Level { get; protected set; }
        public float XP { get; protected set; }

        protected XpLeaderboardEntry() { }

        public static XpLeaderboardEntry Create(int rank, Player player, int round)
        {
            var newLeaderboard = new XpLeaderboardEntry
            {
                Rank = rank,
                PlayerName = player.GetFullName(),
                RoundNumber = round,
                Sex = player.Gender,
                CovenantName = player.Covenant == null ? null : player.Covenant.Name,
                FormName = player.FormSource.FriendlyName,
                Mobility = player.Mobility,
                Level = player.Level,
                FormSource = player.FormSource,
                GameMode = player.GameMode,
                XP = (float)player.XP
            };
            return newLeaderboard;
        }

    }
}
