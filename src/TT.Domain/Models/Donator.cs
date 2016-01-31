using System.ComponentModel.DataAnnotations;

namespace TT.Domain.Models
{
    public class Donator
    {
        public int Id { get; set; }
        [StringLength(128)]
        public string OwnerMembershipId { get; set; }
        public string PatreonName { get; set; }
        public int Tier { get; set; }
        public decimal ActualDonationAmount { get; set; }
        //public DateTime Timestamp { get; set; }
        //public bool HasBoughtCustomFormPortrait { get; set; }
        //public bool EarnedFromContributions { get; set; }
        public string SpecialNotes { get; set; }
    }
}