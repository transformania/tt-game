using TT.Domain.World.Entities;

namespace TT.Tests.Builders.World
{
    public class PvPLeaderboardEntryBuilder : Builder<PvPLeaderboardEntry, int>
    {
        public PvPLeaderboardEntryBuilder()
        {
            Instance = Create();
            With(u => u.Id, 3);
        }
    }
}
