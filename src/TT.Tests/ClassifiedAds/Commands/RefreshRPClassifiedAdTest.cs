using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using TT.Domain.ClassifiedAds.Commands;
using TT.Domain.Entities.RPClassifiedAds;
using TT.Domain.Exceptions.RPClassifiedAds;
using TT.Domain.Identity.Entities;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.RPClassifiedAds;

namespace TT.Tests.ClassifiedAds.Commands
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
            Assert.That(() => Repository.Execute(cmd), Throws.Nothing);

            var ads = DataContext.AsQueryable<RPClassifiedAd>();
            Assert.That(ads, Has.Exactly(1).Items);
            var ad = ads.Single();
            Assert.That(ad, Is.EqualTo(Ad));

            Assert.That(ad.RefreshTimestamp, Is.EqualTo(DateTime.UtcNow).Within(1).Seconds);
        }

        [Test]
        public void Should_refresh_ad_if_CheckUserId_is_false()
        {
            Thread.Sleep(3000);
            cmd.CheckUserId = false;
            cmd.UserId = null;
            Assert.That(() => Repository.Execute(cmd), Throws.Nothing);

            var ads = DataContext.AsQueryable<RPClassifiedAd>();
            Assert.That(ads, Has.Exactly(1).Items);
            var ad = ads.Single();
            Assert.That(ad, Is.EqualTo(Ad));

            Assert.That(ad.RefreshTimestamp, Is.EqualTo(DateTime.UtcNow).Within(1).Seconds);
        }

        [Test]
        public void Should_throw_if_user_does_not_own_ad()
        {
            cmd.UserId += "-";
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<RPClassifiedAdNotOwnerException>().With.Message
                    .EqualTo($"User {cmd.UserId} does not own RPClassifiedAdId {cmd.RPClassifiedAdId}").And
                    .Property("UserFriendlyError").EqualTo("You do not own this RP Classified Ad."));
        }

        [Test]
        public void Should_throw_if_ad_does_not_exist()
        {
            cmd.RPClassifiedAdId++;
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<RPClassifiedAdNotFoundException>().With.Message
                    .EqualTo($"RPClassifiedAdId with ID {cmd.RPClassifiedAdId} could not be found").And
                    .Property("UserFriendlyError").EqualTo("This RP Classified Ad doesn't exist."));
        }
    }
}
