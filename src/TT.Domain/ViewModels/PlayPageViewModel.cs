using System;
using System.Collections.Generic;
using TT.Domain.Items.DTOs;
using TT.Domain.Models;
using TT.Domain.World.DTOs;

namespace TT.Domain.ViewModels
{
    public class PlayPageViewModel
    {
        public PlayerFormViewModel You { get; set; }
        public IEnumerable<PlayerFormViewModel> PlayersHere { get; set; }

        public Location Location { get; set; }

        public int InventoryMaxSize { get; set; }

        public IEnumerable<LocationLogDetail> LocationLog { get; set; }
        public IEnumerable<PlayerLog> PlayerLog { get; set; }
        public IEnumerable<PlayerLog> PlayerLogImportant { get; set; }

        public IEnumerable<SkillViewModel> Skills { get; set; }
        public IEnumerable<ItemDetail> PlayerItems { get; set; }
        public IEnumerable<PlayPageItemDetail> LocationItems { get; set; }
        public WorldStats WorldStats { get; set; }
        public int AttacksMade { get; set; }
        public decimal APSearchCost { get; set; }

        public int UnreadMessageCount { get; set; }

        public DateTime LastUpdateTimestamp { get; set; }

        public WorldDetail World { get; set; }
        public bool PlayerIsAtBusStop { get; set; }

    }
}