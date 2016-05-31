using System;
using TT.Domain.Commands.RPClassifiedAds;
using TT.Domain.Entities.Identity;

namespace TT.Domain.Entities.RPClassifiedAds
{
    public class RPClassifiedAd : Entity<int>
    {
        public User User { get; protected set; }
        public string OwnerMembershipId { get; set; }

        public string Text { get; protected set; }
        public string YesThemes { get; protected set; }
        public string NoThemes { get; protected set; }
        public DateTime CreationTimestamp { get; protected set; }
        public DateTime RefreshTimestamp { get; protected set; }
        public string PreferredTimezones { get; protected set; }
        public string Title { get; protected set; }

        private RPClassifiedAd()
        {
        }

        public static RPClassifiedAd Create(User user, CreateRPClassifiedAd cmd)
        {
            return new RPClassifiedAd
            {
                User = user,
                Text = cmd.Text,
                YesThemes = cmd.YesThemes,
                NoThemes = cmd.NoThemes,
                CreationTimestamp = DateTime.UtcNow,
                RefreshTimestamp = DateTime.UtcNow,
                PreferredTimezones = cmd.PreferredTimezones,
                Title = cmd.Title
            };
        }

        public RPClassifiedAd Update(UpdateRPClassifiedAd cmd)
        {
            Text = cmd.Text;
            YesThemes = cmd.YesThemes;
            NoThemes = cmd.NoThemes;
            RefreshTimestamp = DateTime.UtcNow;
            PreferredTimezones = cmd.PreferredTimezones;
            Title = cmd.Title;

            return this;
        }

        public RPClassifiedAd Refresh(RefreshRPClassifiedAd cmd)
        {
            RefreshTimestamp = DateTime.UtcNow;

            return this;
        }
    }
}
