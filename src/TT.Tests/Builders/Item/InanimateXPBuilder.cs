using TT.Domain.Items.Entities;

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
