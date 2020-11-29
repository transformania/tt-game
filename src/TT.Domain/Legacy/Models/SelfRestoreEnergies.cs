using System;

namespace TT.Domain.Models
{
    public class SelfRestoreEnergies
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public float Amount { get; set; }
        public DateTime Timestamp { get; set; }

    }
}