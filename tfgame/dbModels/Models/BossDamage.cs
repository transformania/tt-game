using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.dbModels.Models
{
    public class BossDamage
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int BossMembershipId { get; set; }
        public int PlayerAttacksOnBoss { get; set; }
        public int BossAttacksOnPlayer { get; set; }
        public float PlayerDamageOnBoss { get; set; }
        public float BossDamageOnPlayer { get; set; }
        public DateTime Timestamp { get; set; }
    }
}