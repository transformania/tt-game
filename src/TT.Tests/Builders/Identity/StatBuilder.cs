using TT.Domain.Entities.Players;

namespace TT.Tests.Builders.Identity
{
   
    public class StatBuilder : Builder<Stat, int>
    {
        public StatBuilder()
        {
            Instance = Create();
            With(u => u.Id, 7);
        }
    }
}
