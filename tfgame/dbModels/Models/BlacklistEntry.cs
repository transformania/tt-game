using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.dbModels.Models
{
    public class BlacklistEntry
    {
        public int Id { get; set; }
        public int CreatorMembershipId { get; set; }
        public int TargetMembershipId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}