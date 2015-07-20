using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.dbModels.Models
{
    public class PlayerBio
    {
        public int Id { get; set; }
        public string OwnerMembershipId { get; set; }
        public string Text { get; set; }
        public string WebsiteURL { get; set; }
        public int PublicVisibility { get; set; }
        public string OtherNames { get; set; }
        public string Tags { get; set; }
        public DateTime Timestamp { get; set; }

    }
}