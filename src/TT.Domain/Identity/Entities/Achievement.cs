using System;
using TT.Domain.Entities;

namespace TT.Domain.Identity.Entities
{
    public class Stat : Entity<int>
    {
        public User Owner { get; protected set; }
        public float Amount { get; protected set; }
        public string AchievementType { get; protected set; }
        public DateTime Timestamp { get; protected set; }

        public int TimesEarned { get; protected set; } // unused but can't be left out since the column in SQL is non-nullable

        protected Stat() { }

        public static Stat Create(User owner, float amount, string achievementType)
        {
            return new Stat
            {
                Owner = owner,
                Amount = amount,
                AchievementType = achievementType,
                Timestamp = DateTime.UtcNow,
                TimesEarned = 0
            };
        }

        public void AddAmount(float amount)
        {
            Amount += amount;
            Timestamp = DateTime.UtcNow;
        }

    }
}
