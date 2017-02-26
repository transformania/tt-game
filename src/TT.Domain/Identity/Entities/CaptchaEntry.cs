using System;
using TT.Domain.Entities;

namespace TT.Domain.Identity.Entities
{
    public class CaptchaEntry : Entity<int>
    {
        public User User { get; protected set; }
        public int TimesPassed { get; protected set; }
        public int TimesFailed { get; protected set; }
        public DateTime ExpirationTimestamp { get; protected set; }

        private CaptchaEntry()
        {
            
        }

        public void AddPassAttempt()
        {
            this.TimesPassed++;
        }

        public void AddFailAttempt()
        {
            this.TimesFailed++;
        }

        /// <summary>
        /// Updates the next expiration date.  The more the user successfully completes the captcha, the farther out
        /// the next expiration time is.  At most the expiration time is a day; at least it is one hour.
        /// </summary>
        public void UpdateExpirationTimestamp()
        {
            var minutesUntilNextExpiration = 1440 + (30*TimesPassed - 15*TimesFailed);

            if (minutesUntilNextExpiration < 720)
            {
                minutesUntilNextExpiration = 720;
            }

            if (minutesUntilNextExpiration > 4320)
            {
                minutesUntilNextExpiration = 4320;
            }

            this.ExpirationTimestamp = DateTime.UtcNow.AddMinutes(minutesUntilNextExpiration);
        }

        public static CaptchaEntry Create(User user)
        {
            return new CaptchaEntry
            {
                TimesPassed = 0,
                TimesFailed = 0,
                ExpirationTimestamp = DateTime.UtcNow.AddMinutes(1),
                User = user
            };
        }

    }

}
