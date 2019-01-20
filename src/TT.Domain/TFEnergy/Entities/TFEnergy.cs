using System;
using TT.Domain.Entities;
using TT.Domain.Forms.Entities;
using TT.Domain.Players.Entities;
using TT.Domain.TFEnergies.Commands;

namespace TT.Domain.TFEnergies.Entities
{
    public class TFEnergy : Entity<int>
    {
        public Player Owner { get; protected set; }
        public FormSource FormSource { get; protected set; }
        public decimal Amount { get; protected set; }
        public Player Caster { get; protected set; }
        public DateTime Timestamp { get; protected set; }

        private TFEnergy()
        {

        }

        public static TFEnergy Create(Player player, Player caster, FormSource form, CreateTFEnergy cmd)
        {
            var newEnergy = new TFEnergy
            {
                Owner = player,
                Caster = caster,
                FormSource = form,
                Amount = cmd.Amount,
                Timestamp = DateTime.UtcNow
            };

            return newEnergy;
        }

        public void SetAmount(decimal amount)
        {
            this.Amount = amount;
        }
    }
}
