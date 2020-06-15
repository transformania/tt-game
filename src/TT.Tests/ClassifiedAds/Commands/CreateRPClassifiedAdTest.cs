using NUnit.Framework;
using System;
using System.Linq;
using TT.Domain.ClassifiedAds.Commands;
using TT.Domain.Entities.RPClassifiedAds;
using TT.Domain.Exceptions.Identity;
using TT.Domain.Exceptions.RPClassifiedAds;
using TT.Domain.Identity.Entities;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.RPClassifiedAds;

namespace TT.Tests.ClassifiedAds.Commands
{
    [Category("RPClassifiedAd Tests")]
    public class CreateRPClassifiedAdTest : TestBase
    {
        private User JohnSmith;
        private CreateRPClassifiedAd cmd;

        public override void SetUp()
        {
            base.SetUp();

            JohnSmith = new UserBuilder()
                .With(u => u.Email, "JohnSmith@example.cum")
                .With(u => u.UserName, "JohnSmith")
                .With(u => u.Id, "guid")
                .BuildAndSave();

            cmd = new CreateRPClassifiedAd
            {
                Title = "This Is a Title",
                Text = "This is some text. This is some text. This is some text.",
                YesThemes = "Everything",
                NoThemes = "rp with pictures",
                PreferredTimezones = "Anytime",
                UserId = JohnSmith.Id
            };
        }

        [Test]
        public void Should_create_new_ad()
        {
            var id = Repository.Execute(cmd);
            var ads = DataContext.AsQueryable<RPClassifiedAd>();
            Assert.That(ads, Has.Exactly(1).Items);
            var ad = ads.Single();

            Assert.That(ad.Id, Is.EqualTo(id));
            Assert.That(ad.User, Is.EqualTo(JohnSmith));
            Assert.That(ad.Title, Is.EqualTo(cmd.Title));
            Assert.That(ad.Text, Is.EqualTo(cmd.Text));
            Assert.That(ad.YesThemes, Is.EqualTo(cmd.YesThemes));
            Assert.That(ad.NoThemes, Is.EqualTo(cmd.NoThemes));
            Assert.That(ad.CreationTimestamp, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
            Assert.That(ad.RefreshTimestamp, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
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

        [TestCase("2")]
        public void Should_throw_exception_if_user_does_not_exist(string userId)
        {
            cmd.UserId = userId;

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<UserNotFoundException>().With.Message
                    .EqualTo($"User with ID {userId} could not be found.").And.Property("UserFriendlyError")
                    .EqualTo("You don't exist."));
        }

        [TestCase(3)]
        public void Should_throw_exception_if_user_makes_too_many_ads(int adLimit)
        {
            // Populate database with the limit of ads
            for (var i = 0; i < adLimit; i++)
            {
                new RPClassifiedAdBuilder()
                    .With(ad => ad.Title, "This Is a Title")
                    .With(ad => ad.Text, "This is some text. This is some text. This is some text.")
                    .With(ad => ad.YesThemes, "Everything")
                    .With(ad => ad.NoThemes, "rp with pictures")
                    .With(ad => ad.PreferredTimezones, "Anytime")
                    .With(ad => ad.User, JohnSmith)
                    .BuildAndSave();
            }

            // Should throw when trying to add one more
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<RPClassifiedAdLimitException>().With.Message
                    .EqualTo($"User with ID {JohnSmith.Id} can not create any more ads.").And
                    .Property("UserFriendlyError")
                    .EqualTo("You already have the maximum number of RP Classified Ads posted per player.").And
                    .Property("UserFriendlySubError")
                    .EqualTo(
                        "Wait a while for old postings to get automatically deleted or delete some of your own yourself."));
        }
    }
}
