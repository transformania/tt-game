using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.dbModels.Models
{
    public class RPPoint
    {
        public int Id { get; set; }
        public string OwnerMembershipId { get; set; }
        public int Amount { get; set; }
        public int RemainingPointsToGive { get; set; }
    }
}