using System;
using TT.Domain.Entities.Players;

namespace TT.Domain.Entities.Items
{
    public class InanimateXP : Entity<int>
    {

        public Player Owner { get; protected set; }
        public decimal Amount { get; protected set; }
        public int TimesStruggled { get; protected set; }
        public DateTime LastActionTimestamp { get; protected set; }
        public int LastActionTurnstamp { get; protected set; }

        private InanimateXP(){ }
    }
}
