using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.ViewModels
{
    public class WorldStats
    {
        public int TotalPlayers { get; set; }
        public int TotalLivingPlayers { get; set; }
        public int TotalAnimalPlayers { get; set; }
        public int TotalInanimatePlayers { get; set; }

        public int TotalUniquePlayers { get; set; }
        public int CurrentOnlinePlayers { get; set; }
      
    }
}