using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.ViewModels
{
    public class InventoryBonusesViewModel
    {
        public IEnumerable<ItemViewModel> Items { get; set; }
        public BuffBox Bonuses { get; set; }
    }
}