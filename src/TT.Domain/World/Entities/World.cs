using System;
using TT.Domain.Entities;

namespace TT.Domain.World.Entities
{
    public class World : Entity<int>
    {
        public int TurnNumber { get; protected set; }
        public DateTime LastUpdateTimestamp { get; protected set; }
        public bool WorldIsUpdating { get; protected set; }
        public DateTime LastUpdateTimestamp_Finished { get; protected set; }
        public bool Boss_DonnaActive { get; protected set; }
        public string Boss_Donna { get; protected set; }
        public string Boss_Valentine { get; protected set; }
        public string Boss_Bimbo { get; protected set; }
        public string Boss_Thief { get; protected set; }
        public string Boss_Sisters { get; protected set; }
        public string Boss_Faeboss { get; protected set; }
        public string GameNewsDate { get; protected set; }
        public bool TestServer { get; protected set; }
        public bool ChaosMode { get; protected set; }
        public int RoundDuration { get; protected set; }
        public bool InbetweenRoundsNonChaos { get; protected set; }
        public string RoundNumber { get; protected set; }
        public DateTime? RoundStartsAt { get; protected set; }

        protected World() { }

        public void SetRoundNumber(string roundNumber)
        {
            this.RoundNumber = roundNumber;
        }

    }
}
