using TT.Domain.Items.DTOs;

namespace TT.Domain.Assets.DTOs
{
    public class RestockItemDetail
    {
        public int Id { get; private set; }
        public ItemSourceDetail BaseItem { get; protected set; }
        public int AmountBeforeRestock { get; protected set; }
        public int AmountToRestockTo { get; protected set; }
        public int BotId { get; protected set; }
    }
}
