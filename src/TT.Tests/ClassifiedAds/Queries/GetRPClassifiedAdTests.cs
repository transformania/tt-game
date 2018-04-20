using FluentAssertions;
using NUnit.Framework;
using System;
using TT.Domain.ClassifiedAds.Queries;
using TT.Domain.Entities.RPClassifiedAds;
using TT.Domain.Exceptions.RPClassifiedAds;
using TT.Domain.Identity.Entities;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.RPClassifiedAds;

namespace TT.Tests.ClassifiedAds.Queries
{
    [Category("RPClassifiedAd Tests")]
    public class GetRPClassifiedAdTests : TestBase
    {
        private User JohnSmith;
        private GetRPClassifiedAd cmd;
        private RPClassifiedAd Ad;

        public override void SetUp()
        {
            base.SetUp();

            JohnSmith = new UserBuilder()
                .With(u => u.Email, "JohnSmith@example.cum")
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

            cmd = new GetRPClassifiedAd
            {
                UserId = JohnSmith.Id,
                RPClassifiedAdId = Ad.Id
            };
        }

        [Test]
        public void Should_get_ad()
        {
            var ad = Repository.FindSingle(cmd);
            ad.Id.Should().Be(Ad.Id);
        }

        [Test]
        public void Should_not_check_Id_if_checkId_is_false()
        {
            cmd.UserId += '-';
            cmd.CheckUserId = false;
            var ad = Repository.FindSingle(cmd);
            ad.Id.Should().Be(Ad.Id);
        }

        [Test]
        public void Should_throw_if_not_owner()
        {
            cmd.UserId += '-';
            Action action = () => Repository.FindSingle(cmd);

            action.Should().ThrowExactly<RPClassifiedAdNotOwnerException>()
                .WithMessage($"User {cmd.UserId} does not own RP Classified Ad Id {Ad.Id}")
                .And.UserFriendlyError.Should().Be("You do not own this RP Classified Ad.");
        }

        [Test]
        public void Should_throw_if_ad_does_not_exist()
        {
            cmd.RPClassifiedAdId++;
            Action action = () => Repository.FindSingle(cmd);

            action.Should().ThrowExactly<RPClassifiedAdNotFoundException>()
                .WithMessage($"RPClassifiedAd with ID {cmd.RPClassifiedAdId} could not be found")
                .And.UserFriendlyError.Should().Be("This RP Classified Ad doesn't exist.");
        }
    }
}
