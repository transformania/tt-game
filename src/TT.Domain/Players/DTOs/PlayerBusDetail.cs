using System;

namespace TT.Domain.Players.DTOs
{
    public class PlayerBusDetail
    {
        public decimal Money { get; set; }
        public decimal ActionPoints { get; set; }
        public DateTime LastCombatTimestamp { get; set; }
    }
}
