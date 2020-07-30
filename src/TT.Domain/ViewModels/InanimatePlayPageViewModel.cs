using System;
using System.Collections.Generic;
using TT.Domain.Items.DTOs;
using TT.Domain.Models;
using TT.Domain.World.DTOs;

namespace TT.Domain.ViewModels
{
    public class InanimatePlayPageViewModel
    {
        public Player Player { get; set; }
        public DbStaticForm Form { get; set; }
        public ItemDetail Item { get; set; }
        public PlayerFormViewModel WornBy { get; set; }
        public string AtLocation { get; set; }
        public bool HasNewMessages { get; set; }
        public int UnreadMessageCount { get; set; }
        public IEnumerable<LocationLogDetail> LocationLog { get; set; }
        public IEnumerable<PlayerFormViewModel> PlayersHere { get; set; }
        public IEnumerable<PlayerLog> PlayerLog { get; set; }
        public IEnumerable<PlayerLog> PlayerLogImportant { get; set; }

        public decimal StruggleChance { get; set; }

        public WorldStats WorldStats { get; set; }

        public WorldDetail World { get; set; }

        public DateTime LastUpdateTimestamp { get; set; }
        public bool RenderCaptcha { get; set; }

    }
}