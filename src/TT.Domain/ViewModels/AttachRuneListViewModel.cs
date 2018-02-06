using System.Collections.Generic;
using TT.Domain.Items.DTOs;
using TT.Domain.Statics;

namespace TT.Domain.ViewModels
{
   public class AttachRuneListViewModel
    {
        public ItemRuneDetail rune { get; set; }
        public IEnumerable<ItemRuneDetail> items { get; set; }

        public bool CanAttachRune(ItemRuneDetail item)
        {
            var runeLimit = item.ItemSource.ItemType == PvPStatics.ItemType_Pet ? 2 : 1;
            return item.Runes.Count < runeLimit && item.Level >= rune.ItemSource.RuneLevel;
        }
    }
}
