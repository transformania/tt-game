using System;
using System.ComponentModel.DataAnnotations;

namespace TT.Domain.Models
{
    public class PlayerBio
    {
        public int Id { get; set; }
        [StringLength(128)]
        public string OwnerMembershipId { get; set; }
        public string Text { get; set; }
        public string WebsiteURL { get; set; }
        public int PublicVisibility { get; set; }
        public string OtherNames { get; set; }
        public string Tags { get; set; }
        public DateTime Timestamp { get; set; }

    }
}