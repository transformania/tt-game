using System;

namespace TT.Domain.Identity.DTOs
{
    public class StrikeDetail
    {
        public int Id { get; set; }
        public UserDetail User { get; set; }
        public DateTime Timestamp { get; set; }
        public UserDetail FromModerator { get; set; }
        public string Reason { get; set; }
        public int Round { get; set; }
    }
}
