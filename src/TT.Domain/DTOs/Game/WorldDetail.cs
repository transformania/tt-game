using System;
using System.Linq;
using TT.Domain.ViewModels;

namespace TT.Domain.DTOs.Game
{
    public class WorldDetail
    {
        public int Id { get;  set; }
        public int TurnNumber { get;  set; }
        public DateTime LastUpdateTimestamp { get;  set; }
        public bool WorldIsUpdating { get;  set; }
        public DateTime LastUpdateTimestamp_Finished { get;  set; }
        public bool Boss_DonnaActive { get;  set; }
        public string Boss_Donna { get;  set; }
        public string Boss_Valentine { get;  set; }
        public string Boss_Bimbo { get;  set; }
        public string Boss_Thief { get;  set; }
        public string Boss_Sisters { get;  set; }
        public string Boss_Faeboss { get;  set; }
        public string GameNewsDate { get;  set; }
        public bool TestServer { get;  set; }
        public bool ChaosMode { get;  set; }
        public int RoundDuration { get;  set; }
        public bool InbetweenRoundsNonChaos { get;  set; }
        public string RoundNumber { get; set; }

        /// <summary>
        /// Returns true if any of the NPC bosses are currently active
        /// </summary>
        /// <returns></returns>
        public bool AnyBossIsActive()
        {

            if (this.Boss_Thief == "active")
            {
                return true;
            }
            else if (this.Boss_Valentine == "active")
            {
                return true;
            }
            else if (this.Boss_Bimbo == "active")
            {
                return true;
            }
            else if (this.Boss_Donna == "active")
            {
                return true;
            }
            else if (this.Boss_Sisters == "active")
            {
                return true;
            }

            return false;
        }

        public bool IsDonnaAvailable()
        {
            if (Boss_Donna == "unstarted" && Boss_Valentine != "active" && Boss_Bimbo != "active" && Boss_Thief != "active" && Boss_Sisters != "active" && TurnNumber >= BossSummonDictionary.GlobalBossSummonDictionary.Values.FirstOrDefault(p => p.BossName == "Donna").MinimumTurn)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool IsValentineAvailable()
        {
            if (Boss_Valentine == "unstarted" && Boss_Donna != "active" && Boss_Bimbo != "active" && Boss_Thief != "active" && Boss_Sisters != "active" && TurnNumber >= BossSummonDictionary.GlobalBossSummonDictionary.Values.FirstOrDefault(p => p.BossName == "Valentine").MinimumTurn)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsBimboAvailable()
        {
            if (Boss_Bimbo == "unstarted" && Boss_Valentine != "active" && Boss_Donna != "active" && Boss_Thief != "active" && Boss_Sisters != "active" && TurnNumber >= BossSummonDictionary.GlobalBossSummonDictionary.Values.FirstOrDefault(p => p.BossName == "BimboBoss").MinimumTurn)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsTheifAvailable()
        {
            if (Boss_Thief == "unstarted" && Boss_Valentine != "active" && Boss_Donna != "active" && Boss_Bimbo != "active" && Boss_Sisters != "active" && TurnNumber >= BossSummonDictionary.GlobalBossSummonDictionary.Values.FirstOrDefault(p => p.BossName == "Thieves").MinimumTurn)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsSistersAvailable()
        {
            if (Boss_Sisters == "unstarted" && Boss_Thief != "active" && Boss_Valentine != "active" && Boss_Donna != "active" && Boss_Bimbo != "active" && TurnNumber >= BossSummonDictionary.GlobalBossSummonDictionary.Values.FirstOrDefault(p => p.BossName == "Sisters").MinimumTurn)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if the fae boss has not yet been started, no other bosses are active, and the minimum turn number has passed
        /// </summary>
        /// <returns></returns>
        public bool IsFaeBossAvailable()
        {
            if (this.Boss_Faeboss == "unstarted" &&
                !this.AnyBossIsActive() &&
                this.TurnNumber >= BossSummonDictionary.GlobalBossSummonDictionary.Values.FirstOrDefault(p => p.BossName == "FaeBoss").MinimumTurn)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
