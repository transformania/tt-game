using TT.Domain.World.Entities;

namespace TT.Tests.Builders.World
{
    public class ItemLeaderboardEntryBuilder : Builder<ItemLeaderboardEntry, int>
    {
        public ItemLeaderboardEntryBuilder()
        {
            Instance = Create();
            With(u => u.Id, 3);
        }
    }
}
