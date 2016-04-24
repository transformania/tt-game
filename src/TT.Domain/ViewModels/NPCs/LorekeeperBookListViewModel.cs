using System.Collections.Generic;

namespace TT.Domain.ViewModels.NPCs
{
    public class LorekeeperBookListViewModel
    {
        public IEnumerable<ItemViewModel> Items { get; set; }
        public decimal MyMoney { get; set; }
    }
}
