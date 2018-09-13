using System;
using TT.Domain.Entities;

namespace TT.Domain.Identity.Entities
{
   public class Report : Entity<int>
    {
        public User Reporter { get; protected set; }
        public User Reported { get; protected set; }

        public DateTime Timestamp { get; protected set; }

        public string Reason { get; protected set; }
        public string ModeratorResponse { get; set; }

        public int Round { get; protected set; }

        private Report()
        {

        }

        public static Report Create(User reporter, User reported, string reason, int round)
        {
            return new Report
            {
                Reporter = reporter,
                Reported = reported,
                Reason = reason,
                Timestamp = DateTime.UtcNow,
                Round = round,
                ModeratorResponse = "Pending moderator response"
            };
        }

        public void SetModeratorResponse(string moderatorResponse)
        {
            this.ModeratorResponse = moderatorResponse;
        }


    }
}
