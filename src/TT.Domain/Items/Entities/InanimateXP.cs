using System;
using TT.Domain.Entities;
using TT.Domain.Players.Entities;

namespace TT.Domain.Items.Entities
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
