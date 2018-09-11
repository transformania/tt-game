using System;

namespace TT.Domain.Identity.DTOs
{
    public class ReportDetail
    {
        public int Id { get; set; }

        public UserDetail Reporter { get; protected set; }
        public UserDetail Reported { get; protected set; }

        public DateTime Timestamp { get; protected set; }

        public string Reason { get; protected set; }
        public string ModeratorResponse { get; set; }

        public int Round { get; protected set; }
    }
}
