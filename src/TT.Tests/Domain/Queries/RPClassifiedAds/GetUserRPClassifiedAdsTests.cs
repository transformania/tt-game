using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using TT.Domain.ClassifiedAds.Queries;
using TT.Domain.Entities.RPClassifiedAds;
using TT.Domain.Identity.Entities;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.RPClassifiedAds;

namespace TT.Tests.Domain.Queries.RPClassifiedAds
{
    [Category("RPClassifiedAd Tests")]
    public class GetUserRPClassifiedAdsTests : TestBase
    {
        private User user;
        private ICollection<RPClassifiedAd> ads = new List<RPClassifiedAd>();
        private GetUserRPClassifiedAds cmd;

        public override void SetUp()
        {
            base.SetUp();

            user = new UserBuilder()
                .With(u => u.Email, "JohnSmith@example.com")
                .With(u => u.UserName, "JohnSmith")
                .With(u => u.Id, "guid")
                .BuildAndSave();

            for (int i = 0; i < 3; i++)
            {
                ads.Add(new RPClassifiedAdBuilder()
                    .With(ad => ad.Title, "This Is a Title")
                    .With(ad => ad.Text, "This is some text. This is some text. This is some text.")
                    .With(ad => ad.YesThemes, "Everything")
                    .With(ad => ad.NoThemes, "rp with pictures")
                    .With(ad => ad.PreferredTimezones, "Anytime")
                    .With(ad => ad.User, user)
                    .With(ad => ad.OwnerMembershipId, user.Id)
                    .With(ad => ad.CreationTimestamp, DateTime.UtcNow)
                    .With(ad => ad.RefreshTimestamp, DateTime.UtcNow)
                    .BuildAndSave());
            }

            cmd = new GetUserRPClassifiedAds() { UserId = user.Id };
        }

        [Test]
        public void Should_get_all_users_ads()
        {
            var ads = Repository.Find(cmd);

            var adsq = from ad in this.ads
                       orderby ad.RefreshTimestamp descending
                       select ad;

            ads.Should().Equal(adsq);
        }
    }
}
