using System.Collections.Generic;
using System.Linq;
using TT.Domain.Commands.Identity;
using TT.Domain.Entities.Players;
using TT.Domain.Entities.RPClassifiedAds;

namespace TT.Domain.Entities.Identity
{
    public class User : Entity<string>
    {
        public string UserName { get; private set; }
        public string Email { get; private set; }
        public ICollection<RPClassifiedAd> RPClassifiedAds { get; private set; } = new List<RPClassifiedAd>();
        public ICollection<Stat> Stats { get; private set; } = new List<Stat>();
        public ICollection<Strike> Strikes { get; private set; } = new List<Strike>();
        public ICollection<Strike> StrikesGiven { get; private set; } = new List<Strike>();
        public Donator Donator { get; protected set; }

        public ArtistBio ArtistBio { get; protected set; }

        private User() { }

        public void UpdateDonator(UpdateDonator cmd)
        {
            this.Donator.Update(cmd.PatreonName, cmd.Tier, cmd.ActualDonationAmount, cmd.SpecialNotes);
        }

        public void CreateDonator(CreateDonator cmd)
        {
            this.Donator = Donator.Create(cmd.PatreonName, cmd.Tier, cmd.ActualDonationAmount, cmd.SpecialNotes);
        }

        public void AddStat(string type, float amount)
        {
            var stat = this.Stats.FirstOrDefault(a => a.AchievementType == type);
            if (stat == null)
            {
                this.Stats.Add(Stat.Create(this, amount, type));
            }
            else
            {
                stat.AddAmount(amount);
            }
        }

    }
}