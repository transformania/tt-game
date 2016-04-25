using System;
using TT.Domain.Entities.Players;


namespace TT.Tests.Builders.Item
{
    public class PlayerBuilder : Builder<Player, int>
    {
        public PlayerBuilder()
        {
            Instance = Create();
            With(u => u.Id, 3);
        }
    }
}
