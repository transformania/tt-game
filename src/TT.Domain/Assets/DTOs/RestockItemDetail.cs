using TT.Domain.Items.DTOs;

namespace TT.Domain.Assets.DTOs
{
    public class RestockItemDetail
    {
        public int Id { get; internal set; }
        public ItemSourceDetail BaseItem { get; protected internal set; }
        public int AmountBeforeRestock { get; protected internal set; }
        public int AmountToRestockTo { get; protected internal set; }
        public int BotId { get; protected internal set; }
    }
}
