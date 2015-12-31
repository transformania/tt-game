using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.dbModels.Models
{
    public class DbStaticSkill
    {
        public int Id { get; set; }
        public string dbName { get; set; }
        public string FriendlyName { get; set; }
        public string FormdbName { get; set; }
        public string Description { get; set; }
        public decimal ManaCost { get; set; }
        public decimal TFPointsAmount { get; set; }
        public decimal HealthDamageAmount { get; set; }
        public string LearnedAtRegion { get; set; }
        public string LearnedAtLocation { get; set; }
        public string DiscoveryMessage { get; set; }
        public string IsLive { get; set; }
        public bool IsPlayerLearnable { get; set; }

        public string GivesEffect { get; set; }

        public string ExclusiveToForm { get; set; }
        public string ExclusiveToItem { get; set; }

        public string MobilityType { get; set; }
    }
}