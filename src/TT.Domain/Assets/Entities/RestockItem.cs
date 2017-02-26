using TT.Domain.Assets.Commands;
using TT.Domain.Entities;
using TT.Domain.Items.Entities;

namespace TT.Domain.Assets.Entities
{
    public class RestockItem : Entity<int>
    {

        public ItemSource BaseItem { get; protected set; }
        public int AmountBeforeRestock { get; protected set; }
        public int AmountToRestockTo { get; protected set; }
        public int BotId { get; protected set; }

        private RestockItem() { }

        public static RestockItem Create(ItemSource baseItem, int amountBeforeRestock, int amountToRestockTo, int botId)
        {
            return new RestockItem
            {
                BaseItem = baseItem,
                AmountBeforeRestock = amountBeforeRestock,
                AmountToRestockTo = amountToRestockTo,
                BotId = botId
            };
        }

        public RestockItem Update(UpdateRestockItem cmd, ItemSource baseItem, int botId)
        {
            Id = cmd.RestockItemId;
            BaseItem = baseItem;
            BotId = botId;
            AmountToRestockTo = cmd.AmountToRestockTo;
            AmountBeforeRestock = cmd.AmountBeforeRestock;
            return this;
        }
    }
}