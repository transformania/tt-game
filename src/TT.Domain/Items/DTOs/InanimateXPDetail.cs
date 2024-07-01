using System;

namespace TT.Domain.Items.DTOs
{
    public class InanimateXPDetail
    {
        public int Id { get; protected internal set; }
        public decimal Amount { get; protected internal set; }
        public int TimesStruggled { get; protected internal set; }
        public DateTime LastActionTimestamp { get; protected internal set; }
        public int LastActionTurnstamp { get; protected internal set; }
    }
}
