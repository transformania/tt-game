using System;
using TT.Domain.Entities.Identity;

namespace TT.Domain.Entities.Identities
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
            var minutesUntilNextExpiration = 240 + (15*TimesPassed - 15*TimesFailed);

            if (minutesUntilNextExpiration < 60)
            {
                minutesUntilNextExpiration = 60;
            }

            if (minutesUntilNextExpiration > 1440)
            {
                minutesUntilNextExpiration = 1440;
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
