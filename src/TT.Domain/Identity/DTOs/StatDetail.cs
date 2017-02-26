using System;

namespace TT.Domain.Identity.DTOs
{
    public class StatDetail
    {
        public int Id { get; set; }
        public float Amount { get; set; }
        public string AchievementType { get; set; }
        public int TimesEarned { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
