using System;
using System.ComponentModel.DataAnnotations;

namespace TT.Domain.Models
{
    public class ReservedName
    {
        public int Id { get; set; }
        [StringLength(128)]
        public string MembershipId { get; set; }
        public string FullName { get; set; }
        public DateTime Timestamp { get; set; }
    }
}