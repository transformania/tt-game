using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace tfgame.dbModels.Models
{
    public class AchievementBadge
    {
        public int Id { get; set; }
        [StringLength(128)]
        public string OwnerMembershipId { get; set; }
        public string AchievementType { get; set; }
        public string Round { get; set; }
        public float Amount { get; set; }

    }
}