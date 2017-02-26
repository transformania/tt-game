using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain.Entities.TFEnergies;
using TT.Domain.Items.Entities;
using TT.Domain.Statics;
using TT.Domain.ViewModels;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;
using TT.Tests.Builders.TFEnergies;

namespace TT.Tests.Domain.Entities
{
    public class PlayerTests : TestBase
    {

        [Test]
        public void player_should_drop_all_items()
        {
            var player = new PlayerBuilder()
                .With(p => p.Id, 50)
                .BuildAndSave();

            var item1 = new ItemBuilder()
                .With(i => i.Id, 1)
                .With(i => i.Owner.Id, 50)
                .With(i => i.IsEquipped, false)
                .BuildAndSave();

            var item2 = new ItemBuilder()
                .With(i => i.Id, 2)
                .With(i => i.Owner.Id, 50)
                .With(i => i.IsEquipped, false)
                .BuildAndSave();

            player.Items.Add(item1);
            player.Items.Add(item2);

            player.DropAllItems();

            item1.Owner.Should().BeNull();
            item1.IsEquipped.Should().BeFalse();
            item1.dbLocationName.Should().Be("street_70e9th");

            item2.Owner.Should().BeNull();
            item2.IsEquipped.Should().BeFalse();
            item2.dbLocationName.Should().Be("street_70e9th");

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

    }
}
