namespace TT.Domain.ViewModels.Assets
{
    public class CreateRestockItemViewModel
    {
        public int BaseItemId { get; set; }
        public int AmountBeforeRestock { get; set; }
        public int AmountToRestockTo { get; set; }
        public int BotId { get; set; }

        public CreateRestockItemViewModel()
        {
            AmountToRestockTo = 1;
        }

    }
}
 