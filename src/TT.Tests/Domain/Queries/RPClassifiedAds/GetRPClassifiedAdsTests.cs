using NUnit.Framework;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Entities.Identity;
using TT.Domain.Entities.RPClassifiedAds;
using TT.Domain.Queries.RPClassifiedAds;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.RPClassifiedAds;

namespace TT.Tests.Domain.Queries.RPClassifiedAds
{
    [Category("RPClassifiedAd Tests")]
    public class GetRPClassifiedAdsTest : TestBase
    {
        private ICollection<User> users = new List<User>();
        private ICollection<RPClassifiedAd> ads = new List<RPClassifiedAd>();
        private GetRPClassifiedAds cmd;

        public override void SetUp()
        {
            base.SetUp();

            // Populate database users ads
            for (int i = 0; i < 3; i++)
            {
                users.Add(new UserBuilder()
                    .With(u => u.Email, "JohnSmith@example.com")
                    .With(u => u.UserName, "JohnSmith")
                    .With(u => u.Id, "guid")
                    .BuildAndSave());
            }

            // Populate database with ads
            foreach (var user in users)
            {
                for (int i = 0; i < 3; i++)
                {
                    ads.Add(new RPClassifiedAdBuilder()
                        .With(ad => ad.Title, "This Is a Title")
                        .With(ad => ad.Text, "This is some text. This is some text. This is some text.")
                        .With(ad => ad.YesThemes, "Everything")
                        .With(ad => ad.NoThemes, "rp with pictures")
                        .With(ad => ad.PreferredTimezones, "Anytime")
                        .With(ad => ad.User, user)
                        .With(ad => ad.CreationTimestamp, DateTime.UtcNow)
                        .With(ad => ad.RefreshTimestamp, DateTime.UtcNow)
                        .BuildAndSave());
                }
            }

            cmd = new GetRPClassifiedAds() { CutOff = TimeSpan.FromDays(3) };
            ((List<RPClassifiedAd>)ads)[0].RefreshTimestamp.AddDays(-4);
        }

        [Test]
        public void Should_get_ads_with_cutOff()
        {
            var ads = Repository.Find(cmd);

            var adsq = from ad in this.ads
                       where ad.RefreshTimestamp >= DateTime.UtcNow.Add(cmd.CutOff.Negate())
                       orderby ad.RefreshTimestamp descending
                       select ad;

            ads.Select(ad => ad.RPClassifiedAd).Should().Equal(adsq);
        }

        [Test]
        public void Should_get_all_ads()
        {
            var ads = Repository.Find(new GetRPClassifiedAds());

            var adsq = from ad in this.ads
                       orderby ad.RefreshTimestamp descending
                       select ad;

            ads.Select(ad => ad.RPClassifiedAd).Should().Equal(adsq);
        }
    }
}
