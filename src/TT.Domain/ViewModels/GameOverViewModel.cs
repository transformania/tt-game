using System;
using System.Collections.Generic;
using TT.Domain.DTOs.Item;
using TT.Domain.DTOs.LocationLog;
using TT.Domain.Models;

namespace TT.Domain.ViewModels
{
    public class GameOverViewModel
    {
        public Player Player { get; set; }
        public DbStaticForm Form { get; set; }
        public ItemDetail Item { get; set; }
        public PlayerFormViewModel WornBy { get; set; }
        public string AtLocation { get; set; }
        public int NewMessageCount { get; set; }
        public bool IsPermanent { get; set; }
        public IEnumerable<LocationLogDetail> LocationLog { get; set; }
        public IEnumerable<PlayerFormViewModel> PlayersHere { get; set; }
        public IEnumerable<PlayerLog> PlayerLog { get; set; }
        public IEnumerable<PlayerLog> PlayerLogImportant { get; set; }

        public decimal StruggleChance { get; set; }

        public WorldStats WorldStats { get; set; }

        public PvPWorldStat PvPWorldStat { get; set; }

        public DateTime LastUpdateTimestamp { get; set; }

    }
}