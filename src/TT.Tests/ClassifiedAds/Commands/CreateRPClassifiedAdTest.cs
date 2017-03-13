using FluentAssertions;
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
            ads.Should().HaveCount(1);
            var ad = ads.Single();

            ad.Id.Should().Be(id);
            ad.User.Should().Be(JohnSmith);
            ad.Title.Should().Be(cmd.Title);
            ad.Text.Should().Be(cmd.Text);
            ad.YesThemes.Should().Be(cmd.YesThemes);
            ad.NoThemes.Should().Be(cmd.NoThemes);
            ad.CreationTimestamp.Should().BeCloseTo(DateTime.UtcNow, precision: 1000);
            ad.RefreshTimestamp.Should().BeCloseTo(DateTime.UtcNow, precision: 1000);
        }

        [TestCase(4)]
        public void Should_throw_exception_if_ad_title_is_too_short(int titleLength)
        {
            cmd.Title = new string('-', titleLength);

            Action action = () => Repository.Execute(cmd);
            action.ShouldThrowExactly<RPClassifiedAdInvalidInputException>()
                .WithMessage("The title is too short.");
        }

        [TestCase(36)]
        public void Should_throw_exception_if_ad_title_is_too_long(int titleLength)
        {
            cmd.Title = new string('-', titleLength);

            Action action = () => Repository.Execute(cmd);
            action.ShouldThrow<RPClassifiedAdInvalidInputException>()
                .WithMessage("The title is too long.");
        }

        [TestCase(49)]
        public void Should_throw_exception_if_ad_description_is_too_short(int textLength)
        {
            cmd.Text = new string('-', textLength);

            Action action = () => Repository.Execute(cmd);
            action.ShouldThrow<RPClassifiedAdInvalidInputException>()
                .WithMessage("The description is too short.");
        }

        [TestCase(301)]
        public void Should_throw_exception_if_ad_description_is_too_long(int textLength)
        {
            cmd.Text = new string('-', textLength);

            Action action = () => Repository.Execute(cmd);
            action.ShouldThrow<RPClassifiedAdInvalidInputException>()
                .WithMessage("The description is too long.");
        }

        [TestCase(201)]
        public void Should_throw_exception_if_YesThemes_is_too_long(int yesThemesLength)
        {
            cmd.YesThemes = new string('-', yesThemesLength);

            Action action = () => Repository.Execute(cmd);
            action.ShouldThrow<RPClassifiedAdInvalidInputException>()
                .WithMessage("The desired themes field is too long.");
        }

        [TestCase(201)]
        public void Should_throw_exception_if_NoThemes_is_too_long(int noThemesLength)
        {
            cmd.NoThemes = new string('-', noThemesLength);

            Action action = () => Repository.Execute(cmd);
            action.ShouldThrow<RPClassifiedAdInvalidInputException>()
                .WithMessage("The undesired themes field is too long.");
        }

        [TestCase(71)]
        public void Should_throw_exception_if_PreferredTimezones_is_too_long(int preferredTimezonesLength)
        {
            cmd.PreferredTimezones = new string('-', preferredTimezonesLength);

            Action action = () => Repository.Execute(cmd);
            action.ShouldThrow<RPClassifiedAdInvalidInputException>()
                .WithMessage("The preferred timezones field is too long.");
        }

        [TestCase("2")]
        public void Should_throw_exception_if_user_does_not_exist(string userId)
        {
            cmd.UserId = userId;

            Action action = () => Repository.Execute(cmd);
            action.ShouldThrow<UserNotFoundException>()
                .WithMessage($"User with ID {userId} could not be found.")
                .And.UserFriendlyError.Should().Be("You don't exist.");
        }

        [TestCase(3)]
        public void Should_throw_exception_if_user_makes_too_many_ads(int adLimit)
        {
            // Populate database with the limit of ads
            for (int i = 0; i < adLimit; i++)
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
            Action action = () => Repository.Execute(cmd);
            var ex = action.ShouldThrow<RPClassifiedAdLimitException>()
                .WithMessage($"User with ID {JohnSmith.Id} can not create any more ads.")
                .And;

            ex.UserFriendlyError.Should().Be("You already have the maximum number of RP Classified Ads posted per player.");
            ex.UserFriendlySubError.Should().Be("Wait a while for old postings to get automatically deleted or delete some of your own yourself.");
        }
    }
}
