using System;
using System.ComponentModel.DataAnnotations;

namespace TT.Domain.Models
{
    public class Achievement
    {
        public int Id { get; set; }
        [StringLength(128)]
        public string OwnerMembershipId { get; set; }
        public float Amount { get; set; }
        public string AchievementType { get; set; }
        public int TimesEarned { get; set; }
        public DateTime Timestamp { get; set; }
    }
}