using System.Collections.Generic;
using TT.Domain.Commands.Identity;
using TT.Domain.Entities.Identities;
using TT.Domain.Entities.RPClassifiedAds;

namespace TT.Domain.Entities.Identity
{
    public class User : Entity<string>
    {
        public string UserName { get; private set; }
        public string Email { get; private set; }
        public ICollection<RPClassifiedAd> RPClassifiedAds { get; private set; } = new List<RPClassifiedAd>();
        public Donator Donator { get; protected set; }

        private User() { }

        public void UpdateDonator(UpdateDonator cmd)
        {
            this.Donator.Update(cmd.PatreonName, cmd.Tier, cmd.ActualDonationAmount, cmd.SpecialNotes);
        }

        public void CreateDonator(CreateDonator cmd)
        {
            this.Donator = Donator.Create(cmd.PatreonName, cmd.Tier, cmd.ActualDonationAmount, cmd.SpecialNotes);
        }
    }
}