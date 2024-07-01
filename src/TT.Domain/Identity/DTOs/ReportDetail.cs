using System;

namespace TT.Domain.Identity.DTOs
{
    public class ReportDetail
    {
        public int Id { get; set; }

        public UserDetail Reporter { get; protected internal set; }
        public UserDetail Reported { get; protected internal set; }

        public DateTime Timestamp { get; protected internal set; }

        public string Reason { get; protected internal set; }
        public string ModeratorResponse { get; set; }

        public int Round { get; protected internal set; }
    }
}
