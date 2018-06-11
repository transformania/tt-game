using System;
using System.Linq;
using FeatureSwitch;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Domain.World.DTOs
{
    public class WorldDetail
    {
        public int Id { get;  set; }
        public int TurnNumber { get;  set; }
        public DateTime LastUpdateTimestamp { get;  set; }
        public bool WorldIsUpdating { get;  set; }
        public DateTime LastUpdateTimestamp_Finished { get;  set; }
        public string Boss_Donna { get;  set; }
        public string Boss_Valentine { get;  set; }
        public string Boss_Bimbo { get;  set; }
        public string Boss_Thief { get;  set; }
        public string Boss_Sisters { get;  set; }
        public string Boss_Faeboss { get;  set; }
        public string Boss_MotorcycleGang { get; set; }
        public string GameNewsDate { get;  set; }
        public bool TestServer { get;  set; }
        public bool ChaosMode { get;  set; }
        public int RoundDuration { get;  set; }
        public bool InbetweenRoundsNonChaos { get;  set; }
        public string RoundNumber { get; set; }
        public DateTime? RoundStartsAt { get; protected set; }

 

        /// <summary>
        /// Returns true if any of the NPC bosses are currently active
        /// </summary>
        /// <returns></returns>
        public bool AnyBossIsActive()
        {
            return this.Boss_Thief == AIStatics.ACTIVE ||
                   this.Boss_Valentine == AIStatics.ACTIVE ||
                   this.Boss_Bimbo == AIStatics.ACTIVE ||
                   this.Boss_Donna == AIStatics.ACTIVE ||
                   this.Boss_Sisters == AIStatics.ACTIVE ||
                   this.Boss_Faeboss == AIStatics.ACTIVE ||
                   this.Boss_MotorcycleGang == AIStatics.ACTIVE;
        }

        public bool IsDonnaAvailable()
        {
            return !AnyBossIsActive() &&
                   this.Boss_Donna == AIStatics.UNSTARTED &&
                   minimumTurnIsMet(AIStatics.DonnaBotId);
        }

        public bool IsValentineAvailable()
        {
            return !AnyBossIsActive() &&
                   this.Boss_Valentine == AIStatics.UNSTARTED &&
                   minimumTurnIsMet(AIStatics.ValentineBotId);
        }

        public bool IsBimboAvailable()
        {
            return !AnyBossIsActive() &&
                   this.Boss_Bimbo == AIStatics.UNSTARTED &&
                   minimumTurnIsMet(AIStatics.BimboBossBotId);
        }

        public bool IsTheifAvailable()
        {
            return !AnyBossIsActive() &&
                   this.Boss_Thief == AIStatics.UNSTARTED &&
                   minimumTurnIsMet(AIStatics.FemaleRatBotId);
        }

        public bool IsSistersAvailable()
        {
            return !AnyBossIsActive() &&
                   this.Boss_Sisters == AIStatics.UNSTARTED &&
                   minimumTurnIsMet(AIStatics.MouseNerdBotId);
        }

        /// <summary>
        /// Returns true if the fae boss has not yet been started, no other bosses are active, and the minimum turn number has passed
        /// </summary>
        /// <returns></returns>
        public bool IsFaeBossAvailable()
        {
            return !AnyBossIsActive() &&
                   this.Boss_Faeboss == AIStatics.UNSTARTED &&
                   minimumTurnIsMet(AIStatics.FaebossBotId);
        }

        /// <summary>
        /// Returns true if the motorcycle boss has not yet been started, no other bosses are active, and the minimum turn number has passed
        /// </summary>
        /// <returns></returns>
        public bool IsMotorCycleGangBossAvailable()
        {
            return !AnyBossIsActive() &&
                   FeatureContext.IsEnabled<EnableMotorcycleBoss>() && 
                   this.Boss_MotorcycleGang == AIStatics.UNSTARTED &&
                   minimumTurnIsMet(AIStatics.MotorcycleGangLeaderBotId);
        }

        private bool minimumTurnIsMet(int botId)
        {
            return this.TurnNumber >= BossSummonDictionary.GlobalBossSummonDictionary.Values.FirstOrDefault(p => p.BossId == botId)
                .MinimumTurn;
        }
    }
}
