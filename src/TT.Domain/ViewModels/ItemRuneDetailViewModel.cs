using TT.Domain.Items.DTOs;

namespace TT.Domain.ViewModels
{
    public class ItemRuneDetailViewModel
    {

        public ItemRuneDetail itemRuneDetail { get; set; }

        public ItemRuneDetailViewModel(ItemRuneDetail itemRuneDetail)
        {
            this.itemRuneDetail = itemRuneDetail;
        }

        public string GetStyle(float value)
        {
            return value > 0 ? "good" : "bad";
        }

    }
}
