using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TT.Domain.Entities.Identity;

namespace TT.Domain.DTOs.Identity
{
    public class DonatorDetail
    {
        public int Id { get; set; }
        public string PatreonName { get; set; }
        public int Tier { get; set; }
        public decimal ActualDonationAmount { get; set; }
        public string SpecialNotes { get; set; }
        public virtual UserDetail Owner { get; protected set; }
    }
}
