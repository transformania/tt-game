using System.Collections.Generic;
using TT.Domain.Models;

namespace TT.Domain.ViewModels
{
    public class BioPageViewModel
    {
        public Player Player { get; set; }
        public PlayerBio PlayerBio { get; set; }
        public IEnumerable<AchievementBadge> Badges { get; set; }
        public bool IsMyBio { get; set; }
    }
}