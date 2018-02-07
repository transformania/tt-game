using System.Collections.Generic;
using TT.Domain.Items.DTOs;

namespace TT.Domain.ViewModels
{
    public class InventoryBonusesViewModel
    {
        public IEnumerable<ItemDetail> Items { get; set; }
        public BuffBox Bonuses { get; set; }
        public decimal Health { get; set; }
        public decimal MaxHealth { get; set; }
        public decimal Mana { get; set; }
        public decimal MaxMana { get; set; }
        public int CurrentCarryCount { get; set; }
    }
}