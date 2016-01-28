using System.Collections.Generic;

namespace tfgame.ViewModels
{
    public class InventoryBonusesViewModel
    {
        public IEnumerable<ItemViewModel> Items { get; set; }
        public BuffBox Bonuses { get; set; }
        public decimal Health { get; set; }
        public decimal MaxHealth { get; set; }
        public decimal Mana { get; set; }
        public decimal MaxMana { get; set; }
    }
}