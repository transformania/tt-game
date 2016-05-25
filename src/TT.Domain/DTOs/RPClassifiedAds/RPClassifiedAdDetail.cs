using System;
using TT.Domain.DTOs.Identity;

namespace TT.Domain.DTOs.RPClassifiedAds
{
    public class RPClassifiedAdDetail
    {
        public UserDetail User { get; set; }
        public string Text { get; set; }
        public string YesThemes { get; set; }
        public string NoThemes { get; set; }
        public DateTime CreationTimestamp { get; set; }
        public DateTime RefreshTimestamp { get; set; }
        public string PreferredTimezones { get; set; }
        public string Title { get; set; }
    }
}
