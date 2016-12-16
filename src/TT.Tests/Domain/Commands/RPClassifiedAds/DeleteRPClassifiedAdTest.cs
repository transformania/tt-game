using FluentAssertions;
using NUnit.Framework;
using System;
using TT.Domain.Commands.RPClassifiedAds;
using TT.Domain.Entities.Identity;
using TT.Domain.Entities.RPClassifiedAds;
using TT.Domain.Exceptions.RPClassifiedAds;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.RPClassifiedAds;

namespace TT.Tests.Domain.Commands.RPClassifiedAds
{
    [Category("RPClassifiedAd Tests")]
    public class DeleteRPClassifiedAdTest : TestBase
    {
        private User JohnSmith;
        private RPClassifiedAd Ad;
        private DeleteRPClassifiedAd cmd;

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

            cmd = new DeleteRPClassifiedAd() { UserId = JohnSmith.Id, RPClassifiedAdId = Ad.Id };
        }

        [Test]
        public void Should_delete_ad()
        {
            Repository.Execute(cmd);

            DataContext.AsQueryable<RPClassifiedAd>().Should().HaveCount(0);
        }

        [Test]
        public void Should_delete_if_CheckUserId_is_false()
        {
            cmd.UserId = null;
            cmd.CheckUserId = false;

            Repository.Execute(cmd);

            DataContext.AsQueryable<RPClassifiedAd>().Should().HaveCount(0);
        }

        [Test]
        public void Should_throw_if_not_owner()
        {
            cmd.UserId += "-";

            Action action = () => Repository.Execute(cmd);
            action.ShouldThrow<RPClassifiedAdNotOwnerException>()
                .WithMessage($"User {cmd.UserId} does not own RP Classified Ad {cmd.RPClassifiedAdId}")
                .And.UserFriendlyError.Should().Be("You do not own this RP Classified Ad.");
        }

        [Test]
        public void Should_throw_if_ad_does_not_exist()
        {
            cmd.RPClassifiedAdId++;

            Action action = () => Repository.Execute(cmd);
            action.ShouldThrow<RPClassifiedAdNotFoundException>()
                .WithMessage($"RPClassifiedAd with ID {cmd.RPClassifiedAdId} was not found")
                .And.UserFriendlyError.Should().Be("This RP Classified Ad doesn't exist.");
        }
    }
}
