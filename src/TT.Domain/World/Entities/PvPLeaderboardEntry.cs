﻿using TT.Domain.Entities;
using TT.Domain.Forms.Entities;
using TT.Domain.Players.Entities;
using TT.Domain.World.DTOs;

namespace TT.Domain.World.Entities
{
    public class PvPLeaderboardEntry : Entity<int>
    {

        public int Rank { get; protected set; }
        public int RoundNumber { get; protected set; }
        public string PlayerName { get; protected set; }
        public string Sex { get; protected set; }
        public string CovenantName { get; protected set; }
        public string FormName { get; protected set; }
        public string Mobility { get; protected set; }
        public FormSource FormSource { get; protected set; }
        public int Level { get; protected set; }
        public int DungeonPoints { get; protected set; }

        protected PvPLeaderboardEntry() { }

        public static PvPLeaderboardEntry Create(int rank, Player player, int round)
        {
            var newLeaderboard = new PvPLeaderboardEntry
            {
                Rank = rank,
                PlayerName = player.GetFullName(),
                RoundNumber = round,
                Sex = player.Gender,
                CovenantName = player.Covenant == null ? null : player.Covenant.Name,
                FormName = player.FormSource.FriendlyName,
                Mobility = player.Mobility,
                Level = player.Level,
                DungeonPoints = (int)player.PvPScore,
                FormSource = player.FormSource
            };
            return newLeaderboard;
        }

        public PvPLeaderboardEntryDetail MapToDto()
        {
            return new PvPLeaderboardEntryDetail
            {
                Rank = Rank,
                PlayerName = PlayerName,
                Sex = Sex,
                CovenantName = CovenantName,
                FormName = FormName,
                Mobility = Mobility,
                Level = Level,
                DungeonPoints = DungeonPoints,
                FormSource = new FormImageDetail { PortraitUrl = FormSource.PortraitUrl }
            };
        }
    }
}
