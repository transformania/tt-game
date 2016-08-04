using System.Collections.Generic;
using TT.Domain.Entities.Identities;
using TT.Domain.Entities.RPClassifiedAds;

namespace TT.Domain.Entities.Identity
{
    public class User : Entity<string>
    {
        public string UserName { get; private set; }
        public string Email { get; private set; }
        public ICollection<RPClassifiedAd> RPClassifiedAds { get; private set; } = new List<RPClassifiedAd>();

        private User() { }
    }
}