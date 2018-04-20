using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
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

            var cmd = new AddStrike() { UserId = user.Id, ModeratorId = moderator.Id, Reason = "Did stuff", Round = 50 };
            DomainRegistry.Repository.Execute(cmd);

            var strike = DataContext.AsQueryable<Strike>().First();
            strike.User.UserName.Should().Be(user.UserName);
            strike.FromModerator.UserName.Should().Be(moderator.UserName);
            strike.Reason.Should().Be("Did stuff");

            var playerLoaded = DataContext.AsQueryable<Player>().First();
            playerLoaded.PlayerLogs.First()
                .Message.Should()
                .Be(
                    "<b class='bad'>You have received a strike against your account from a moderator.  Reason cited: Did stuff.</b>");
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

            var cmd = new AddStrike() { UserId = user.Id, ModeratorId = moderator.Id, Reason = "Did stuff", Round = 50 };
            DomainRegistry.Repository.Execute(cmd);

            var strike = DataContext.AsQueryable<Strike>().First();
            strike.User.UserName.Should().Be(user.UserName);
            strike.FromModerator.UserName.Should().Be(moderator.UserName);
            strike.Reason.Should().Be("Did stuff");
        }

        [Test]
        public void should_throw_exception_if_user_not_found()
        {
            var moderator = new UserBuilder()
                .With(p => p.Id, "moderator")
                .With(p => p.UserName, "Frank")
                .BuildAndSave();

            var cmd = new AddStrike() { UserId = "fake", ModeratorId = moderator.Id, Reason = "Did stuff", Round = 50 };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("User with Id 'fake' could not be found");
        }

        [Test]
        public void should_throw_exception_if_moderator_not_found()
        {
            var user = new UserBuilder()
                .With(p => p.Id, "user")
                .With(p => p.UserName, "Bob")
                .BuildAndSave();

            var cmd = new AddStrike() { UserId = user.Id, ModeratorId = "fake", Reason = "Did stuff", Round = 50 };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("Moderator with Id 'fake' could not be found");
        }

        [Test]
        public void should_throw_exception_if_no_reason_provided()
        {

            var cmd = new AddStrike() { UserId = "user", ModeratorId = "fake", Round = 50 };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("Reason for strike is required");
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public void should_throw_exception_if_round_not_positive(int round)
        {

            var cmd = new AddStrike() { UserId = "user", ModeratorId = "fake", Reason = "Did stuff", Round = round };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("Round must be a positive integer greater than 0");
        }
    }
}
