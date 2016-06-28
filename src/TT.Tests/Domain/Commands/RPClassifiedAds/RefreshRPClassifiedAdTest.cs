using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using TT.Domain.Commands.RPClassifiedAds;
using TT.Domain.Entities.Identity;
using TT.Domain.Entities.RPClassifiedAds;
using TT.Domain.Exceptions.RPClassifiedAds;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.RPClassifiedAds;

namespace TT.Tests.Domain.Commands.RPClassifiedAds
{
    [Category("RPClassifiedAd Tests")]
    public class RefreshRPClassifiedAdTest : TestBase
    {
        private User JohnSmith;
        private RPClassifiedAd Ad;
        private RefreshRPClassifiedAd cmd;

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

            cmd = new RefreshRPClassifiedAd() { UserId = JohnSmith.Id, RPClassifiedAdId = Ad.Id };
        }

        [Test]
        public void Should_refresh_ad()
        {
            Thread.Sleep(3000);
            Repository.Execute(cmd);

            var ads = DataContext.AsQueryable<RPClassifiedAd>();
            ads.Should().HaveCount(1);
            var ad = ads.Single();
            ad.Should().Be(Ad);

            ad.RefreshTimestamp.Should().BeCloseTo(DateTime.UtcNow, precision: 1000);
        }

        [Test]
        public void Should_refresh_ad_if_CheckUserId_is_false()
        {
            Thread.Sleep(3000);
            cmd.CheckUserId = false;
            cmd.UserId = null;
            Repository.Execute(cmd);

            var ads = DataContext.AsQueryable<RPClassifiedAd>();
            ads.Should().HaveCount(1);
            var ad = ads.Single();
            ad.Should().Be(Ad);

            ad.RefreshTimestamp.Should().BeCloseTo(DateTime.UtcNow, precision: 1000);
        }

        [Test]
        public void Should_throw_if_user_does_not_own_ad()
        {
            cmd.UserId += "-";
            Action action = () => Repository.Execute(cmd);
            action.ShouldThrowExactly<RPClassifiedAdNotOwnerException>()
                .WithMessage(string.Format("User {0} does not own RPClassifiedAdId {1}", cmd.UserId, cmd.RPClassifiedAdId))
                .And.UserFriendlyError.Should().Be("You do not own this RP Classified Ad.");
        }

        [Test]
        public void Should_throw_if_ad_does_not_exist()
        {
            cmd.RPClassifiedAdId++;
            Action action = () => Repository.Execute(cmd);
            action.ShouldThrowExactly<RPClassifiedAdNotFoundException>()
                .WithMessage(string.Format("RPClassifiedAdId with ID {0} could not be found", cmd.RPClassifiedAdId))
                .And.UserFriendlyError.Should().Be("This RP Classified Ad doesn't exist.");
        }
    }
}
