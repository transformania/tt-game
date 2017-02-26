using TT.Domain.World.Entities;

namespace TT.Tests.Builders.Game
{
    public class WorldBuilder : Builder<World, int>
    {
        public WorldBuilder()
        {
            Instance = Create();
            With(f => f.Id, 7);
        }
    }
}

