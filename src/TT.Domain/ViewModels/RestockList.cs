using System.Collections.Generic;

namespace TT.Domain.ViewModels
{
    public class RestockListItem
    {
        public string dbName { get; set; }
        public int AmountBeforeRestock { get; set; }
        public int AmountToRestockTo { get; set; }
        public string Merchant { get; set; }
    }

    public class RestockList {
        public List<RestockListItem> RestockItmes { get; set; }
    }
}