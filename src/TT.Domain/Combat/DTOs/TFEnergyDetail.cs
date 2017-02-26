using System;
using TT.Domain.Forms.Entities;
using TT.Domain.Players.Entities;

namespace TT.Domain.Combat.DTOs
{
    public class TFEnergyDetail
    {
        public int Id { get; set; }
        public Player Owner { get; set; }
        public string FormName { get; set; }
        public FormSource FormSource { get; protected set; }
        public decimal Amount { get; set; }
        public Player Caster { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
