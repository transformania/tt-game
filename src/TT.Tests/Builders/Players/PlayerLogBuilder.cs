using TT.Domain.Players.Entities;

namespace TT.Tests.Builders.Players
{
    public class PlayerLogBuilder : Builder<PlayerLog, int>
    {
        public PlayerLogBuilder()
        {
            Instance = Create();
            With(u => u.Id, 3);
        }

    }
}
