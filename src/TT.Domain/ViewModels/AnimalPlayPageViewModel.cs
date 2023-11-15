﻿using System;
using System.Collections.Generic;
using TT.Domain.Items.DTOs;
using TT.Domain.Models;
using TT.Domain.World.DTOs;

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

        public bool HasNewMessages { get; set; }
        public int UnreadMessageCount { get; set; }

        public IEnumerable<PlayerFormViewModel> PlayersHere { get; set; }
        public IEnumerable<PlayPageItemDetail> LocationItems { get; set; }

        public bool IsPermanent { get; set; }
        public string PortraitUrl { get; set; }

        public decimal StruggleChance { get; set; }
        public string Message { get; set; }

        public bool RenderCaptcha { get; set; }
        public bool HasOwnerChat { get; set; }
        public bool FriendOnlyMessages { get; set; }
    }
}