using TT.Domain.Identity.Entities;

namespace TT.Tests.Builders.Identity
{
    public class DonatorBuilder : Builder<Donator, int>
    {
        public DonatorBuilder()
        {
            Instance = Create();
            With(u => u.Id, 7);
        }
    }
}
