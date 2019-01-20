using System;
using TT.Domain.Players.Entities;

namespace TT.Domain.Entities.TFEnergies
{
    public class SelfRestoreEnergy : Entity<int>
    {
        public Player Owner { get; protected set; }
        public float Amount { get; protected set; }
        public DateTime Timestamp { get; protected set; }

        private SelfRestoreEnergy()
        {

        }

        public static SelfRestoreEnergy Create(Player player, float initialAmount)
        {
            var newEnergy = new SelfRestoreEnergy
            {
                Owner = player,
                Amount = initialAmount,
                Timestamp = DateTime.UtcNow
            };

            return newEnergy;
        }

        public void AddAmount(float amount)
        {
            this.Amount += amount;
            this.Timestamp = DateTime.UtcNow;
        }

        public void Reset()
        {
            this.Amount = 0;
            this.Timestamp = DateTime.UtcNow;
        }
    }
}
