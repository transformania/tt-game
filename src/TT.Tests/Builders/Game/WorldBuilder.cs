namespace TT.Tests.Builders.Game
{
    public class WorldBuilder : Builder<TT.Domain.Entities.Game.World, int>
    {
        public WorldBuilder()
        {
            Instance = Create();
            With(f => f.Id, 7);
        }
    }
}

