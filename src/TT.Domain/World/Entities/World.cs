using System;
using TT.Domain.Entities;
using TT.Domain.World.DTOs;

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
        public string Boss_MotorcycleGang { get; protected set; }
        public bool JokeShop { get; set; }
        public string GameNewsDate { get; protected set; }
        public bool TestServer { get; protected set; }
        public bool ChaosMode { get; protected set; }
        public int RoundDuration { get; protected set; }
        public bool InbetweenRoundsNonChaos { get; protected set; }
        public string RoundNumber { get; protected set; }
        public DateTime? RoundStartsAt { get; protected set; }
        public string TurnTimeConfiguration { get; protected set; }

        protected World() { }

        public void SetRoundNumber(string roundNumber)
        {
            this.RoundNumber = roundNumber;
        }

        public WorldDetail MapToDto()
        {
            return new WorldDetail
            {
                Id = Id,
                TurnNumber = TurnNumber,
                LastUpdateTimestamp = LastUpdateTimestamp,
                WorldIsUpdating = WorldIsUpdating,
                LastUpdateTimestamp_Finished = LastUpdateTimestamp_Finished,
                Boss_Donna = Boss_Donna,
                Boss_Valentine = Boss_Valentine,
                Boss_Bimbo = Boss_Bimbo,
                Boss_Thief = Boss_Thief,
                Boss_Sisters = Boss_Sisters,
                Boss_Faeboss = Boss_Faeboss,
                Boss_MotorcycleGang = Boss_MotorcycleGang,
                JokeShop = JokeShop,
                GameNewsDate = GameNewsDate,
                TestServer = TestServer,
                ChaosMode = ChaosMode,
                RoundDuration = RoundDuration,
                InbetweenRoundsNonChaos = InbetweenRoundsNonChaos,
                RoundNumber = RoundNumber,
                RoundStartsAt = RoundStartsAt,
                TurnTimeConfiguration = TurnTimeConfiguration
            };
        }
    }
}
