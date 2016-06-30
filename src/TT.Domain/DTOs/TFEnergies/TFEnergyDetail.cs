using System;
using TT.Domain.Entities.Forms;
using TT.Domain.Entities.Players;

namespace TT.Domain.DTOs.TFEnergies
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
