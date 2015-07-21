using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace tfgame.dbModels.Models
{
    public class RPClassifiedAd
    {
        public int Id { get; set; }
        [StringLength(128)]
        public string OwnerMembershipId { get; set; }
        public string Text { get; set; }
        public string YesThemes { get; set; }
        public string NoThemes { get; set; }
        public DateTime CreationTimestamp { get; set; }
        public DateTime RefreshTimestamp { get; set; }
        public string PreferredTimezones { get; set; }
        public string Title { get; set; }
    }
}