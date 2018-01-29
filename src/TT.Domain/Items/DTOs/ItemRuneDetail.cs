using System.Collections.Generic;
using TT.Domain.ClassifiedAds.DTOs;

namespace TT.Domain.Items.DTOs
{
    public class ItemRuneDetail
    {
        public int Id { get; set; }
        public ItemSourceDetail ItemSource { get; set; }
        public PlayerDetail Owner { get; set; }
        public PlayerDetail FormerPlayer { get; set; }
        public string VictimName { get; set; }
        public int Level { get; set; }
        public List<ItemRuneObject> Runes { get; protected set; }

        public string PrintVictimName()
        {
            if (FormerPlayer != null)
            {
                return $"(Formerly {FormerPlayer.FirstName} {FormerPlayer.LastName})";
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
