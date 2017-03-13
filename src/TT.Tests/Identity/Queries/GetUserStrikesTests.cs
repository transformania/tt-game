using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Identity.Queries;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Identity.Queries
{
    [TestFixture]
    public class GetUserStrikesTests : TestBase
    {
        [Test]
        public void get_all_user_strikes()
        {

            var user = new UserBuilder()
                .With(u => u.Id, "userId")
                .BuildAndSave();

            var moderator = new UserBuilder()
                .With(u => u.Id, "moderatorId")
                .BuildAndSave();

            var moderator2 = new UserBuilder()
                .With(u => u.Id, "moderator2Id")
                .BuildAndSave();

            var olderStrike = new StrikeBuilder()
                .With(s => s.Id, 5)
                .With(s => s.User, user)
                .With(s => s.FromModerator, moderator)
                .With(s => s.Reason, "bad behavior")
                .With(s => s.Timestamp, DateTime.UtcNow.AddDays(-1))
                .BuildAndSave();

            var newerStrike = new StrikeBuilder()
                .With(s => s.Id, 13)
                .With(s => s.User, user)
                .With(s => s.FromModerator, moderator2)
                .With(s => s.Reason, "more bad behavior")
                .With(s => s.Timestamp, DateTime.UtcNow.AddHours(-3))
                .BuildAndSave();

            var strikes = DomainRegistry.Repository.Find(new GetUserStrikes {UserId = user.Id}).ToArray();

            strikes.Should().HaveCount(2);

            strikes[0].Id.Should().Be(newerStrike.Id);
            strikes[0].Reason.Should().Be(newerStrike.Reason);
            strikes[0].User.Id.Should().Be(user.Id);
            strikes[0].FromModerator.Id.Should().Be(moderator2.Id);

            strikes[1].Id.Should().Be(olderStrike.Id);
            strikes[1].Reason.Should().Be(olderStrike.Reason);
            strikes[1].User.Id.Should().Be(user.Id);
            strikes[1].FromModerator.Id.Should().Be(moderator.Id);
        }
    }
}
