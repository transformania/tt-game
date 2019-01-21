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

namespace TT.Tests.Players.Commands
{
    [TestFixture]
    public class ChangeGameModeTests : TestBase
    {
        [Test]
        public void should_change_PvP_to_P_in_chaos()
        {

            var player = new PlayerBuilder()
                .With(u => u.User, new UserBuilder().With(u => u.Id, "abcde").BuildAndSave())
                .With(p => p.GameMode, (int)GameModeStatics.GameModes.PvP)
                .BuildAndSave();

            DomainRegistry.Repository.Execute(new ChangeGameMode {MembershipId = player.User.Id, GameMode = (int)GameModeStatics.GameModes.Protection, InChaos = true});

            var loadedPlayer = DataContext.AsQueryable<Player>().First(p => p.User.Id == "abcde");
            loadedPlayer.GameMode.Should().Be((int)GameModeStatics.GameModes.Protection);
        }

        [Test]
        public void should_not_change_PvP_to_P_in_not_chaos()
        {

            var player = new PlayerBuilder()
                .With(u => u.User, new UserBuilder().With(u => u.Id, "abcde").BuildAndSave())
                .With(p => p.GameMode, (int)GameModeStatics.GameModes.PvP)
                .BuildAndSave();

            var cmd = new ChangeGameMode { MembershipId = player.User.Id, GameMode = (int)GameModeStatics.GameModes.Protection, InChaos = false };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("You cannot leave PvP mode during regular gameplay.");
        }

        [Test]
        [TestCase(GameModeStatics.GameModes.Superprotection)]
        [TestCase(GameModeStatics.GameModes.Protection)]
        public void should_not_change_not_PvP_to_PvP_in_not_chaos(int mode)
        {

            var player = new PlayerBuilder()
                .With(u => u.User, new UserBuilder().With(u => u.Id, "abcde").BuildAndSave())
                .With(p => p.GameMode, mode)
                .BuildAndSave();

            var cmd = new ChangeGameMode { MembershipId = player.User.Id, GameMode = (int)GameModeStatics.GameModes.PvP, InChaos = false };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("You cannot enter PvP mode during regular gameplay.");
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void should_change_P_to_SP_anytime(bool inChaos)
        {

            var player = new PlayerBuilder()
                .With(u => u.User, new UserBuilder().With(u => u.Id, "abcde").BuildAndSave())
                .With(p => p.GameMode, (int)GameModeStatics.GameModes.Protection)
                .BuildAndSave();

            DomainRegistry.Repository.Execute(new ChangeGameMode { MembershipId = player.User.Id, GameMode = (int)GameModeStatics.GameModes.Superprotection, InChaos = inChaos });

            var loadedPlayer = DataContext.AsQueryable<Player>().First(p => p.User.Id == "abcde");
            loadedPlayer.GameMode.Should().Be((int)GameModeStatics.GameModes.Superprotection);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void should_change_SP_to_P_anytime(bool inChaos)
        {

            var player = new PlayerBuilder()
                .With(u => u.User, new UserBuilder().With(u => u.Id, "abcde").BuildAndSave())
                .With(p => p.GameMode, (int)GameModeStatics.GameModes.Superprotection)
                .BuildAndSave();

            DomainRegistry.Repository.Execute(new ChangeGameMode { MembershipId = player.User.Id, GameMode = (int)GameModeStatics.GameModes.Protection, InChaos = inChaos });

            var loadedPlayer = DataContext.AsQueryable<Player>().First(p => p.User.Id == "abcde");
            loadedPlayer.GameMode.Should().Be((int)GameModeStatics.GameModes.Protection);
        }

        [Test]
        public void Should_throw_exception_if_player_not_found()
        {
            var cmd = new ChangeGameMode {MembershipId = "fake", GameMode = (int)GameModeStatics.GameModes.Protection };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("Player with MembershipID 'fake' could not be found");
        }

        [Test]
        public void Should_throw_exception_if_membership_null()
        {
            var cmd = new ChangeGameMode { MembershipId = null, GameMode = (int)GameModeStatics.GameModes.Protection };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("MembershipID is required!");
        }

        [Test]
        [TestCase(-1)]
        [TestCase(3)]
        public void Should_throw_exception_if_game_mode_invalid(int mode)
        {

            var cmd = new ChangeGameMode { MembershipId = "abcde", GameMode = mode };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("Game mode selection is invalid");
        }

        [Test]
        public void Should_throw_exception_if_choosing_same_game_mode()
        {

            new PlayerBuilder()
             .With(u => u.User, new UserBuilder().With(u => u.Id, "abcde").BuildAndSave())
             .With(p => p.GameMode, (int)GameModeStatics.GameModes.Superprotection)
             .BuildAndSave();

            var cmd = new ChangeGameMode { MembershipId = null, GameMode = (int)GameModeStatics.GameModes.Superprotection };
            var action = new Action(() => { Repository.Execute(cmd); });

            action.Should().ThrowExactly<DomainException>().WithMessage("MembershipID is required!");
        }
    }
}
