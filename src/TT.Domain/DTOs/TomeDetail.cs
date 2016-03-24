using TT.Domain.Entities.Assets;
using TT.Domain.Entities.Item;

namespace TT.Domain.DTOs
{
    public class TomeDetail
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public ItemSource BaseItem { get; set; }

        public TomeDetail(Tome tome)
        {
            Id = tome.Id;
            Text = tome.Text;
            BaseItem = tome.BaseItem;
        }

        public TomeDetail() {

        }
    }
}
