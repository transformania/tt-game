using System;
using System.ComponentModel.DataAnnotations;

namespace tfgame.dbModels.Models
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Form { get; set; }
        public decimal Health { get; set; }
        public decimal Mana { get; set; }
        public decimal HealthMax { get; set; }
        public decimal ManaMax { get; set; }
        [StringLength(128)]
        public string SimpleMembershipId { get; set; }
        public string AtScene { get; set; }
        public DateTime LastDbSave { get; set; }
    }
}