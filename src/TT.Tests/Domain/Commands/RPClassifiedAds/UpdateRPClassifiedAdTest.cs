using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using TT.Domain.Commands.RPClassifiedAds;
using TT.Domain.Entities.Identity;
using TT.Domain.Entities.RPClassifiedAds;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.RPClassifiedAds;

namespace TT.Tests.Domain.Commands.RPClassifiedAds
{
    public class UpdateRPClassifiedAdTest : TestBase
    {
        private User JohnSmith;
        private RPClassifiedAd Ad;
        private UpdateRPClassifiedAd cmd;

        public override void SetUp()
        {
            base.SetUp();

            JohnSmith = new UserBuilder()
                .With(u => u.Email, "JohnSmith@example.com")
                .With(u => u.UserName, "JohnSmith")
                .With(u => u.Id, "guid")
                .BuildAndSave();

            Ad = new RPClassifiedAdBuilder()
                .With(ad => ad.Title, "This Is a Title")
                .With(ad => ad.Text, "This is some text. This is some text. This is some text.")
                .With(ad => ad.YesThemes, "Everything")
                .With(ad => ad.NoThemes, "rp with pictures")
                .With(ad => ad.PreferredTimezones, "Anytime")
                .With(ad => ad.User, JohnSmith)
                .With(ad => ad.OwnerMembershipId, JohnSmith.Id)
                .BuildAndSave();

            cmd = new UpdateRPClassifiedAd() {
                UserId = JohnSmith.Id,
                RPClassifiedAdId = Ad.Id,
                Title = "This Is a Title-",
                Text = "This is some text. This is some text. This is some text.-",
                YesThemes = "Everything-",
                NoThemes = "rp with pictures-",
                PreferredTimezones = "Anytime-",
            };
        }

        [Test]
        public void Should_update_all_columns()
        {
            Repository.Execute(cmd);
            var ads = DataContext.AsQueryable<RPClassifiedAd>();
            ads.Should().HaveCount(1);
            var ad = ads.Single();
            ad.Should().Be(Ad);

            ad.Title.Should().Be(cmd.Title);
            ad.Text.Should().Be(cmd.Text);
            ad.YesThemes.Should().Be(cmd.YesThemes);
            ad.NoThemes.Should().Be(cmd.NoThemes);
        }

        [Test]
        public void Should_update_refresh_timestamp()
        {
            Thread.Sleep(3000);
            Repository.Execute(cmd);

            var ads = DataContext.AsQueryable<RPClassifiedAd>();
            ads.Should().HaveCount(1);
            var ad = ads.Single();
            ad.Should().Be(Ad);

            ad.RefreshTimestamp.Should().BeCloseTo(DateTime.UtcNow, precision: 1000);
        }
    }
}
