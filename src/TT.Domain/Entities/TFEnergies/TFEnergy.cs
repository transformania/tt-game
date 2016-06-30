using System;
using TT.Domain.Commands.TFEnergies;
using TT.Domain.Entities.Forms;
using TT.Domain.Entities.Players;

namespace TT.Domain.Entities.TFEnergies
{
    public class TFEnergy : Entity<int>
    {
        public Player Owner { get; protected set; }
        public string FormName { get; protected set; }
        public FormSource FormSource { get; protected set; }
        public decimal Amount { get; protected set; }
        public Player Caster { get; protected set; }
        //public int CasterId { get; protected set; }
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
                FormName = cmd.FormName,
                Timestamp = DateTime.UtcNow
            };


            return newEnergy;
        }
    }

    




}
