using System;
using System.Collections.Generic;
using TT.Domain.Entities;
using TT.Domain.Identity.Entities;
using TT.Domain.Players.Entities;

namespace TT.Domain.Covenants.Entities
{
    public class Covenant : Entity<int>
    {
        public string Name { get; protected set; }

        public string FlagUrl { get; protected set; }
        public string SelfDescription { get; protected set; }
        public DateTime LastMemberAcceptance { get; protected set; }
        public Player Leader { get; protected set; }
        public bool IsPvP { get; protected set; }
        public string NoticeboardMessage { get; protected set; }
        public string Captains { get; protected set; } // TODO: convert to ICollection<Player> Captains
        public decimal Money { get; protected set; }
        public string HomeLocation { get; protected set; }
        public int Level { get; protected set; }

        public ICollection<Player> Players { get; protected set; }

        private Covenant() { }

    }
}
