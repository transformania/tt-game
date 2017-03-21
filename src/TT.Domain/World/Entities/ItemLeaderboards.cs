using TT.Domain.Entities;
using TT.Domain.Items.Entities;

namespace TT.Domain.World.Entities
{
    public class ItemLeaderboardEntry : Entity<int>
    {

        public int Rank { get; protected set; }
        public int RoundNumber { get; protected set; }
        public string PlayerName { get; protected set; }
        public int GameMode { get; protected set; }
        public string Sex { get; protected set; }
        public string ItemName { get; protected set; }
        public ItemSource ItemSource { get; protected set; }
        public string CovenantName { get; protected set; }
        public string ItemType { get; protected set; }
     
        public int Level { get; protected set; }
        public float XP { get; protected set; }

        protected ItemLeaderboardEntry() { }

        public static ItemLeaderboardEntry Create(int rank, Item item, int round)
        {
            var newLeaderboard = new ItemLeaderboardEntry
            {
                Rank = rank,
                PlayerName = item.FormerPlayer.GetFullName(),
                RoundNumber = round,
                Sex = item.FormerPlayer.Gender,
                CovenantName = item.FormerPlayer.Covenant == null ? null : item.FormerPlayer.Covenant.Name,
                ItemName = item.ItemSource.FriendlyName,
                ItemSource = item.ItemSource,
                ItemType = item.ItemSource.ItemType,
                Level = item.Level,
                GameMode = item.FormerPlayer.GameMode,
                XP = item.FormerPlayer.ItemXP ==  null ? 0 : (float)item.FormerPlayer.ItemXP.Amount

            };
            return newLeaderboard;
        }

    }
}
