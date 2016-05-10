using System;
using TT.Domain.DTOs.Players;

namespace TT.Domain.DTOs.Item
{
    public class ItemListingDetail
    {
        public int Id { get; set; }
        public ItemSourceListingDetail ItemSource { get; set; }
        public string dbLocationName { get; set; }
        public string VictimName { get; set; }
        public bool IsEquipped { get; set; }
        public int Level { get; set; }
        public int PvPEnabled { get; set; }
        public bool IsPermanent { get; set; }
        public string Nickname { get; set; }
        public DateTime LastSouledTimestamp { get; set; }
    }
}
