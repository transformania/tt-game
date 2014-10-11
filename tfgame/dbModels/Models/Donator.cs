using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.dbModels.Models
{
    public class Donator
    {
        public int Id { get; set; }
        public int OwnerMembershipId { get; set; }
        public string PatreonName { get; set; }
        public int Tier { get; set; }
        public decimal ActualDonationAmount { get; set; }
        public DateTime Timestamp { get; set; }
        public bool HasBoughtCustomFormPortrait { get; set; }
    }
}