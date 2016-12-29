using TT.Domain.Entities.Items;

namespace TT.Tests.Builders.Item
{
    public class InanimateXPBuilder : Builder<InanimateXP, int>
    {
        public InanimateXPBuilder()
        {
            Instance = Create();
        }
    }
}
