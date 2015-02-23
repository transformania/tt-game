using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.ViewModels
{
    public class PlayerAchievementViewModel
    {
        public Player Player { get; set; }
        public Achievement Achivement { get; set; }
    }
}