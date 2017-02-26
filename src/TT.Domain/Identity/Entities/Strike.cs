using System;
using TT.Domain.Entities;

namespace TT.Domain.Identity.Entities
{
    public class Strike : Entity<int>
    {
        public User User { get; protected set; }

        public DateTime Timestamp { get; protected set; }

        public User FromModerator { get; protected set; }
        public string Reason { get; protected set; }

        public int Round { get; protected set; }

        private Strike()
        {

        }

        public static Strike Create(User user, User moderator, string reason, int round)
        {
            return new Strike
            {
                User = user,
                FromModerator = moderator,
                Reason = reason,
                Timestamp = DateTime.UtcNow,
                Round = round
            };
        }
    }
}
