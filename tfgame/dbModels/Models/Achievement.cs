using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.dbModels.Models
{
    public class Achievement
    {
        public int Id { get; set; }
        public string OwnerMembershipId { get; set; }
        public float Amount { get; set; }
        public string AchievementType { get; set; }
        public int TimesEarned { get; set; }
        public DateTime Timestamp { get; set; }
    }
}