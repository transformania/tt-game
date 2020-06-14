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
            Assert.That(() => Repository.Execute(cmd), Throws.Nothing);
            var ads = DataContext.AsQueryable<RPClassifiedAd>();
            Assert.That(ads, Has.Exactly(1).Items);
            var ad = ads.Single();
            Assert.That(ad, Is.EqualTo(Ad));

            Assert.That(ad.Title, Is.EqualTo(cmd.Title));
            Assert.That(ad.Text, Is.EqualTo(cmd.Text));
            Assert.That(ad.YesThemes, Is.EqualTo(cmd.YesThemes));
            Assert.That(ad.NoThemes, Is.EqualTo(cmd.NoThemes));
        }

        [Test]
        public void Should_refresh_timestamp()
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
        public void Should_throw_exception_if_ad_does_not_exist()
        {
            cmd.RPClassifiedAdId++;

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<RPClassifiedAdNotFoundException>().With.Message
                    .EqualTo($"RPClassifiedAd with ID {cmd.RPClassifiedAdId} could not be found").And
                    .Property("UserFriendlyError").EqualTo("This RP Classified Ad doesn't exist."));
        }

        [Test]
        public void Should_throw_exception_if_user_does_not_own_ad()
        {
            cmd.UserId += '-';

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<RPClassifiedAdNotOwnerException>().With.Message
                    .EqualTo($"User {cmd.UserId} does not own RPClassifiedAdId {cmd.RPClassifiedAdId}").And
                    .Property("UserFriendlyError").EqualTo("You do not own this RP Classified Ad."));
        }

        [Test]
        public void Should_update_and_not_check_user_if_CheckUserId_is_false()
        {
            cmd.CheckUserId = false;
            cmd.UserId += '-';

            Assert.That(() => Repository.Execute(cmd), Throws.Nothing);
            var ads = DataContext.AsQueryable<RPClassifiedAd>();
            Assert.That(ads, Has.Exactly(1).Items);
            var ad = ads.Single();
            Assert.That(ad, Is.EqualTo(Ad));

            Assert.That(ad.Title, Is.EqualTo(cmd.Title));
            Assert.That(ad.Text, Is.EqualTo(cmd.Text));
            Assert.That(ad.YesThemes, Is.EqualTo(cmd.YesThemes));
            Assert.That(ad.NoThemes, Is.EqualTo(cmd.NoThemes));
        }

        [TestCase(4)]
        public void Should_throw_exception_if_ad_title_is_too_short(int titleLength)
        {
            cmd.Title = new string('-', titleLength);

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<RPClassifiedAdInvalidInputException>().With.Message.EqualTo("The title is too short."));
        }

        [TestCase(36)]
        public void Should_throw_exception_if_ad_title_is_too_long(int titleLength)
        {
            cmd.Title = new string('-', titleLength);

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<RPClassifiedAdInvalidInputException>().With.Message.EqualTo("The title is too long."));
        }

        [TestCase(49)]
        public void Should_throw_exception_if_ad_description_is_too_short(int textLength)
        {
            cmd.Text = new string('-', textLength);

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<RPClassifiedAdInvalidInputException>().With.Message
                    .EqualTo("The description is too short."));
        }

        [TestCase(301)]
        public void Should_throw_exception_if_ad_description_is_too_long(int textLength)
        {
            cmd.Text = new string('-', textLength);

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<RPClassifiedAdInvalidInputException>().With.Message
                    .EqualTo("The description is too long."));
        }

        [TestCase(201)]
        public void Should_throw_exception_if_YesThemes_is_too_long(int yesThemesLength)
        {
            cmd.YesThemes = new string('-', yesThemesLength);

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<RPClassifiedAdInvalidInputException>().With.Message
                    .EqualTo("The desired themes field is too long."));
        }

        [TestCase(201)]
        public void Should_throw_exception_if_NoThemes_is_too_long(int noThemesLength)
        {
            cmd.NoThemes = new string('-', noThemesLength);

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<RPClassifiedAdInvalidInputException>().With.Message
                    .EqualTo("The undesired themes field is too long."));
        }

        [TestCase(71)]
        public void Should_throw_exception_if_PreferredTimezones_is_too_long(int preferredTimezonesLength)
        {
            cmd.PreferredTimezones = new string('-', preferredTimezonesLength);

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<RPClassifiedAdInvalidInputException>().With.Message
                    .EqualTo("The preferred timezones field is too long."));
        }
    }
}
