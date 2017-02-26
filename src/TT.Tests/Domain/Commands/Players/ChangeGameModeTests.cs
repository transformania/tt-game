using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.Players;

namespace TT.Tests.Domain.Commands.Players
{
    [TestFixture]
    public class ChangeGameModeTests : TestBase
    {
        [Test]
        public void should_change_PvP_to_P_in_chaos()
        {

            var player = new PlayerBuilder()
                .With(u => u.User, new UserBuilder().With(u => u.Id, "abcde").BuildAndSave())
                .With(p => p.GameMode, GameModeStatics.PvP)
                .BuildAndSave();

            DomainRegistry.Repository.Execute(new ChangeGameMode {MembershipId = player.User.Id, GameMode = GameModeStatics.Protection, InChaos = true});

            var loadedPlayer = DataContext.AsQueryable<Player>().First(p => p.User.Id == "abcde");
            loadedPlayer.GameMode.Should().Be(GameModeStatics.Protection);
        }

        [Test]
        public void should_not_change_PvP_to_P_in_not_chaos()
        {

            var player = new PlayerBuilder()
                .With(u => u.User, new UserBuilder().With(u => u.Id, "abcde").BuildAndSave())
                .With(p => p.GameMode, GameModeStatics.PvP)
                .BuildAndSave();

            var cmd = new ChangeGameMode { MembershipId = player.User.Id, GameMode = GameModeStatics.Protection, InChaos = false };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("You cannot leave PvP mode during regular gameplay.");
        }

        [Test]
        [TestCase(GameModeStatics.SuperProtection)]
        [TestCase(GameModeStatics.Protection)]
        public void should_not_change_not_PvP_to_PvP_in_not_chaos(int mode)
        {

            var player = new PlayerBuilder()
                .With(u => u.User, new UserBuilder().With(u => u.Id, "abcde").BuildAndSave())
                .With(p => p.GameMode, mode)
                .BuildAndSave();

            var cmd = new ChangeGameMode { MembershipId = player.User.Id, GameMode = GameModeStatics.PvP, InChaos = false };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("You cannot enter PvP mode during regular gameplay.");
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void should_change_P_to_SP_anytime(bool inChaos)
        {

            var player = new PlayerBuilder()
                .With(u => u.User, new UserBuilder().With(u => u.Id, "abcde").BuildAndSave())
                .With(p => p.GameMode, GameModeStatics.Protection)
                .BuildAndSave();

            DomainRegistry.Repository.Execute(new ChangeGameMode { MembershipId = player.User.Id, GameMode = GameModeStatics.SuperProtection, InChaos = inChaos });

            var loadedPlayer = DataContext.AsQueryable<Player>().First(p => p.User.Id == "abcde");
            loadedPlayer.GameMode.Should().Be(GameModeStatics.SuperProtection);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void should_change_SP_to_P_anytime(bool inChaos)
        {

            var player = new PlayerBuilder()
                .With(u => u.User, new UserBuilder().With(u => u.Id, "abcde").BuildAndSave())
                .With(p => p.GameMode, GameModeStatics.SuperProtection)
                .BuildAndSave();

            DomainRegistry.Repository.Execute(new ChangeGameMode { MembershipId = player.User.Id, GameMode = GameModeStatics.Protection, InChaos = inChaos });

            var loadedPlayer = DataContext.AsQueryable<Player>().First(p => p.User.Id == "abcde");
            loadedPlayer.GameMode.Should().Be(GameModeStatics.Protection);
        }

        [Test]
        public void Should_throw_exception_if_player_not_found()
        {
            var cmd = new ChangeGameMode {MembershipId = "fake", GameMode = GameModeStatics.Protection};
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Player with MembershipID 'fake' could not be found");
        }

        [Test]
        public void Should_throw_exception_if_membership_null()
        {
            var cmd = new ChangeGameMode { MembershipId = null, GameMode = GameModeStatics.Protection };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("MembershipID is required!");
        }

        [Test]
        [TestCase(-1)]
        [TestCase(3)]
        public void Should_throw_exception_if_game_mode_invalid(int mode)
        {

            var cmd = new ChangeGameMode { MembershipId = "abcde", GameMode = mode };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Game mode selection is invalid");
        }

        [Test]
        public void Should_throw_exception_if_choosing_same_game_mode()
        {

            new PlayerBuilder()
             .With(u => u.User, new UserBuilder().With(u => u.Id, "abcde").BuildAndSave())
             .With(p => p.GameMode, GameModeStatics.SuperProtection)
             .BuildAndSave();

            var cmd = new ChangeGameMode { MembershipId = null, GameMode = GameModeStatics.SuperProtection };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("MembershipID is required!");
        }
    }
}
