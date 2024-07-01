using TT.Domain.Items.DTOs;

namespace TT.Domain.Assets.DTOs
{
    public class TomeDetail
    {
        public int Id { get; internal set; }
        public string Text { get; internal set; }
        public ItemSourceDetail BaseItem { get; internal set; }
    }
}
