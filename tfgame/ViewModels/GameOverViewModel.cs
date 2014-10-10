using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.ViewModels
{
    public class GameOverViewModel
    {
        public Player Player { get; set; }
        public DbStaticForm Form { get; set; }
        public DbStaticItem Item { get; set; }
        public PlayerFormViewModel WornBy { get; set; }
        public string AtLocation { get; set; }
        public int NewMessageCount { get; set; }
        public bool IsPermanent { get; set; }
        public IEnumerable<LocationLog> LocationLog { get; set; }
        public IEnumerable<PlayerFormViewModel> PlayersHere { get; set; }

        public decimal StruggleChance { get; set; }

        public WorldStats WorldStats { get; set; }

    }
}