using System;
using TT.Domain.Commands.RPClassifiedAds;
using TT.Domain.Entities.Identity;

namespace TT.Domain.Entities.RPClassifiedAds
{
    public class RPClassifiedAd : Entity<int>
    {
        public User User { get; set; }
        public string Text { get; set; }
        public string YesThemes { get; set; }
        public string NoThemes { get; set; }
        public DateTime CreationTimestamp { get; set; }
        public DateTime RefreshTimestamp { get; set; }
        public string PreferredTimezones { get; set; }
        public string Title { get; set; }

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
