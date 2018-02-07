using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain.Items.Commands;
using TT.Domain.Items.Entities;
using TT.Domain.Statics;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Items.Entities
{
    [TestFixture]
    public class ItemTests : TestBase
    {
        [Test]
        public void bot_items_have_old_last_souled_timestamp()
        {
            var player = new PlayerBuilder()
                .With(p => p.BotId, AIStatics.PsychopathBotId)
                .BuildAndSave();

            var itemSource = new ItemSourceBuilder().BuildAndSave();

            var createItemCmd = new CreateItem();

            var item = Item.Create(player, null, itemSource, createItemCmd);

            var timeDifference = Math.Abs((item.LastSouledTimestamp - DateTime.UtcNow).TotalDays);

            timeDifference.Should().BeGreaterThan(360);
        }

        [Test]
        public void human_items_have_old_last_souled_timestamp()
        {
            var player = new PlayerBuilder()
                .With(p => p.BotId, AIStatics.ActivePlayerBotId)
                .BuildAndSave();

            var itemSource = new ItemSourceBuilder().BuildAndSave();

            var createItemCmd = new CreateItem();

            var item = Item.Create(player, null, itemSource, createItemCmd);

            var timeDifference = Math.Abs((item.LastSouledTimestamp - DateTime.UtcNow).TotalDays);

            timeDifference.Should().BeLessThan(1);
        }

        [Test]
        public void Drop_attached_runes_stay_on_objects()
        {
            var owner = new PlayerBuilder()
                .With(p => p.Location, "somewhere")
                .With(p => p.BotId, AIStatics.PsychopathBotId)
                .BuildAndSave();

            var item = new ItemBuilder()
                .With(i => i.Id, 100)
                .With(i => i.IsEquipped, false)
                .With(i => i.Owner, owner)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Shirt)
                    .BuildAndSave())
                .BuildAndSave();

            var rune = new ItemBuilder()
                .With(i => i.IsEquipped, true)
                .With(i => i.Owner, owner)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                    .BuildAndSave())
            .BuildAndSave();

            var unembeddedRune = new ItemBuilder()
                .With(i => i.IsEquipped, false)
                .With(i => i.Owner, owner)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                    .BuildAndSave())
                .BuildAndSave();

            item.AttachRune(rune);

            item.Drop(owner);

            item.IsEquipped.Should().Be(false);
            item.Owner.Should().Be(null);
            item.Runes.Count.Should().Be(1);
            item.dbLocationName.Should().Be(owner.Location);

            rune.IsEquipped.Should().Be(true);
            rune.Owner.Should().Be(null);
            rune.EmbeddedOnItem.Id.Should().Be(item.Id);
            rune.dbLocationName.Should().Be(String.Empty);

            unembeddedRune.Drop(owner);

            unembeddedRune.IsEquipped.Should().Be(false);
            unembeddedRune.Owner.Should().Be(null);
            unembeddedRune.dbLocationName.Should().Be(owner.Location);
            unembeddedRune.EmbeddedOnItem.Should().Be(null);

        }

        
    }
}