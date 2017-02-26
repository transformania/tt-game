using TT.Domain.Assets.Entities;
using TT.Domain.Statics;
using TT.Tests.Builders.AI;
using TT.Tests.Builders.Item;

namespace TT.Tests.Builders.Assets
{
    public class RestockItemBuilder : Builder<RestockItem, int>
    {
        public RestockItemBuilder()
        {
            Instance = Create();
            With(x => x.Id, 7);
            With(x => x.AmountBeforeRestock, 1);
            With(x => x.AmountToRestockTo, 3);
            With(x => x.BaseItem, new ItemSourceBuilder().BuildAndSave());
            With(x => x.BotId, AIStatics.LindellaBotId);
        }

    }
}