using TT.Domain.Covenants.Entities;

namespace TT.Tests.Builders.Covenants
{
    public class CovenantBuilder : Builder<Covenant, int>
    {
        public CovenantBuilder()
        {
            Instance = Create();
            With(u => u.Id, 3);
        }
    }
}
