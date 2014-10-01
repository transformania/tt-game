using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.dbModels.Models
{
    public class DMRoll
    {
        public int Id { get; set; }
        public int MembershipOwnerId { get; set; }
        public string Message { get; set; }
        public string Tags { get; set; }
        public string ActionType { get; set; }
        public bool IsLive { get; set; }
    }
}