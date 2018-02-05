using System.Collections.Generic;
using TT.Domain.Items.DTOs;

namespace TT.Domain.ViewModels.NPCs
{
    public class LorekeeperBookListViewModel
    {
        public IEnumerable<ItemDetail> Items { get; set; }
        public decimal MyMoney { get; set; }
    }
}
