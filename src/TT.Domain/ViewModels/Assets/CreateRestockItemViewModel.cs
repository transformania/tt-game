using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT.Domain.ViewModels.Assets
{
    public class CreateRestockItemViewModel
    {
        public int BaseItemId { get; set; }
        public int AmountBeforeRestock { get; set; }
        public int AmountToRestockTo { get; set; }
        public int NPCId { get; set; }

        public CreateRestockItemViewModel()
        {
            AmountToRestockTo = 1;
        }

    }
}
 