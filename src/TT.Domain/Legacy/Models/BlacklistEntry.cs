using System;
using System.ComponentModel.DataAnnotations;

namespace TT.Domain.Models
{
    public class BlacklistEntry
    {
        public int Id { get; set; }
        [StringLength(128)]
        public string CreatorMembershipId { get; set; }
        [StringLength(128)]
        public string TargetMembershipId { get; set; }
        public DateTime Timestamp { get; set; }
        public int BlacklistLevel { get; set; } // 0 == No attacking, // 1 == No attacking or messaging
    }
}