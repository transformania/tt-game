using TT.Domain.World.Entities;

namespace TT.Tests.Builders.World
{
    public class XPLeaderboardEntryBuilder : Builder<XpLeaderboardEntry, int>
    {
        public XPLeaderboardEntryBuilder()
        {
            Instance = Create();
            With(u => u.Id, 3);
        }
    }
}
