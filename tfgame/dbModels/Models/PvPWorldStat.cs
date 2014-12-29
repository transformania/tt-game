using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.ViewModels;

namespace tfgame.dbModels.Models
{
    public class PvPWorldStat
    {
        public int Id { get; set; }
        public int TurnNumber { get; set; }
        public DateTime LastUpdateTimestamp { get; set; }
        public bool WorldIsUpdating { get; set; }
        public DateTime LastUpdateTimestamp_Finished { get; set; }
        public bool Boss_DonnaActive { get; set; }
        public string Boss_Donna { get; set; }
        public string Boss_Valentine { get; set; }
        public string Boss_Bimbo { get; set; }
        public string Boss_Thief { get; set; }
        public string GameNewsDate { get; set; }
        public bool TestServer { get; set; }
        public bool InbetweenRoundsNonChaos { get; set; }

        public bool IsDonnaAvailable()
        {
            if (Boss_Donna == "unstarted" && Boss_Valentine != "active" && Boss_Bimbo != "active" && Boss_Thief != "active" && TurnNumber >= BossSummonDictionary.GlobalBossSummonDictionary.Values.FirstOrDefault(p => p.BossName == "Donna").MinimumTurn)
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
            if (Boss_Valentine == "unstarted" && Boss_Donna != "active" && Boss_Bimbo != "active" && Boss_Thief != "active" && TurnNumber >= BossSummonDictionary.GlobalBossSummonDictionary.Values.FirstOrDefault(p => p.BossName == "Valentine").MinimumTurn)
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
            if (Boss_Bimbo == "unstarted" && Boss_Valentine != "active" && Boss_Donna != "active" && Boss_Thief != "active" && TurnNumber >= BossSummonDictionary.GlobalBossSummonDictionary.Values.FirstOrDefault(p => p.BossName == "BimboBoss").MinimumTurn)
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
            if (Boss_Thief == "unstarted" && Boss_Valentine != "active" && Boss_Donna != "active" && Boss_Bimbo != "active" && TurnNumber >= BossSummonDictionary.GlobalBossSummonDictionary.Values.FirstOrDefault(p => p.BossName == "BimboBoss").MinimumTurn)
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