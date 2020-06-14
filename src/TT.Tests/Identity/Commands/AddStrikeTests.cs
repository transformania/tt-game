using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Commands;
using TT.Domain.Identity.Entities;
using TT.Domain.Players.Entities;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.Players;

namespace TT.Tests.Identity.Commands
{

    [TestFixture]
    public class AddStrikeTests : TestBase
    {

        [Test]
        public void can_add_strike_when_player_exists()
        {
            var user = new UserBuilder()
                .With(p => p.Id, "user")
                .With(p => p.UserName, "Bob")
                .BuildAndSave();

            new PlayerBuilder()
                .With(p => p.PlayerLogs, new List<PlayerLog>())
                .With(p => p.User, user)
                .BuildAndSave();

            var moderator = new UserBuilder()
                .With(p => p.Id, "moderator")
                .With(p => p.UserName, "Frank")
                .BuildAndSave();

            var cmd = new AddStrike { UserId = user.Id, ModeratorId = moderator.Id, Reason = "Did stuff", Round = 50 };
            Assert.That(() => DomainRegistry.Repository.Execute(cmd), Throws.Nothing);

            var strike = DataContext.AsQueryable<Strike>().FirstOrDefault();
            Assert.That(strike, Is.Not.Null);
            Assert.That(strike.User.UserName, Is.EqualTo(user.UserName));
            Assert.That(strike.FromModerator.UserName, Is.EqualTo(moderator.UserName));
            Assert.That(strike.Reason, Is.EqualTo("Did stuff"));

            var playerLoaded = DataContext.AsQueryable<Player>().FirstOrDefault();
            Assert.That(playerLoaded, Is.Not.Null);
            Assert.That(playerLoaded.PlayerLogs.First().Message,
                Is.EqualTo(
                    "<b class='bad'>You have received a strike against your account from a moderator.  Reason cited: Did stuff.</b>"));
        }

        [Test]
        public void can_add_strike_when_player_does_not_exist()
        {
            var user = new UserBuilder()
                .With(p => p.Id, "user")
                .With(p => p.UserName, "Bob")
                .BuildAndSave();

            var moderator = new UserBuilder()
                .With(p => p.Id, "moderator")
                .With(p => p.UserName, "Frank")
                .BuildAndSave();

            var cmd = new AddStrike { UserId = user.Id, ModeratorId = moderator.Id, Reason = "Did stuff", Round = 50 };
            DomainRegistry.Repository.Execute(cmd);

            var strike = DataContext.AsQueryable<Strike>().FirstOrDefault();
            Assert.That(strike, Is.Not.Null);
            Assert.That(strike.User.UserName, Is.EqualTo(user.UserName));
            Assert.That(strike.FromModerator.UserName, Is.EqualTo(moderator.UserName));
            Assert.That(strike.Reason, Is.EqualTo("Did stuff"));
        }

        [Test]
        public void should_throw_exception_if_user_not_found()
        {
            var moderator = new UserBuilder()
                .With(p => p.Id, "moderator")
                .With(p => p.UserName, "Frank")
                .BuildAndSave();

            var cmd = new AddStrike { UserId = "fake", ModeratorId = moderator.Id, Reason = "Did stuff", Round = 50 };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("User with Id 'fake' could not be found"));
        }

        [Test]
        public void should_throw_exception_if_moderator_not_found()
        {
            var user = new UserBuilder()
                .With(p => p.Id, "user")
                .With(p => p.UserName, "Bob")
                .BuildAndSave();

            var cmd = new AddStrike { UserId = user.Id, ModeratorId = "fake", Reason = "Did stuff", Round = 50 };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Moderator with Id 'fake' could not be found"));
        }

        [Test]
        public void should_throw_exception_if_no_reason_provided()
        {

            var cmd = new AddStrike { UserId = "user", ModeratorId = "fake", Round = 50 };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Reason for strike is required"));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public void should_throw_exception_if_round_not_positive(int round)
        {

            var cmd = new AddStrike { UserId = "user", ModeratorId = "fake", Reason = "Did stuff", Round = round };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("Round must be a positive integer greater than 0"));
        }
    }
}
