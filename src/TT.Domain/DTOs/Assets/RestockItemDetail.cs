using TT.Domain.DTOs.AI;
using TT.Domain.DTOs.Item;

namespace TT.Domain.DTOs.Assets
{
    public class RestockItemDetail
    {
        public int Id { get; private set; }
        public ItemSourceDetail BaseItem { get; protected set; }
        public int AmountBeforeRestock { get; protected set; }
        public int AmountToRestockTo { get; protected set; }
        public NPCDetail NPC { get; protected set; }
    }
}
