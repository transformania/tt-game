using TT.Domain.Identity.Entities;

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
