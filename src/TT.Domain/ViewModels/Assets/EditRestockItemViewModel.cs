using TT.Domain.DTOs.Assets;

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
 