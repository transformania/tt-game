using System.Collections.Generic;

namespace TT.Domain.Items.DTOs
{
    public class ItemRuneDetail
    {
        public int Id { get; set; }
        public InventoryItemSource ItemSource { get; set; }
        public PlayPageItemDetail.PlayPagePlayerDetail Owner { get; set; }
        public PlayPageItemDetail.PlayPagePlayerDetail FormerPlayer { get; set; }
        public int Level { get; set; }
        public List<ItemRuneObject> Runes { get; protected set; }

        public string PrintVictimName()
        {
            if (FormerPlayer != null)
            {
                var formerly = FormerPlayer.IsUsingOriginalName() ? "Formerly" : "Known as";
                return $"({formerly} {FormerPlayer.FirstName} {FormerPlayer.LastName})";
            }
            return "";
        }

        public class ItemRuneObject
        {
            public ItemRuneSource ItemSource { get; set; }
        }

        public class ItemRuneSource
        {
            public string FriendlyName { get; set; }
        }

    }

    
}
