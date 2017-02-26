using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Entities;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.Players;

namespace TT.Tests.Domain.Commands.Players
{
    [TestFixture()]
    public class ChangeRPModeTests : TestBase
    {
        [Test]
        public void should_put_player_into_RP_mode_when_not()
        {

            var player = new PlayerBuilder()
                .With(u => u.User, new UserBuilder().With(u => u.Id, "abcde").BuildAndSave())
                .With(p => p.InRP, false)
                .BuildAndSave();

            DomainRegistry.Repository.Execute(new ChangeRPMode {MembershipId = player.User.Id, InRPMode = true});

            var loadedPlayer = DataContext.AsQueryable<Player>().First(p => p.User.Id == "abcde");
            loadedPlayer.InRP.Should().Be(true);
        }

        [Test]
        public void should_put_player_into_not_RP_mode_when_in()
        {

            var player = new PlayerBuilder()
                .With(u => u.User, new UserBuilder().With(u => u.Id, "abcde").BuildAndSave())
                .With(p => p.InRP, true)
                .BuildAndSave();

            DomainRegistry.Repository.Execute(new ChangeRPMode { MembershipId = player.User.Id, InRPMode = false });

            var loadedPlayer = DataContext.AsQueryable<Player>().First(p => p.User.Id == "abcde");
            loadedPlayer.InRP.Should().Be(false);
        }

        [Test]
        public void Should_throw_exception_if_player_not_found()
        {
            var cmd = new ChangeRPMode {MembershipId = "fake", InRPMode = true};
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Player with MembershipID 'fake' could not be found");
        }

        [Test]
        public void Should_throw_exception_if_membership_null()
        {
            var cmd = new ChangeRPMode { MembershipId = null, InRPMode = true };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("MembershipID is required!");
        }
    }
}
