using TT.Domain.Items.DTOs;

namespace TT.Domain.Assets.DTOs
{
    public class TomeDetail
    {
        public int Id { get; private set; }
        public string Text { get; private set; }
        public ItemSourceDetail BaseItem { get; private set; }
    }
}
