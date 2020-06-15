using System;
using System.Linq;
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

            Assert.That(strikes, Has.Exactly(2).Items);

            Assert.That(strikes[0].Id, Is.EqualTo(newerStrike.Id));
            Assert.That(strikes[0].Reason, Is.EqualTo(newerStrike.Reason));
            Assert.That(strikes[0].User.Id, Is.EqualTo(user.Id));
            Assert.That(strikes[0].FromModerator.Id, Is.EqualTo(moderator2.Id));

            Assert.That(strikes[1].Id, Is.EqualTo(olderStrike.Id));
            Assert.That(strikes[1].Reason, Is.EqualTo(olderStrike.Reason));
            Assert.That(strikes[1].User.Id, Is.EqualTo(user.Id));
            Assert.That(strikes[1].FromModerator.Id, Is.EqualTo(moderator.Id));
        }
    }
}
