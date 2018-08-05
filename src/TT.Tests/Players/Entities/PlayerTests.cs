using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain.Entities.TFEnergies;
using TT.Domain.Identity.Entities;
using TT.Domain.Items.Entities;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;
using TT.Tests.Builders.Identity;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;
using TT.Tests.Builders.TFEnergies;

namespace TT.Tests.Players.Entities
{
    public class PlayerTests : TestBase
    {

        [Test]
        public void player_should_drop_all_items()
        {
            var player = new PlayerBuilder()
                .With(p => p.Id, 50)
                .BuildAndSave();

            var runeItem = new ItemBuilder()
                .With(i => i.Id, 1)
                .With(i => i.Owner.Id, 50)
                .With(i => i.IsEquipped, false)
                .With(i => i.Owner, player)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                    .BuildAndSave()
                )
                .BuildAndSave();

            var nonRuneItem = new ItemBuilder()
                .With(i => i.Id, 2)
                .With(i => i.Owner.Id, 50)
                .With(i => i.IsEquipped, false)
                .With(i => i.Owner, player)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Hat)
                    .BuildAndSave()
                )
                .BuildAndSave();

            player.Items.Add(runeItem);
            player.Items.Add(nonRuneItem);

            player.DropAllItems();

            runeItem.Owner.Should().BeNull();
            runeItem.IsEquipped.Should().BeFalse();
            runeItem.dbLocationName.Should().Be("street_70e9th");

            nonRuneItem.Owner.Should().BeNull();
            nonRuneItem.IsEquipped.Should().BeFalse();
            nonRuneItem.dbLocationName.Should().Be("street_70e9th");

        }

        [Test]
        public void player_should_drop_all_items_ignoring_runes()
        {
            var player = new PlayerBuilder()
                .With(p => p.Id, 50)
                .BuildAndSave();

            var runeItem = new ItemBuilder()
                .With(i => i.Id, 1)
                .With(i => i.Owner.Id, 50)
                .With(i => i.IsEquipped, true)
                .With(i => i.Owner, player)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                    .BuildAndSave()
                )
                .BuildAndSave();

            var nonRuneItem = new ItemBuilder()
                .With(i => i.Id, 2)
                .With(i => i.Owner.Id, 50)
                .With(i => i.IsEquipped, false)
                .With(i => i.Owner, player)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Hat)
                    .BuildAndSave()
                )
                .BuildAndSave();

            player.Items.Add(runeItem);
            player.Items.Add(nonRuneItem);

            player.DropAllItems(true);

            runeItem.Owner.Id.Should().Be(player.Id);
            runeItem.IsEquipped.Should().Be(true);
            runeItem.dbLocationName.Should().BeNull();

            nonRuneItem.Owner.Should().BeNull();
            nonRuneItem.IsEquipped.Should().BeFalse();
            nonRuneItem.dbLocationName.Should().Be("street_70e9th");

        }

        [Test]
        public void reducing_tf_energy_reduces_by_two_percent_without_buffs()
        {

            var tfEnergies = new List<TFEnergy>()
            {
                new TFEnergyBuilder().With(t => t.Amount, 50).BuildAndSave()
            };

            var player = new PlayerBuilder()
                .With(p => p.Id, 50)
                .With(p => p.TFEnergies, tfEnergies)
                .BuildAndSave();

            player.CleanseTFEnergies(new BuffBox());
            player.TFEnergies.First().Amount.Should().Be(49);
        }

        [Test]
        public void reducing_tf_energy_reduces_by_greater_percent_with_buffs()
        {

            var tfEnergies = new List<TFEnergy>()
            {
                new TFEnergyBuilder().With(t => t.Amount, 50).BuildAndSave()
            };

            var player = new PlayerBuilder()
                .With(p => p.Id, 50)
                .With(p => p.TFEnergies, tfEnergies)
                .BuildAndSave();

            var buffs = new BuffBox();
            buffs.FromForm_CleanseExtraTFEnergyRemovalPercent = 10;

            player.CleanseTFEnergies(buffs);
            player.TFEnergies.First().Amount.Should().Be(44);
        }

        [Test]
        public void players_do_generate_logs_when_cleansing()
        {

            var player = new PlayerBuilder()
                .With(p => p.Id, 50)
                .With(p => p.BotId, AIStatics.ActivePlayerBotId)
                .BuildAndSave();

            player.Cleanse(new BuffBox());
            player.PlayerLogs.Count().Should().Be(1);
        }

        [Test]
        public void bots_dont_generate_logs_when_cleansing()
        {

            var player = new PlayerBuilder()
                .With(p => p.Id, 50)
                .With(p => p.BotId, AIStatics.PsychopathBotId)
                .BuildAndSave();

            player.Cleanse(new BuffBox());
            player.PlayerLogs.Count().Should().Be(0);
        }

        [Test]
        public void players_do_generate_logs_when_meditating()
        {

            var player = new PlayerBuilder()
                .With(p => p.Id, 50)
                .With(p => p.BotId, AIStatics.ActivePlayerBotId)
                .BuildAndSave();

            player.Meditate(new BuffBox());
            player.PlayerLogs.Count().Should().Be(1);
        }

        [Test]
        public void bots_dont_generate_logs_when_meditating()
        {

            var player = new PlayerBuilder()
                .With(p => p.Id, 50)
                .With(p => p.BotId, AIStatics.PsychopathBotId)
                .BuildAndSave();

            player.Meditate(new BuffBox());
            player.PlayerLogs.Count().Should().Be(0);
        }

        [Test]
        public void should_get_xp_required_for_levelup()
        {

            var player = new PlayerBuilder()
                .With(p => p.XP, 0)
                .With(p => p.Level, 3)
                .BuildAndSave();

            player.GetXPNeededForLevelUp().Should().Be(200);
        }

        [Test]
        public void should_give_player_xp_and_not_level_up()
        {

            var player = new PlayerBuilder()
                .With(p => p.XP, 95)
                .With(p => p.Level, 3)
                .With(p => p.UnusedLevelUpPerks, 0)
                .BuildAndSave();

            player.AddXP(10);
            player.XP.Should().Be(105);
            player.Level.Should().Be(3);
            player.UnusedLevelUpPerks.Should().Be(0);
        }

        [Test]
        public void should_give_player_xp_and_level_up()
        {

            var player = new PlayerBuilder()
                .With(p => p.XP, 195)
                .With(p => p.Level, 3)
                .With(p => p.UnusedLevelUpPerks, 0)
                .BuildAndSave();

            player.AddXP(10);
            player.XP.Should().Be(5);
            player.Level.Should().Be(4);
            player.UnusedLevelUpPerks.Should().Be(1);
        }

        [Test]
        public void should_get_count_of_item_type()
        {

            // include this item, correct type
            var item1 = new ItemBuilder()
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.Id, 5)
                    .BuildAndSave())
                .BuildAndSave();

            // exclude this item, different type
            var item2 = new ItemBuilder()
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.Id, 7)
                    .BuildAndSave())
                .BuildAndSave();

            var player = new PlayerBuilder()
               .With(i => i.Items, new List<Item>())
               .BuildAndSave();

            player.GiveItem(item1);

            player.GetCountOfItem(5).Should().Be(1);
        }

        [Test]
        public void should_give_items_of_type()
        {
            var player = new PlayerBuilder()
               .With(i => i.Items, new List<Item>())
               .BuildAndSave();

            var itemSource = new ItemSourceBuilder()
                .With(i => i.Id, 50)
                .With(i => i.FriendlyName, "Socks")
                .BuildAndSave();

            player.GiveItemsOfType(itemSource, 3);
            player.Items.Count().Should().Be(3);
            player.Items.ElementAt(0).ItemSource.FriendlyName.Should().Be(itemSource.FriendlyName);
            player.Items.ElementAt(0).dbLocationName.Should().Be("");
            player.Items.ElementAt(1).ItemSource.FriendlyName.Should().Be(itemSource.FriendlyName);
            player.Items.ElementAt(1).dbLocationName.Should().Be("");
            player.Items.ElementAt(2).ItemSource.FriendlyName.Should().Be(itemSource.FriendlyName);
            player.Items.ElementAt(2).dbLocationName.Should().Be("");
        }

        [Test]
        public void getMaxInventorySize_returns_number_of_items_a_player_can_carry_when_they_have_no_items()
        {
            var player = new PlayerBuilder()
                .With(i => i.Items, new List<Item>())
                .BuildAndSave();

            var buffs = new BuffBox();

            player.GetMaxInventorySize(buffs).Should().Be(6);
        }

        [Test]
        public void getMaxInventorySize_returns_number_of_items_a_player_can_carry_when_they_have_some_buffs()
        {

            var items = new List<Item>();

            var item1 = new ItemBuilder()
                .With(i => i.IsEquipped, false)
                .BuildAndSave();

            var item2 = new ItemBuilder()
                .With(i => i.IsEquipped, false)
                .BuildAndSave();

            items.Add(item1);
            items.Add(item2);

            var player = new PlayerBuilder()
                .With(i => i.Items, items)
                .BuildAndSave();

            var buffs = new BuffBox();
            buffs.FromForm_ExtraInventorySpace = 2;

            player.GetMaxInventorySize(buffs).Should().Be(8);
        }

        [Test]
        public void IsCarryingTooMuchToMove_should_return_false_when_player_has_okay_item_count()
        {
            var items = new List<Item>();

            var item1 = new ItemBuilder()
                .With(i => i.IsEquipped, false)
                .BuildAndSave();

            var item2 = new ItemBuilder()
                .With(i => i.IsEquipped, false)
                .BuildAndSave();

            items.Add(item1);
            items.Add(item2);

            var player = new PlayerBuilder()
                .With(i => i.Items, items)
                .BuildAndSave();

            var buffs = new BuffBox();

            player.IsCarryingTooMuchToMove(buffs).Should().Be(false);
        }

        [Test]
        public void IsCarryingTooMuchToMove_should_return_true_when_player_has_too_high_item_count()
        {
            var items = new List<Item>();

            var item1 = new ItemBuilder()
                .With(i => i.IsEquipped, false)
                .BuildAndSave();

            var item2 = new ItemBuilder()
                .With(i => i.IsEquipped, false)
                .BuildAndSave();

            items.Add(item1);
            items.Add(item2);

            var player = new PlayerBuilder()
                .With(i => i.Items, items)
                .BuildAndSave();

            var buffs = new BuffBox();
            buffs.FromForm_ExtraInventorySpace = -5;

            player.IsCarryingTooMuchToMove(buffs).Should().Be(true);
        }

        [Test]
        public void IsCarryingTooMuchToMove_should_return_false_when_player_has_maxed_out_item_count()
        {
            var items = new List<Item>();

            var item1 = new ItemBuilder()
                .With(i => i.IsEquipped, false)
                .BuildAndSave();

            var item2 = new ItemBuilder()
                .With(i => i.IsEquipped, false)
                .BuildAndSave();

            items.Add(item1);
            items.Add(item2);

            var player = new PlayerBuilder()
                .With(i => i.Items, items)
                .BuildAndSave();

            var buffs = new BuffBox();
            buffs.FromForm_ExtraInventorySpace = -4;

            player.IsCarryingTooMuchToMove(buffs).Should().Be(false);
        }

        [Test]
        public void IsInDungeon_returns_true_when_player_in_dungeon()
        {

            var player = new PlayerBuilder()
                .With(i => i.Location, "dungeon_place")
                .BuildAndSave();

            player.IsInDungeon().Should().Be(true);
        }

        [Test]
        public void IsInDungeon_returns_false_when_player_not_in_dungeon()
        {

            var player = new PlayerBuilder()
                .With(i => i.Location, LocationsStatics.STREET_200_SUNNYGLADE_DRIVE)
                .BuildAndSave();

            player.IsInDungeon().Should().Be(false);
        }

        [Test]
        public void CanMoveAsAnimal()
        {

            var stats = new List<Stat>()
            {
                new StatBuilder().With(t => t.AchievementType, StatsProcedures.Stat__TimesMoved).With(t => t.Amount, 3).BuildAndSave()
            };

            var player = new PlayerBuilder()
                .With(i => i.Location, LocationsStatics.STREET_200_SUNNYGLADE_DRIVE)
                .With(p => p.User, new UserBuilder()
                    .With(u => u.Stats, stats)
                    .BuildAndSave())
                .BuildAndSave();

            var destinationLogs = player.MoveToAsAnimal("coffee_shop_patio");

            destinationLogs.SourceLocationLog.Should().Be("John Doe (feral) left toward Carolyne's Coffee Shop (Patio)");
            destinationLogs.DestinationLocationLog.Should().Be("John Doe (feral) entered from Street: 200 Sunnyglade Drive");

            player.PlayerLogs.ElementAt(0).Message.Should().Be("You moved from <b>Street: 200 Sunnyglade Drive</b> to <b>Carolyne's Coffee Shop (Patio)</b>.");
            player.PlayerLogs.ElementAt(0).IsImportant.Should().Be(false);

            player.Location.Should().Be("coffee_shop_patio");

            player.User.Stats.FirstOrDefault(s => s.AchievementType == StatsProcedures.Stat__TimesMoved).Amount.Should()
                .Be(4);
        }

        [Test]
        public void CanMoveAsPlayer_NoSneak()
        {

            var stats = new List<Stat>()
            {
                new StatBuilder().With(t => t.AchievementType, StatsProcedures.Stat__TimesMoved).With(t => t.Amount, 3).BuildAndSave()
            };

            var player = new PlayerBuilder()
                .With(i => i.ActionPoints, 10)
                .With(i => i.Location, LocationsStatics.STREET_200_SUNNYGLADE_DRIVE)
                .With(i => i.LastActionTimestamp, DateTime.UtcNow.AddHours(-2))
                .With(p => p.User, new UserBuilder()
                    .With(u => u.Stats, stats)
                    .With(u => u.Id, "bob")
                    .BuildAndSave())
                .BuildAndSave();

            var item1 = new ItemBuilder()
                .With(i => i.Id, 1)
                .With(i => i.Owner.Id, 50)
                .With(i => i.IsEquipped, true)
                .With(i => i.dbLocationName, player.Location)
                .BuildAndSave();

            var item2 = new ItemBuilder()
                .With(i => i.Id, 2)
                .With(i => i.Owner.Id, 50)
                .With(i => i.IsEquipped, false)
                .With(i => i.dbLocationName, player.Location)
                .BuildAndSave();

            player.Items.Add(item1);
            player.Items.Add(item2);

            var buffs = new BuffBox();
            buffs.FromForm_MoveActionPointDiscount = .5M;

            var destinationLogs = player.MoveTo("coffee_shop_patio", buffs);

            destinationLogs.SourceLocationLog.Should().Be("John Doe left toward Carolyne's Coffee Shop (Patio)");
            destinationLogs.DestinationLocationLog.Should().Be("John Doe entered from Street: 200 Sunnyglade Drive");

            player.PlayerLogs.ElementAt(0).Message.Should().Be("You moved from <b>Street: 200 Sunnyglade Drive</b> to <b>Carolyne's Coffee Shop (Patio)</b>.");
            player.PlayerLogs.ElementAt(0).IsImportant.Should().Be(false);

            player.Location.Should().Be("coffee_shop_patio");
            player.ActionPoints.Should().Be(9.5M);

            player.User.Stats.FirstOrDefault(s => s.AchievementType == StatsProcedures.Stat__TimesMoved).Amount.Should()
                .Be(4);

            player.Items.ElementAt(0).dbLocationName.Should().Be(String.Empty);
            player.Items.ElementAt(1).dbLocationName.Should().Be(String.Empty);

            player.LastActionTimestamp.Should().BeCloseTo(DateTime.UtcNow, 10000);
        }

        [Test]
        public void CanMoveAsPlayer_WithSneak()
        {

            var stats = new List<Stat>()
            {
                new StatBuilder().With(t => t.AchievementType, StatsProcedures.Stat__TimesMoved).With(t => t.Amount, 8).BuildAndSave()
            };

            var player = new PlayerBuilder()
                .With(i => i.ActionPoints, 10)
                .With(i => i.Location, LocationsStatics.STREET_200_SUNNYGLADE_DRIVE)
                .With(p => p.User, new UserBuilder()
                    .With(u => u.Stats, stats)
                    .With(u => u.Id, "bob")
                    .BuildAndSave())
                .BuildAndSave();

            var buffs = new BuffBox();
            buffs.FromForm_SneakPercent = 100;

            var logs = player.MoveTo("coffee_shop_patio", buffs);

            logs.SourceLocationLog.Should().Be("John Doe left toward Carolyne's Coffee Shop (Patio)");
            logs.DestinationLocationLog.Should().Be("John Doe entered from Street: 200 Sunnyglade Drive");
            logs.ConcealmentLevel.Should().BeGreaterThan(0);

            player.PlayerLogs.ElementAt(0).Message.Should().Contain("You moved from <b>Street: 200 Sunnyglade Drive</b> to <b>Carolyne's Coffee Shop (Patio)</b>. (Concealment lvl <b>");
            player.PlayerLogs.ElementAt(0).IsImportant.Should().Be(false);

            player.Location.Should().Be("coffee_shop_patio");
            player.ActionPoints.Should().Be(9);

            player.User.Stats.FirstOrDefault(s => s.AchievementType == StatsProcedures.Stat__TimesMoved).Amount.Should()
                .Be(9);

            player.LastActionTimestamp.Should().BeCloseTo(DateTime.UtcNow, 10000);

        }

        [Test]
        public void GetCurrentCarryWeight_returns_correct_weight_for_runes()
        {
            var player = new PlayerBuilder()
                .With(p => p.Id, 50)
                .BuildAndSave();

            var shirt = new ItemBuilder()
                .With(i => i.Owner.Id, 50)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Shirt)
                .BuildAndSave()
                ).With(i => i.IsEquipped, true)
                .BuildAndSave();

            var pants = new ItemBuilder()
                .With(i => i.Owner.Id, 50)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Pants)
                    .BuildAndSave()
                ).With(i => i.IsEquipped, false)
                .BuildAndSave();

            var runeOnWornItem = new ItemBuilder()
                .With(i => i.Owner.Id, 50)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                    .BuildAndSave()
                ).With(i => i.IsEquipped, false)
                .BuildAndSave();

            var runeOnCarriedItem = new ItemBuilder()
                .With(i => i.Owner.Id, 50)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                    .BuildAndSave()
                ).With(i => i.IsEquipped, false)
                .BuildAndSave();

            shirt.AttachRune(runeOnWornItem);
            pants.AttachRune(runeOnCarriedItem);

            player.Items.Add(shirt);
            player.Items.Add(pants);

            player.GetCurrentCarryWeight().Should().Be(1);

        }

    }
}
