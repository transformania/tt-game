using System;
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

            Assert.That(timeDifference, Is.GreaterThan(360));
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

            Assert.That(timeDifference, Is.LessThan(1));
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
                .With(i => i.TimeDropped, DateTime.UtcNow.AddHours(-8))
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Shirt)
                    .BuildAndSave())
                .BuildAndSave();

            var rune = new ItemBuilder()
                .With(i => i.IsEquipped, true)
                .With(i => i.Owner, owner)
                .With(i => i.TimeDropped, DateTime.UtcNow.AddHours(-8))
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                    .BuildAndSave())
                .BuildAndSave();

            var unembeddedRune = new ItemBuilder()
                .With(i => i.IsEquipped, false)
                .With(i => i.Owner, owner)
                .With(i => i.TimeDropped, DateTime.UtcNow.AddHours(-8))
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                    .BuildAndSave())
                .BuildAndSave();

            item.AttachRune(rune);

            item.Drop(owner);

            Assert.That(item.IsEquipped, Is.False);
            Assert.That(item.Owner, Is.Null);
            Assert.That(item.Runes, Has.Exactly(1).Items);
            Assert.That(item.dbLocationName, Is.EqualTo(owner.Location));
            Assert.That(item.TimeDropped, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);

            Assert.That(rune.IsEquipped, Is.True);
            Assert.That(rune.Owner, Is.Null);
            Assert.That(rune.EmbeddedOnItem.Id, Is.EqualTo(item.Id));
            Assert.That(rune.dbLocationName, Is.Empty);
            Assert.That(rune.TimeDropped, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);

            unembeddedRune.Drop(owner);

            Assert.That(unembeddedRune.IsEquipped, Is.False);
            Assert.That(unembeddedRune.Owner, Is.Null);
            Assert.That(unembeddedRune.dbLocationName, Is.EqualTo(owner.Location));
            Assert.That(unembeddedRune.EmbeddedOnItem, Is.Null);
            Assert.That(unembeddedRune.TimeDropped, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
        }

        [Test]
        public void ChangeOwner_transfers_runes()
        {
            var oldOwner = new PlayerBuilder()
                .With(p => p.Id, 1)
                .With(p => p.Location, "somewhere")
                .BuildAndSave();

            var newOwner = new PlayerBuilder()
                .With(p => p.Id, 1)
                .With(p => p.Location, "somewhere")
                .BuildAndSave();

            var item = new ItemBuilder()
                .With(i => i.Id, 100)
                .With(i => i.IsEquipped, false)
                .With(i => i.TimeDropped, DateTime.UtcNow.AddHours(-8))
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Shirt)
                    .BuildAndSave())
                .BuildAndSave();

            var rune = new ItemBuilder()
                .With(i => i.IsEquipped, true)
                .With(i => i.Owner, oldOwner)
                .With(i => i.TimeDropped, DateTime.UtcNow.AddHours(-8))
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                    .BuildAndSave())
                .BuildAndSave();

            item.AttachRune(rune);

            oldOwner.GiveItem(item);

            item.ChangeOwner(newOwner);

            Assert.That(item.Owner.Id, Is.EqualTo(newOwner.Id));
            Assert.That(item.Runes, Has.Exactly(1).Items);
            Assert.That(item.TimeDropped, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);

            Assert.That(rune.Owner.Id, Is.EqualTo(newOwner.Id));
            Assert.That(rune.EmbeddedOnItem.Id, Is.EqualTo(100));
            Assert.That(rune.IsEquipped, Is.True);
            Assert.That(rune.dbLocationName, Is.Empty);
            Assert.That(rune.TimeDropped, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
        }

        [Test]
        public void ChangeOwner_automatically_equips_pets_and_not_other_items()
        {

            var newOwner = new PlayerBuilder()
                .With(p => p.Id, 1)
                .With(p => p.Location, "somewhere")
                .BuildAndSave();

            var pet = new ItemBuilder()
                .With(i => i.Id, 100)
                .With(i => i.IsEquipped, false)
                .With(i => i.Owner, null)
                .With(i => i.dbLocationName, "somewhere")
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Pet)
                    .BuildAndSave())
                .BuildAndSave();

            var shirt = new ItemBuilder()
                .With(i => i.Id, 100)
                .With(i => i.IsEquipped, true)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Shirt)
                    .BuildAndSave())
                .BuildAndSave();

            pet.ChangeOwner(newOwner);
            shirt.ChangeOwner(newOwner);

            Assert.That(pet.IsEquipped, Is.True);
            Assert.That(shirt.IsEquipped, Is.False);
        }

        [Test]
        public void ChangeOwner_sets_new_game_mode()
        {
            var newOwner = new PlayerBuilder()
                .With(p => p.Id, 1)
                .With(p => p.Location, "somewhere")
                .BuildAndSave();

            var pet = new ItemBuilder()
                .With(i => i.Id, 100)
                .With(i => i.PvPEnabled, (int)GameModeStatics.GameModes.Superprotection)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Pet)
                    .BuildAndSave())
                .BuildAndSave();

            pet.ChangeOwner(newOwner, (int)GameModeStatics.GameModes.PvP);
            Assert.That(pet.PvPEnabled, Is.EqualTo((int) GameModeStatics.GameModes.PvP));
        }
    }
}