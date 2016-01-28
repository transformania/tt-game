using System.Collections.Generic;
using tfgame.dbModels.Models;

namespace tfgame.ViewModels
{
    public class BioPageViewModel
    {
        public PlayerBio PlayerBio { get; set; }
        public IEnumerable<AchievementBadge> Badges { get; set; }
    }
}