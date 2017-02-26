using System;

namespace TT.Domain.Identity.DTOs
{
    public class CaptchaEntryDetail
    {
        public int Id { get; set; }
        public UserDetail User { get; set; }
        public int TimesPassed { get; set; }
        public int TimesFailed { get; set; }
        public DateTime ExpirationTimestamp { get; set; }
    }
}
