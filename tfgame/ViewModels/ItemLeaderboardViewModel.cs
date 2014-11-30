using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.ViewModels
{
    public class ItemLeaderboardViewModel
    {
        public string dbName { get; set; }
        public string VictimName { get; set; }
        public int PlayerId { get; set; }
        public int Level { get; set; }
        public decimal XP { get; set; }
        public DbStaticItem StaticItem { get; set; }
        public DbStaticForm StaticForm { get; set; }
    }

    public class SimpleItemLeaderboardViewModel
    {
        public Item Item { get; set; }
        public DbStaticItem StaticItem { get; set; }
        public int PlayerId { get; set; }
        public decimal ItemXP { get; set; }
    }

}