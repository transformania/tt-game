using System;
using System.Collections.Generic;
using TT.Domain.Players.DTOs;

namespace TT.Domain.ViewModels
{
    public class BusStopsViewModel
    {
        public IEnumerable<BusStop> Stops { get; set; }
        public PlayerBusDetail Player { get; set; }

        public int GetMinutesUntilOutOfCombat()
        {
            return (int)Math.Ceiling(12 - DateTime.UtcNow.Subtract(this.Player.LastCombatTimestamp).TotalMinutes);
        }

        public bool InCombatTooRecently()
        {
            return DateTime.UtcNow.Subtract(this.Player.LastCombatTimestamp).TotalMinutes < 12;
        }

        public bool PlayerHasEnergy()
        {
            return Player.ActionPoints > 3;
        }

    }
}
