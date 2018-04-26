using System;

namespace TT.Domain.Items.DTOs
{
    public class ItemListingDetail
    {
        public int Id { get; set; }
        public ItemSourceListingDetail ItemSource { get; set; }
        public string dbLocationName { get; set; }
        public bool IsEquipped { get; set; }
        public int Level { get; set; }
        public int PvPEnabled { get; set; }
        public bool IsPermanent { get; set; }
        public DateTime LastSouledTimestamp { get; set; }
    }
}
