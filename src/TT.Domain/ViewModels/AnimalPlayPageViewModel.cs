using System;
using System.Collections.Generic;
using TT.Domain.DTOs.Game;
using TT.Domain.DTOs.Item;
using TT.Domain.DTOs.LocationLog;
using TT.Domain.Models;

namespace TT.Domain.ViewModels
{
    public class AnimalPlayPageViewModel
    {
        public Player You { get; set; }
        public DbStaticForm Form { get; set; }
        public ItemDetail YouItem { get; set; }
        public PlayerFormViewModel OwnedBy { get; set; }
        public Location Location { get; set; }


        public WorldDetail World { get; set; }
        public WorldStats WorldStats { get; set; }
        public IEnumerable<LocationLogDetail> LocationLog { get; set; }
        public IEnumerable<PlayerLog> PlayerLog { get; set; }
        public IEnumerable<PlayerLog> PlayerLogImportant { get; set; }

        public DateTime LastUpdateTimestamp { get; set; }

        public int NewMessageCount { get; set; }

        public IEnumerable<PlayerFormViewModel> PlayersHere { get; set; }
        public IEnumerable<ItemListingDetail> LocationItems { get; set; }

        public bool IsPermanent { get; set; }

        public decimal StruggleChance { get; set; }

        public bool RenderCaptcha { get; set; }

    }
}