using TT.Domain.Entities.Players;

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
