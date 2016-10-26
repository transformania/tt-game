using System;

namespace TT.Domain.DTOs.Players
{
    public class PlayerBusDetail
    {
        public decimal Money { get; set; }
        public decimal ActionPoints { get; set; }
        public DateTime LastCombatTimestamp { get; set; }
    }
}
