using System;
using TT.Domain.ClassifiedAds.Commands;
using TT.Domain.ClassifiedAds.DTOs;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Entities.RPClassifiedAds
{
    public class RPClassifiedAd : Entity<int>
    {
        public User User { get; protected set; }
        public string OwnerMembershipId { get; protected set; }

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

        public RPClassifiedAdDetail MapToDto()
        {
            return new RPClassifiedAdDetail
            {
                Id = Id,
                OwnerMembershipId = OwnerMembershipId,
                Title = Title,
                Text = Text,
                YesThemes = YesThemes,
                NoThemes = NoThemes,
                CreationTimestamp = CreationTimestamp,
                RefreshTimestamp = RefreshTimestamp,
                PreferredTimezones = PreferredTimezones
            };
        }
    }
}
