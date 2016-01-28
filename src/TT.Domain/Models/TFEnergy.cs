using System;

namespace tfgame.dbModels.Models
{
    public class TFEnergy
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public string FormName { get; set; }
        public decimal Amount { get; set; }
        public int CasterId { get; set; }
        public DateTime Timestamp { get; set; }

    }
}