using System;
using System.Linq;
using TT.Domain.ViewModels;

namespace TT.Domain.Models
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
        public string Boss_Sisters { get; set; }
        public string Boss_Faeboss { get; set; }
        public string Boss_MotorcycleGang { get; set; }
        public string GameNewsDate { get; set; }
        public bool TestServer { get; set; }
        public bool ChaosMode { get; set; }
        public int RoundDuration { get; set; }
        public bool InbetweenRoundsNonChaos { get; set; }
        public DateTime? RoundStartsAt { get; set; }

    }
}