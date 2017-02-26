using TT.Domain.Assets.DTOs;

namespace TT.Domain.ViewModels.Assets
{
    public class UpdateTomeViewModel
    {
        public TomeDetail Tome { get; private set; }

        public UpdateTomeViewModel(TomeDetail tome)
        {
            Tome = tome;
        }
    }
}
