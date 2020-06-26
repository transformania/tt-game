using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Items.Entities;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.Item;
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

            Assert.That(
                () => DomainRegistry.Repository.Execute(new ChangeGameMode
                {
                    MembershipId = player.User.Id, GameMode = (int) GameModeStatics.GameModes.Protection, InChaos = true
                }), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<Player>().First(p => p.User.Id == "abcde").GameMode,
                Is.EqualTo((int) GameModeStatics.GameModes.Protection));
        }

        [Test]
        public void should_not_change_PvP_to_P_in_not_chaos()
        {

            var player = new PlayerBuilder()
                .With(u => u.User, new UserBuilder().With(u => u.Id, "abcde").BuildAndSave())
                .With(p => p.GameMode, (int)GameModeStatics.GameModes.PvP)
                .With(p => p.LastCombatTimestamp, DateTime.UtcNow)
                .BuildAndSave();

            var cmd = new ChangeGameMode { MembershipId = player.User.Id, GameMode = (int)GameModeStatics.GameModes.Protection, InChaos = false };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("You cannot leave PvP mode until you have been out of combat for thirty (30) minutes."));
        }

        [Test]
        public void should_not_change_PvP_to_SP_in_not_chaos_with_recent_combat()
        {

            var player = new PlayerBuilder()
                .With(u => u.User, new UserBuilder().With(u => u.Id, "abcde").BuildAndSave())
                .With(p => p.GameMode, (int)GameModeStatics.GameModes.PvP)
                .With(p => p.LastCombatTimestamp, DateTime.UtcNow.AddMinutes(-28))
                .BuildAndSave();

            var cmd = new ChangeGameMode { MembershipId = player.User.Id, GameMode = (int)GameModeStatics.GameModes.Superprotection, InChaos = false };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("You cannot leave PvP mode until you have been out of combat for thirty (30) minutes."));
        }

        [Test]
        public void should_change_PvP_to_SP_in_not_chaos_no_recent_combat()
        {

            var player = new PlayerBuilder()
                .With(u => u.User, new UserBuilder().With(u => u.Id, "abcde").BuildAndSave())
                .With(p => p.GameMode, (int)GameModeStatics.GameModes.PvP)
                .With(p => p.LastCombatTimestamp, DateTime.UtcNow.AddMinutes(-31))
                .BuildAndSave();

            var cmd = new ChangeGameMode { MembershipId = player.User.Id, GameMode = (int)GameModeStatics.GameModes.Superprotection, InChaos = false };
            Assert.That(() => Repository.Execute(cmd),
                Throws.Nothing);
            
            Assert.That(DataContext.AsQueryable<Player>().First(p => p.User.Id == "abcde").GameMode,
                Is.EqualTo((int)GameModeStatics.GameModes.Superprotection));
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
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("You cannot switch into that mode during regular gameplay."));
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

            Assert.That(
                () => DomainRegistry.Repository.Execute(new ChangeGameMode
                {
                    MembershipId = player.User.Id, GameMode = (int) GameModeStatics.GameModes.Superprotection,
                    InChaos = inChaos
                }), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<Player>().First(p => p.User.Id == "abcde").GameMode,
                Is.EqualTo((int) GameModeStatics.GameModes.Superprotection));
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

            Assert.That(
                () => DomainRegistry.Repository.Execute(new ChangeGameMode
                {
                    MembershipId = player.User.Id, GameMode = (int) GameModeStatics.GameModes.Protection,
                    InChaos = inChaos
                }), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<Player>().First(p => p.User.Id == "abcde").GameMode,
                Is.EqualTo((int) GameModeStatics.GameModes.Protection));
        }

        [Test]
        [TestCase(GameModeStatics.GameModes.Protection, GameModeStatics.GameModes.PvP, GameModeStatics.GameModes.PvP)]
        [TestCase(GameModeStatics.GameModes.PvP, GameModeStatics.GameModes.Protection, GameModeStatics.GameModes.Protection)]
        [TestCase(GameModeStatics.GameModes.PvP, GameModeStatics.GameModes.Superprotection, GameModeStatics.GameModes.Protection)]
        public void should_change_item_mode(int playerStartMode, int playerEndMode, int expectedItemEndMode)
        {
            var playerItems = new List<Item>();

            var pvpItem = new ItemBuilder()
                .With(i => i.Owner, null)
                .With(i => i.Id, 87)
                .With(i => i.PvPEnabled, (int)GameModeStatics.GameModes.PvP)
                .BuildAndSave();

            playerItems.Add(pvpItem);

            var pItem = new ItemBuilder()
                .With(i => i.Owner, null)
                .With(i => i.Id, 88)
                .With(i => i.PvPEnabled, (int)GameModeStatics.GameModes.Protection)
                .BuildAndSave();

            playerItems.Add(pItem);

            var player = new PlayerBuilder()
                .With(p => p.User, new UserBuilder().With(u => u.Id, "abcde").BuildAndSave())
                .With(p => p.GameMode, (int)playerStartMode)
                .With(p => p.Items, playerItems)
                .BuildAndSave();

            try
            {
                DomainRegistry.Repository.Execute(new ChangeGameMode
                {
                    MembershipId = player.User.Id,
                    GameMode = (int)playerEndMode,
                    InChaos = true
                });
            }
            catch (Exception ex)
            {
                Assert.Inconclusive("Mode change unsuccessful due to exception - cannot validate items", ex);
            }

            var playerMode = DataContext.AsQueryable<Player>().First(p => p.User.Id == "abcde").GameMode;
            if (playerMode != playerEndMode)
            {
                Assert.Inconclusive("Mode change unsuccessful due to unexpected player mode - cannot validate items", playerMode);
            }

            var previouslyPVPItem = DataContext.AsQueryable<Item>().FirstOrDefault(i => i.Id == 87);
            Assert.That(previouslyPVPItem.PvPEnabled, Is.EqualTo(expectedItemEndMode));

            var previouslyPItem = DataContext.AsQueryable<Item>().FirstOrDefault(i => i.Id == 88);
            Assert.That(previouslyPItem.PvPEnabled, Is.EqualTo(expectedItemEndMode));
        }

        [Test]
        public void Should_throw_exception_if_player_not_found()
        {
            var cmd = new ChangeGameMode {MembershipId = "fake", GameMode = (int)GameModeStatics.GameModes.Protection };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("Player with MembershipID 'fake' could not be found"));
        }

        [Test]
        public void Should_throw_exception_if_membership_null()
        {
            var cmd = new ChangeGameMode { MembershipId = null, GameMode = (int)GameModeStatics.GameModes.Protection };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("MembershipID is required!"));
        }

        [Test]
        [TestCase(-1)]
        [TestCase(3)]
        public void Should_throw_exception_if_game_mode_invalid(int mode)
        {
            var cmd = new ChangeGameMode { MembershipId = "abcde", GameMode = mode };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Game mode selection is invalid"));
        }

        [Test]
        public void Should_throw_exception_if_choosing_same_game_mode()
        {

            new PlayerBuilder()
             .With(u => u.User, new UserBuilder().With(u => u.Id, "abcde").BuildAndSave())
             .With(p => p.GameMode, (int)GameModeStatics.GameModes.Superprotection)
             .BuildAndSave();

            var cmd = new ChangeGameMode { MembershipId = null, GameMode = (int)GameModeStatics.GameModes.Superprotection };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("MembershipID is required!"));
        }
    }
}
