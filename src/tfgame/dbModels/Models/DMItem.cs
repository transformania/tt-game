using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace tfgame.dbModels.Models
{
    public class DMRoll
    {
        public int Id { get; set; }
        [StringLength(128)]
        public string MembershipOwnerId { get; set; }
        public string Message { get; set; }
        public string Tags { get; set; }
        public string ActionType { get; set; }
        public bool IsLive { get; set; }
    }
}