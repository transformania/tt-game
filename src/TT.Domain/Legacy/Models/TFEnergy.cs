using System;

namespace TT.Domain.Models
{
    public class TFEnergy
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int FormSourceId { get; set; }
        public decimal Amount { get; set; }
        public int? CasterId { get; set; }
        public DateTime Timestamp { get; set; }

    }
}