using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;
using TT.Domain;
using TT.Domain.Commands.RPClassifiedAds;
using TT.Domain.Entities.Identity;
using TT.Domain.Entities.RPClassifiedAds;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.RPClassifiedAds;

namespace TT.Tests.Domain.Commands.RPClassifiedAds
{
    [TestFixture]
    public class CreateRPClassifiedAdTest : TestBase
    {
        private User JohnSmith;
        private CreateRPClassifiedAd cmd;

        [SetUp]
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

            DataContext.AsQueryable<RPClassifiedAd>().Count(ad =>
                ad.Id == id &&
                ad.User == JohnSmith &&
                ad.Title == cmd.Title &&
                ad.Text == cmd.Text &&
                ad.YesThemes == cmd.YesThemes &&
                ad.NoThemes == cmd.NoThemes &&
                ad.PreferredTimezones == cmd.PreferredTimezones &&
                (ad.CreationTimestamp - DateTime.UtcNow) < TimeSpan.FromSeconds(2) &&
                (ad.RefreshTimestamp - DateTime.UtcNow) < TimeSpan.FromSeconds(2)
            ).Should().Be(1);
        }

        [TestCase(4)]
        public void Should_throw_exception_if_ad_title_is_too_short(int titleLength)
        {
            cmd.Title = new string('-', titleLength);

            new Action(() => { Repository.Execute(cmd); })
                .ShouldThrow<DomainException>().WithMessage("The title is too short.");
        }

        [TestCase(36)]
        public void Should_throw_exception_if_ad_title_is_too_long(int titleLength)
        {
            cmd.Title = new string('-', titleLength);

            new Action(() => { Repository.Execute(cmd); })
                .ShouldThrow<DomainException>().WithMessage("The title is too long.");
        }

        [TestCase(49)]
        public void Should_throw_exception_if_ad_description_is_too_short(int textLength)
        {
            cmd.Text = new string('-', textLength);

            new Action(() => { Repository.Execute(cmd); })
                .ShouldThrow<DomainException>().WithMessage("The description is too short.");
        }

        [TestCase(301)]
        public void Should_throw_exception_if_ad_description_is_too_long(int textLength)
        {
            cmd.Text = new string('-', textLength);

            new Action(() => { Repository.Execute(cmd); })
                .ShouldThrow<DomainException>().WithMessage("The description is too long.");
        }

        [TestCase(201)]
        public void Should_throw_exception_if_YesThemes_is_too_long(int yesThemesLength)
        {
            cmd.YesThemes = new string('-', yesThemesLength);

            new Action(() => { Repository.Execute(cmd); })
                .ShouldThrow<DomainException>().WithMessage("The desired themes field is too long.");
        }

        [TestCase(201)]
        public void Should_throw_exception_if_NoThemes_is_too_long(int noThemesLength)
        {
            cmd.NoThemes = new string('-', noThemesLength);

            new Action(() => { Repository.Execute(cmd); })
                .ShouldThrow<DomainException>().WithMessage("The undesired themes field is too long.");
        }

        [TestCase(71)]
        public void Should_throw_exception_if_PreferredTimezones_is_too_long(int preferredTimezonesLength)
        {
            cmd.PreferredTimezones = new string('-', preferredTimezonesLength);

            new Action(() => { Repository.Execute(cmd); })
                .ShouldThrow<DomainException>().WithMessage("The preferred timezones field is too long.");
        }

        [TestCase("2")]
        public void Should_throw_exception_if_user_does_not_exist(string userId)
        {
            cmd.UserId = userId;

            new Action(() => Repository.Execute(cmd))
                .ShouldThrow<DomainException>().WithMessage(string.Format("User with ID {0} could not be found.", userId));
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
            new Action(() => Repository.Execute(cmd))
                .ShouldThrow<DomainException>().WithMessage(string.Format("User with ID {0} can not create any more ads.", JohnSmith.Id));
        }
    }
}
