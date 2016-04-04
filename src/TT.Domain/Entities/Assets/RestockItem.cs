using TT.Domain.Commands.Assets;
using TT.Domain.Entities.Item;
using TT.Domain.Entities.NPCs;

namespace TT.Domain.Entities.Assets
{
    public class RestockItem : Entity<int>
    {

        public ItemSource BaseItem { get; protected set; }
        public int AmountBeforeRestock { get; protected set; }
        public int AmountToRestockTo { get; protected set; }
        public NPC NPC { get; protected set; }

        private RestockItem() { }

        public static RestockItem Create(ItemSource baseItem, int amountBeforeRestock, int amountToRestockTo, NPC npc)
        {
            return new RestockItem
            {
                BaseItem = baseItem,
                AmountBeforeRestock = amountBeforeRestock,
                AmountToRestockTo = amountToRestockTo,
                NPC = npc
            };
        }

        public RestockItem Update(UpdateRestockItem cmd, ItemSource baseItem, NPC npc)
        {
            Id = cmd.RestockItemId;
            BaseItem = baseItem;
            NPC = npc;
            AmountToRestockTo = cmd.AmountToRestockTo;
            AmountBeforeRestock = cmd.AmountBeforeRestock;
            return this;
        }
    }
}