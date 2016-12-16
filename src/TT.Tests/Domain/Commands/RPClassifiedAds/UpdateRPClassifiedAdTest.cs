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
        public void Should_refresh_timestamp()
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
        public void Should_throw_exception_if_ad_does_not_exist()
        {
            cmd.RPClassifiedAdId++;

            Action action = () => Repository.Execute(cmd);
            action.ShouldThrowExactly<RPClassifiedAdNotFoundException>()
                .WithMessage($"RPClassifiedAd with ID {cmd.RPClassifiedAdId} could not be found")
                .And.UserFriendlyError.Should().Be("This RP Classified Ad doesn't exist.");
        }

        [Test]
        public void Should_throw_exception_if_user_does_not_own_ad()
        {
            cmd.UserId += '-';

            Action action = () => Repository.Execute(cmd);
            action.ShouldThrowExactly<RPClassifiedAdNotOwnerException>()
                .WithMessage($"User {cmd.UserId} does not own RPClassifiedAdId {cmd.RPClassifiedAdId}")
                .And.UserFriendlyError.Should().Be("You do not own this RP Classified Ad.");
        }

        [Test]
        public void Should_update_and_not_check_user_if_CheckUserId_is_false()
        {
            cmd.CheckUserId = false;
            cmd.UserId += '-';
            
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
    }
}
