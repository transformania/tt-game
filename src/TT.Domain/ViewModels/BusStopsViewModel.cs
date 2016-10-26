using System;
using System.Collections.Generic;
using TT.Domain.DTOs.Players;

namespace TT.Domain.ViewModels
{
    public class BusStopsViewModel
    {
        public IEnumerable<BusStop> Stops { get; set; }
        public PlayerBusDetail Player { get; set; }

        public int GetMinutesUntilOutOfCombat()
        {
            return (int)Math.Ceiling(15 - DateTime.UtcNow.Subtract(this.Player.LastCombatTimestamp).TotalMinutes);
        }

        public bool InCombatTooRecently()
        {
            return DateTime.UtcNow.Subtract(this.Player.LastCombatTimestamp).TotalMinutes < 15;
        }

        public bool PlayerHasEnergy()
        {
            return Player.ActionPoints > 3;
        }

    }
}
