using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.dbModels.Models
{
    public class ReservedName
    {
        public int Id { get; set; }
        public string MembershipId { get; set; }
        public string FullName { get; set; }
        public DateTime Timestamp { get; set; }
    }
}