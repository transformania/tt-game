using TT.Domain.Entities.RPClassifiedAds;

namespace TT.Tests.Builders.RPClassifiedAds
{
    public class RPClassifiedAdBuilder : Builder<RPClassifiedAd, int>
    {
        public RPClassifiedAdBuilder()
        {
            Instance = Create();
        }
    }
}
