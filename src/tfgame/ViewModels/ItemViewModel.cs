using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.ViewModels
{
    //public class ItemViewModel
    //{
    //    public Item dbItem { get; set; }
    //    public StaticItem Item { get; set; }
    //}

    public class ItemViewModel
    {
        public Item_VM dbItem { get; set; }
        public StaticItem Item { get; set; }
    }
}