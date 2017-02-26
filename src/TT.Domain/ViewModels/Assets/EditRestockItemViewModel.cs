using TT.Domain.Assets.DTOs;

namespace TT.Domain.ViewModels.Assets
{
    public class UpdateRestockItemViewModel
    {
        public RestockItemDetail RestockItem { get; private set; }

        public UpdateRestockItemViewModel(RestockItemDetail restockItem)
        {
            RestockItem = restockItem;
        }
    }
}
 