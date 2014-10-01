using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.ViewModels
{
    public class GameOverViewModelAnimal
    {
        public Player You { get; set; }
        public DbStaticForm Form { get; set; }
        public PlayerFormViewModel OwnedBy { get; set; }
        public Location Location { get; set; }
        public WorldStats WorldStats { get; set; }
        public IEnumerable<LocationLog> LocationLog { get; set; }

        public DateTime LastUpdateTimestamp { get; set; }

        public int NewMessageCount { get; set; }

        public IEnumerable<PlayerFormViewModel> PlayersHere { get; set; }
        public IEnumerable<ItemViewModel> LocationItems { get; set; }

        public bool IsPermanent { get; set; }

        public decimal StruggleChance { get; set; }

    }
}