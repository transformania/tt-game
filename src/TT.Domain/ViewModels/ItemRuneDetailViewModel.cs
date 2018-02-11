using TT.Domain.Items.DTOs;

namespace TT.Domain.ViewModels
{
    public class ItemRuneDetailViewModel
    {

        public ItemRuneDetail itemRuneDetail { get; set; }
        public bool ShowName { get; set; }

        public ItemRuneDetailViewModel(ItemRuneDetail itemRuneDetail, bool ShowName = true)
        {
            this.itemRuneDetail = itemRuneDetail;
            this.ShowName = ShowName;
        }

        public string GetStyle(float value)
        {
            return value > 0 ? "good" : "bad";
        }

    }
}
