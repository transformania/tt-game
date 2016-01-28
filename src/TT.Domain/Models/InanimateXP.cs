using System;

namespace tfgame.dbModels.Models
{
    public class InanimateXP
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public decimal Amount { get; set; }
        public int TimesStruggled { get; set; }
        public DateTime LastActionTimestamp { get; set; }
        public int LastActionTurnstamp { get; set; }
    }
}