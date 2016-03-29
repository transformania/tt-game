using TT.Domain.DTOs.Item;

namespace TT.Domain.DTOs.Assets
{
    public class TomeDetail
    {
        public int Id { get; private set; }
        public string Text { get; private set; }
        public ItemSourceDetail BaseItem { get; private set; }
    }
}
