using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TT.Domain.Identity.Entities;
using TT.Domain.Items.Entities;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.TFEnergies.Entities;
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
                .With(i => i.IsEquipped, false)
                .With(i => i.Owner, player)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                    .BuildAndSave()
                )
                .BuildAndSave();

            var nonRuneItem = new ItemBuilder()
                .With(i => i.Id, 2)
                .With(i => i.IsEquipped, false)
                .With(i => i.Owner, player)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Hat)
                    .BuildAndSave()
                )
                .BuildAndSave();

            var embeddedRune = new ItemBuilder()
                .With(i => i.Id, 3)
                .With(i => i.IsEquipped, true)
                .With(i => i.Owner, player)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                    .BuildAndSave()
                )
                .With(i => i.EmbeddedOnItem, nonRuneItem)
                .BuildAndSave();

            nonRuneItem.AttachRune(embeddedRune);

            player.Items.Add(runeItem);
            player.Items.Add(nonRuneItem);

            player.DropAllItems();

            Assert.That(runeItem.Owner, Is.Null);
            Assert.That(runeItem.IsEquipped, Is.False);
            Assert.That(runeItem.dbLocationName, Is.EqualTo("street_70e9th"));

            Assert.That(nonRuneItem.Owner, Is.Null);
            Assert.That(nonRuneItem.IsEquipped, Is.False);
            Assert.That(nonRuneItem.dbLocationName, Is.EqualTo("street_70e9th"));

            Assert.That(embeddedRune.Owner, Is.Null);
            Assert.That(embeddedRune.IsEquipped, Is.True);
            Assert.That(embeddedRune.dbLocationName, Is.Empty);
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

            Assert.That(runeItem.Owner.Id, Is.EqualTo(player.Id));
            Assert.That(runeItem.IsEquipped, Is.True);
            Assert.That(runeItem.dbLocationName, Is.Null);

            Assert.That(nonRuneItem.Owner, Is.Null);
            Assert.That(nonRuneItem.IsEquipped, Is.False);
            Assert.That(nonRuneItem.dbLocationName, Is.EqualTo("street_70e9th"));
        }

        [Test]
        public void reducing_tf_energy_reduces_by_two_percent_without_buffs()
        {
            var tfEnergies = new List<TFEnergy>
            {
                new TFEnergyBuilder().With(t => t.Amount, 50).BuildAndSave()
            };

            var player = new PlayerBuilder()
                .With(p => p.Id, 50)
                .With(p => p.TFEnergies, tfEnergies)
                .BuildAndSave();

            player.CleanseTFEnergies(new BuffBox());
            Assert.That(player.TFEnergies.First().Amount, Is.EqualTo(40));
        }

        [Test]
        public void reducing_tf_energy_reduces_by_greater_percent_with_buffs()
        {
            var tfEnergies = new List<TFEnergy>
            {
                new TFEnergyBuilder().With(t => t.Amount, 100).BuildAndSave()
            };

            var player = new PlayerBuilder()
                .With(p => p.Id, 50)
                .With(p => p.TFEnergies, tfEnergies)
                .BuildAndSave();

            var buffs = new BuffBox
            {
                FromForm_CleanseExtraTFEnergyRemovalPercent = 10
            };

            player.CleanseTFEnergies(buffs);
            Assert.That(player.TFEnergies.First().Amount, Is.EqualTo(40));
        }

        [Test]
        public void players_do_generate_logs_when_cleansing()
        {
            var player = new PlayerBuilder()
                .With(p => p.Id, 50)
                .With(p => p.BotId, AIStatics.ActivePlayerBotId)
                .BuildAndSave();

            player.Cleanse(new BuffBox());
            Assert.That(player.PlayerLogs, Has.Exactly(1).Items);
        }

        [Test]
        public void bots_dont_generate_logs_when_cleansing()
        {
            var player = new PlayerBuilder()
                .With(p => p.Id, 50)
                .With(p => p.BotId, AIStatics.PsychopathBotId)
                .BuildAndSave();

            player.Cleanse(new BuffBox());
            Assert.That(player.PlayerLogs, Is.Empty);
        }

        [Test]
        public void players_do_generate_logs_when_meditating()
        {
            var player = new PlayerBuilder()
                .With(p => p.Id, 50)
                .With(p => p.BotId, AIStatics.ActivePlayerBotId)
                .BuildAndSave();

            player.Meditate(new BuffBox());
            Assert.That(player.PlayerLogs, Has.Exactly(1).Items);
        }

        [Test]
        public void bots_dont_generate_logs_when_meditating()
        {
            var player = new PlayerBuilder()
                .With(p => p.Id, 50)
                .With(p => p.BotId, AIStatics.PsychopathBotId)
                .BuildAndSave();

            player.Meditate(new BuffBox());
            Assert.That(player.PlayerLogs, Is.Empty);
        }

        [Test]
        public void should_get_xp_required_for_levelup()
        {
            var player = new PlayerBuilder()
                .With(p => p.XP, 0)
                .With(p => p.Level, 3)
                .BuildAndSave();

            Assert.That(player.GetXPNeededForLevelUp(), Is.EqualTo(200));
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
            Assert.That(player.XP, Is.EqualTo(105));
            Assert.That(player.Level, Is.EqualTo(3));
            Assert.That(player.UnusedLevelUpPerks, Is.EqualTo(0));
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
            Assert.That(player.XP, Is.EqualTo(5));
            Assert.That(player.Level, Is.EqualTo(4));
            Assert.That(player.UnusedLevelUpPerks, Is.EqualTo(1));
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

            Assert.That(player.GetCountOfItem(5), Is.EqualTo(1));
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
            Assert.That(player.Items, Has.Exactly(3).Items);
            Assert.That(player.Items.ElementAt(0).ItemSource.FriendlyName, Is.EqualTo(itemSource.FriendlyName));
            Assert.That(player.Items.ElementAt(0).dbLocationName, Is.Empty);
            Assert.That(player.Items.ElementAt(1).ItemSource.FriendlyName, Is.EqualTo(itemSource.FriendlyName));
            Assert.That(player.Items.ElementAt(1).dbLocationName, Is.Empty);
            Assert.That(player.Items.ElementAt(2).ItemSource.FriendlyName, Is.EqualTo(itemSource.FriendlyName));
            Assert.That(player.Items.ElementAt(2).dbLocationName, Is.Empty);
        }

        [Test]
        public void getMaxInventorySize_returns_number_of_items_a_player_can_carry_when_they_have_no_items()
        {
            var player = new PlayerBuilder()
                .With(i => i.Items, new List<Item>())
                .BuildAndSave();

            Assert.That(player.GetMaxInventorySize(), Is.EqualTo(6));
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
                .With(i => i.ExtraInventory, 2)
                .BuildAndSave();

            Assert.That(player.GetMaxInventorySize(), Is.EqualTo(8));
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

            Assert.That(player.IsCarryingTooMuchToMove(), Is.False);
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
                .With(i => i.ExtraInventory, -5)
                .BuildAndSave();

            Assert.That(player.IsCarryingTooMuchToMove(), Is.True);
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
                .With(i => i.ExtraInventory, -4)
                .BuildAndSave();

            Assert.That(player.IsCarryingTooMuchToMove(), Is.False);
        }

        [Test]
        public void IsInDungeon_returns_true_when_player_in_dungeon()
        {
            var player = new PlayerBuilder()
                .With(i => i.Location, "dungeon_place")
                .BuildAndSave();

            Assert.That(player.IsInDungeon(), Is.True);
        }

        [Test]
        public void IsInDungeon_returns_false_when_player_not_in_dungeon()
        {
            var player = new PlayerBuilder()
                .With(i => i.Location, LocationsStatics.STREET_200_SUNNYGLADE_DRIVE)
                .BuildAndSave();

            Assert.That(player.IsInDungeon(), Is.False);
        }

        [Test]
        public void CanMoveAsAnimal()
        {
            var stats = new List<Stat>
            {
                new StatBuilder().With(t => t.AchievementType, StatsProcedures.Stat__TimesMoved).With(t => t.Amount, 3).BuildAndSave()
            };

            var player = new PlayerBuilder()
                .With(i => i.Location, LocationsStatics.STREET_200_SUNNYGLADE_DRIVE)
                .With(p => p.User, new UserBuilder()
                    .With(u => u.Stats, stats)
                    .BuildAndSave())
                .With(p => p.Item, new ItemBuilder()
                    .With(i => i.dbLocationName, "someplace")
                    .BuildAndSave()
                )
                .BuildAndSave();

            var destinationLogs = player.MoveToAsAnimal("coffee_shop_patio");

            Assert.That(destinationLogs.SourceLocationLog,
                Is.EqualTo("John Doe (feral) left toward Carolyne's Coffee Shop (Patio)"));
            Assert.That(destinationLogs.DestinationLocationLog,
                Is.EqualTo("John Doe (feral) entered from Street: 200 Sunnyglade Drive"));

            Assert.That(player.PlayerLogs.ElementAt(0).Message,
                Is.EqualTo(
                    "You moved from <b>Street: 200 Sunnyglade Drive</b> to <b>Carolyne's Coffee Shop (Patio)</b>."));
            Assert.That(player.PlayerLogs.ElementAt(0).IsImportant, Is.False);

            Assert.That(player.Location, Is.EqualTo("coffee_shop_patio"));

            Assert.That(player.User.Stats.First(s => s.AchievementType == StatsProcedures.Stat__TimesMoved).Amount,
                Is.EqualTo(4));
        }

        [Test]
        public void CanMoveAsPlayer_NoSneak()
        {
            var stats = new List<Stat>
            {
                new StatBuilder().With(t => t.AchievementType, StatsProcedures.Stat__TimesMoved).With(t => t.Amount, 3).BuildAndSave()
            };

            var player = new PlayerBuilder()
                .With(i => i.ActionPoints, 10)
                .With(i => i.Location, LocationsStatics.STREET_200_SUNNYGLADE_DRIVE)
                .With(i => i.LastActionTimestamp, DateTime.UtcNow.AddHours(-2))
                .With( i => i.MoveActionPointDiscount, .5M)
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

            var destinationLogs = player.MoveTo("coffee_shop_patio");

            Assert.That(destinationLogs.SourceLocationLog,
                Is.EqualTo("John Doe left toward Carolyne's Coffee Shop (Patio)"));
            Assert.That(destinationLogs.DestinationLocationLog,
                Is.EqualTo("John Doe entered from Street: 200 Sunnyglade Drive"));

            Assert.That(player.PlayerLogs.ElementAt(0).Message,
                Is.EqualTo(
                    "You moved from <b>Street: 200 Sunnyglade Drive</b> to <b>Carolyne's Coffee Shop (Patio)</b>."));
            Assert.That(player.PlayerLogs.ElementAt(0).IsImportant, Is.False);

            Assert.That(player.Location, Is.EqualTo("coffee_shop_patio"));
            Assert.That(player.ActionPoints, Is.EqualTo(9.5M));

            Assert.That(player.User.Stats.First(s => s.AchievementType == StatsProcedures.Stat__TimesMoved).Amount,
                Is.EqualTo(4));

            Assert.That(player.Items.ElementAt(0).dbLocationName, Is.Empty);
            Assert.That(player.Items.ElementAt(1).dbLocationName, Is.Empty);

            Assert.That(player.LastActionTimestamp, Is.EqualTo(DateTime.UtcNow).Within(10).Seconds);
        }

        [Test]
        public void CanMoveAsPlayer_WithSneak()
        {
            var stats = new List<Stat>
            {
                new StatBuilder().With(t => t.AchievementType, StatsProcedures.Stat__TimesMoved).With(t => t.Amount, 8).BuildAndSave()
            };

            var player = new PlayerBuilder()
                .With(i => i.ActionPoints, 10)
                .With(i => i.Location, LocationsStatics.STREET_200_SUNNYGLADE_DRIVE)
                .With(i => i.SneakPercent, 100)
                .With(p => p.User, new UserBuilder()
                    .With(u => u.Stats, stats)
                    .With(u => u.Id, "bob")
                    .BuildAndSave())
                .BuildAndSave();

            var logs = player.MoveTo("coffee_shop_patio");

            Assert.That(logs.SourceLocationLog, Is.EqualTo("John Doe left toward Carolyne's Coffee Shop (Patio)"));
            Assert.That(logs.DestinationLocationLog, Is.EqualTo("John Doe entered from Street: 200 Sunnyglade Drive"));
            Assert.That(logs.ConcealmentLevel, Is.GreaterThan(0));

            Assert.That(player.PlayerLogs.ElementAt(0).Message,
                Does.StartWith(
                    "You moved from <b>Street: 200 Sunnyglade Drive</b> to <b>Carolyne's Coffee Shop (Patio)</b>. (Concealment lvl <b>"));
            Assert.That(player.PlayerLogs.ElementAt(0).IsImportant, Is.False);

            Assert.That(player.Location, Is.EqualTo("coffee_shop_patio"));
            Assert.That(player.ActionPoints, Is.EqualTo(9));

            Assert.That(player.User.Stats.First(s => s.AchievementType == StatsProcedures.Stat__TimesMoved).Amount,
                Is.EqualTo(9));

            Assert.That(player.LastActionTimestamp, Is.EqualTo(DateTime.UtcNow).Within(10).Seconds);
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

            Assert.That(player.GetCurrentCarryWeight(), Is.EqualTo(1));
        }
    }
}
