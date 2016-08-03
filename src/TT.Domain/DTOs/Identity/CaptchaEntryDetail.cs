using System;

namespace TT.Domain.DTOs.Identity
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
