using TT.Domain.DTOs.Item;
using TT.Domain.Entities.Assets;
using TT.Domain.Entities.Item;

namespace TT.Domain.DTOs.Assets
{
    public class TomeDetail
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public ItemSourceDetail BaseItem { get; set; }

        public TomeDetail(Tome tome)
        {
            Id = tome.Id;
            Text = tome.Text;
            BaseItem = new ItemSourceDetail(tome.BaseItem);
        }

        public TomeDetail() {

        }
    }
}
