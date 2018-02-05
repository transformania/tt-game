using System.Collections.Generic;
using TT.Domain.Items.DTOs;

namespace TT.Domain.ViewModels
{
    public class PlayerFormItemsSkillsViewModel
    {
        public PlayerFormViewModel PlayerForm { get; set; }
        public IEnumerable<SkillViewModel> Skills { get; set; }
        public IEnumerable<ItemDetail> Items { get; set; }
        public BuffBox Bonuses { get; set; }
    }
}