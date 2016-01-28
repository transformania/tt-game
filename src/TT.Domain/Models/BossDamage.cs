using System;

namespace tfgame.dbModels.Models
{
    public class BossDamage
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int BossBotId { get; set; }
        public int PlayerAttacksOnBoss { get; set; }
        public int BossAttacksOnPlayer { get; set; }
        public float PlayerDamageOnBoss { get; set; }
        public float BossDamageOnPlayer { get; set; }
        public float TotalPoints { get; set;}
        public DateTime Timestamp { get; set; }
    }
}