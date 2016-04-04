using TT.Domain.Entities.Item;
using TT.Domain.Entities.NPCs;

namespace TT.Domain.DTOs.Assets
{
    public class RestockItemDetail
    {
        public int Id { get; private set; }
        public ItemSource BaseItem { get; protected set; }
        public int AmountBeforeRestock { get; protected set; }
        public int AmountToRestockTo { get; protected set; }
        public NPC NPC { get; protected set; }
    }
}
