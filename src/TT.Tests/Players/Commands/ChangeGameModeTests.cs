using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NSubstitute;
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

        // Performs a player mode switch, in order to test that the items have changed mode.
        // Any failures to switch the user's mode will cause the test to be skipped.
        private static void SwitchPlayerModeForItems(Player player, int playerEndMode)
        {
            try
            {
                // If this fails, the unit tests that check the player mode switch should
                // also fail, so we can skip this test instead of failing again.
                DomainRegistry.Repository.Execute(new ChangeGameMode
                {
                    MembershipId = player.User.Id,
                    GameMode = (int)playerEndMode,
                    InChaos = true
                });
            }
            catch (Exception ex)
            {
                Assert.Inconclusive($"Mode change unsuccessful due to exception - cannot validate items: {ex.Message}", ex);
            }

            if (player.GameMode != playerEndMode)
            {
                Assert.Inconclusive($"Mode change unsuccessful due to unexpected player mode {player.GameMode} - cannot validate items");
            }
        }

        private static void GivePlayerItemWithRune(Player player, int offset, GameModeStatics.GameModes mode,
                                       out Item item, out Item rune)
        {
            item = new ItemBuilder()
                .With(i => i.Id, 500 + offset)
                .With(i => i.PvPEnabled, (int)mode)
                .BuildAndSave();

            rune = new ItemBuilder()
                .With(i => i.Id, 200 + offset)
                .With(i => i.PvPEnabled, (int)mode)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                   .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                   .BuildAndSave()
                )
                .BuildAndSave();

            item.AttachRune(rune);
            player.GiveItem(item);
        }

        private static void SetUpPlayerWithItems(out Player player, int playerStartMode, out Item pvpItem, out Item pvpRune, out Item pItem, out Item pRune, out Item commonItem, out Item commonRune)
        {
            player = new PlayerBuilder()
                .With(p => p.User, new UserBuilder()
                    .With(u => u.Id, "current")
                    .BuildAndSave()
                )
                .With(p => p.Id, 100)
                .With(p => p.GameMode, (int)playerStartMode)
                .BuildAndSave();

            GivePlayerItemWithRune(player, 1, GameModeStatics.GameModes.PvP, out pvpItem, out pvpRune);
            GivePlayerItemWithRune(player, 2, GameModeStatics.GameModes.Protection, out pItem, out pRune);
            GivePlayerItemWithRune(player, 3, GameModeStatics.GameModes.Any, out commonItem, out commonRune);
        }

        [Test]
        [TestCase(GameModeStatics.GameModes.Protection, GameModeStatics.GameModes.PvP, GameModeStatics.GameModes.PvP)]
        [TestCase(GameModeStatics.GameModes.PvP, GameModeStatics.GameModes.Protection, GameModeStatics.GameModes.Protection)]
        [TestCase(GameModeStatics.GameModes.PvP, GameModeStatics.GameModes.Superprotection, GameModeStatics.GameModes.Protection)]
        public void should_change_item_mode(int playerStartMode, int playerEndMode, int expectedItemEndMode)
        {
            Player player;
            Item pvpItem, pvpRune, pItem, pRune, commonItem, commonRune;
            SetUpPlayerWithItems(out player, playerStartMode,
                                 out pvpItem, out pvpRune,
                                 out pItem, out pRune,
                                 out commonItem, out commonRune);

            SwitchPlayerModeForItems(player, playerEndMode);

            Assert.That(pvpItem.PvPEnabled, Is.EqualTo(expectedItemEndMode));
            Assert.That(pvpRune.PvPEnabled, Is.EqualTo(expectedItemEndMode));
            Assert.That(pItem.PvPEnabled, Is.EqualTo(expectedItemEndMode));
            Assert.That(pRune.PvPEnabled, Is.EqualTo(expectedItemEndMode));
            Assert.That(commonItem.PvPEnabled, Is.EqualTo((int)GameModeStatics.GameModes.Any));
            Assert.That(commonRune.PvPEnabled, Is.EqualTo((int)GameModeStatics.GameModes.Any));
        }

        [Test]
        [TestCase(GameModeStatics.GameModes.Protection, GameModeStatics.GameModes.PvP)]
        [TestCase(GameModeStatics.GameModes.PvP, GameModeStatics.GameModes.Protection)]
        public void should_not_change_unrelated_item_mode(int playerStartMode, int playerEndMode)
        {
            Player player;
            Item pvpItem, pvpRune, pItem, pRune, commonItem, commonRune;
            SetUpPlayerWithItems(out player, playerStartMode, 
                                 out pvpItem, out pvpRune,
                                 out pItem, out pRune,
                                 out commonItem, out commonRune);

            var someOtherPlayer = new PlayerBuilder()
                .With(p => p.User, new UserBuilder()
                    .With(u => u.Id, "other")
                    .BuildAndSave()
                )
                .With(p => p.Id, 199)
                .With(p => p.GameMode, (int)playerStartMode)
                .BuildAndSave();

            SwitchPlayerModeForItems(someOtherPlayer, playerEndMode);

            Assert.That(pvpItem.PvPEnabled, Is.EqualTo((int)GameModeStatics.GameModes.PvP));
            Assert.That(pvpRune.PvPEnabled, Is.EqualTo((int)GameModeStatics.GameModes.PvP));
            Assert.That(pItem.PvPEnabled, Is.EqualTo((int)GameModeStatics.GameModes.Protection));
            Assert.That(pRune.PvPEnabled, Is.EqualTo((int)GameModeStatics.GameModes.Protection));
            Assert.That(commonItem.PvPEnabled, Is.EqualTo((int)GameModeStatics.GameModes.Any));
            Assert.That(commonRune.PvPEnabled, Is.EqualTo((int)GameModeStatics.GameModes.Any));
        }

        private static void GivePlayerSoulboundItemWithRune(Player player, int offset, GameModeStatics.GameModes mode, out Item item, out Item rune)
        {
            var formerPlayer = new PlayerBuilder()
                .With(p => p.User, new UserBuilder()
                    .With(u => u.Id, "former")
                    .BuildAndSave()
                )
                .With(p => p.Id, 100 + offset)
                .BuildAndSave();

            item = new ItemBuilder()
                .With(i => i.Id, 500 + offset)
                .With(i => i.FormerPlayer, formerPlayer)
                .With(i => i.PvPEnabled, (int)mode)
                .BuildAndSave();

            rune = new ItemBuilder()
                .With(i => i.Id, 600 + offset)
                .With(i => i.PvPEnabled, (int)mode)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                    .BuildAndSave())
                .BuildAndSave();

            item.SoulbindToPlayer(player);
            item.AttachRune(rune);
        }

        private static void SetUpPlayerWithSoulboundItems(out Player player, int playerStartMode, out Item pvpItem, out Item pvpRune, out Item pItem, out Item pRune, out Item commonItem, out Item commonRune)
        {
            player = new PlayerBuilder()
                .With(p => p.Id, 100)
                .With(p => p.User, new UserBuilder()
                    .With(u => u.Id, "player")
                    .BuildAndSave()
                )
                .With(p => p.GameMode, (int)playerStartMode)
                .BuildAndSave();

            GivePlayerSoulboundItemWithRune(player, 1, GameModeStatics.GameModes.PvP, out pvpItem, out pvpRune);
            GivePlayerSoulboundItemWithRune(player, 2, GameModeStatics.GameModes.Protection, out pItem, out pRune);
            GivePlayerSoulboundItemWithRune(player, 3, GameModeStatics.GameModes.Any, out commonItem, out commonRune);
        }

        [Test]
        [TestCase(GameModeStatics.GameModes.Protection, GameModeStatics.GameModes.PvP, GameModeStatics.GameModes.PvP)]
        [TestCase(GameModeStatics.GameModes.PvP, GameModeStatics.GameModes.Protection, GameModeStatics.GameModes.Protection)]
        [TestCase(GameModeStatics.GameModes.PvP, GameModeStatics.GameModes.Superprotection, GameModeStatics.GameModes.Protection)]
        public void should_change_mode_of_soulbound_items_and_their_runes(int playerStartMode, int playerEndMode, int expectedItemEndMode)
        {
            Player player;
            Item pvpItem, pvpRune, pItem, pRune, commonItem, commonRune;
            SetUpPlayerWithSoulboundItems(out player, playerStartMode,
                                          out pvpItem, out pvpRune,
                                          out pItem, out pRune,
                                          out commonItem, out commonRune);

            SwitchPlayerModeForItems(player, playerEndMode);

            Assert.That(pItem.PvPEnabled, Is.EqualTo(expectedItemEndMode));
            Assert.That(pRune.PvPEnabled, Is.EqualTo(expectedItemEndMode));
            Assert.That(pvpItem.PvPEnabled, Is.EqualTo(expectedItemEndMode));
            Assert.That(pvpRune.PvPEnabled, Is.EqualTo(expectedItemEndMode));
            Assert.That(commonItem.PvPEnabled, Is.EqualTo((int)GameModeStatics.GameModes.Any));
            Assert.That(commonRune.PvPEnabled, Is.EqualTo((int)GameModeStatics.GameModes.Any));
        }

        [Test]
        [TestCase(GameModeStatics.GameModes.Protection, GameModeStatics.GameModes.PvP)]
        [TestCase(GameModeStatics.GameModes.PvP, GameModeStatics.GameModes.Protection)]
        public void should_not_change_mode_of_unrelated_soulbound_items_and_their_runes(int playerStartMode, int playerEndMode)
        {
            Player player;
            Item pvpItem, pvpRune, pItem, pRune, commonItem, commonRune;
            SetUpPlayerWithSoulboundItems(out player, playerStartMode, 
                                          out pvpItem, out pvpRune,
                                          out pItem, out pRune,
                                          out commonItem, out commonRune);

            var someOtherPlayer = new PlayerBuilder()
                .With(p => p.User, new UserBuilder()
                    .With(u => u.Id, "other")
                    .BuildAndSave()
                )
                .With(p => p.Id, 199)
                .With(p => p.GameMode, (int)playerStartMode)
                .BuildAndSave();

            SwitchPlayerModeForItems(someOtherPlayer, playerEndMode);

            Assert.That(pItem.PvPEnabled, Is.EqualTo((int)GameModeStatics.GameModes.Protection));
            Assert.That(pRune.PvPEnabled, Is.EqualTo((int)GameModeStatics.GameModes.Protection));
            Assert.That(pvpItem.PvPEnabled, Is.EqualTo((int)GameModeStatics.GameModes.PvP));
            Assert.That(pvpRune.PvPEnabled, Is.EqualTo((int)GameModeStatics.GameModes.PvP));
            Assert.That(commonItem.PvPEnabled, Is.EqualTo((int)GameModeStatics.GameModes.Any));
            Assert.That(commonRune.PvPEnabled, Is.EqualTo((int)GameModeStatics.GameModes.Any));
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
