using System;
using System.Linq;
using NUnit.Framework;
using TT.Domain.Items.Entities;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Items.Entities
{
    [TestFixture]
    public class RuneTests : TestBase
    {
        private Item rune;
        private Player owner;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            owner = new PlayerBuilder()
                .With(p => p.Id, 999)
                .With(p => p.FirstName, "Bob")
                .BuildAndSave();

            rune = new ItemBuilder()
                .With(i => i.Id, 500)
                .With(i => i.IsEquipped, false)
                .With(i => i.Owner, owner)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                    .With(i => i.FriendlyName, "Rune of Mana")
                    .With(i => i.RuneLevel, 7)
                    .BuildAndSave())
                .BuildAndSave();

        }

        [Test]
        [TestCase(PvPStatics.ItemType_Shirt)]
        [TestCase(PvPStatics.ItemType_Accessory)]
        [TestCase(PvPStatics.ItemType_Hat)]
        [TestCase(PvPStatics.ItemType_Pants)]
        [TestCase(PvPStatics.ItemType_Pet)]
        [TestCase(PvPStatics.ItemType_Shoes)]
        [TestCase(PvPStatics.ItemType_Undershirt)]
        [TestCase(PvPStatics.ItemType_Underpants)]
        public void can_attach_runes_on_types(string itemType)
        {
            var item = new ItemBuilder()
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, itemType)
                    .BuildAndSave()
                ).BuildAndSave();
            
            Assert.That(item.CanAttachRunesToThisItemType(), Is.True);
        }

        [Test]
        [TestCase(PvPStatics.ItemType_Consumable)]
        [TestCase(PvPStatics.ItemType_Consumable_Reuseable)]
        [TestCase(PvPStatics.ItemType_Rune)]
        public void can_not_attach_runes_on_types(string itemType)
        {
            var item = new ItemBuilder()
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, itemType)
                    .BuildAndSave()
                ).BuildAndSave();

            Assert.That(item.CanAttachRunesToThisItemType(), Is.False);
        }

        [Test]
        public void can_only_attach_one_rune_for_most_items()
        {

            var item = new ItemBuilder()
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Shoes)
                    .BuildAndSave()
                ).BuildAndSave();

            Assert.That(item.HasRoomForRunes(), Is.True);
            item.AttachRune(rune);
            Assert.That(item.HasRoomForRunes(), Is.False);
        }

        [Test]
        public void can_only_attach_two_runes_for_pets()
        {

            var rune2 = new ItemBuilder()
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                    .BuildAndSave())
                .BuildAndSave();

            var item = new ItemBuilder()
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Pet)
                    .BuildAndSave()
                ).BuildAndSave();

            Assert.That(item.HasRoomForRunes(), Is.True);
            item.AttachRune(rune);
            Assert.That(item.HasRoomForRunes(), Is.True);
            item.AttachRune(rune2);
            Assert.That(item.HasRoomForRunes(), Is.False);
        }

        [Test]
        public void can_equip_rune()
        {
            var item = new ItemBuilder()
                .With(i => i.Id, 600)
                .With(i => i.Level, 10)
                .With(i => i.Owner, owner)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Pet)
                    .BuildAndSave()
                ).BuildAndSave();

            item.AttachRune(rune);

            Assert.That(item.Runes, Has.Exactly(1).Items);
            Assert.That(item.Runes.First().ItemSource.FriendlyName, Is.EqualTo("Rune of Mana"));

            Assert.That(rune.IsEquipped, Is.True);
            Assert.That(rune.EmbeddedOnItem.Id, Is.EqualTo(item.Id));
            Assert.That(rune.Owner.Id, Is.EqualTo(owner.Id));
            Assert.That(rune.Owner.FirstName, Is.EqualTo(owner.FirstName));
            Assert.That(rune.EquippedThisTurn, Is.EqualTo(true));
        }

        [Test]
        public void IsOfHighEnoughLevelForRune_returns_true_when_item_is_high_enough_level()
        {
            var item = new ItemBuilder()
                .With(i => i.Id, 600)
                .With(i => i.Level, 10)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Pet)
                    .BuildAndSave()
                ).BuildAndSave();

            Assert.That(item.IsOfHighEnoughLevelForRune(rune), Is.True);
        }

        [Test]
        public void IsOfHighEnoughLevelForRune_returns_false_when_item_is_high_enough_level()
        {
            var item = new ItemBuilder()
                .With(i => i.Id, 600)
                .With(i => i.Level, 1)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Pet)
                    .BuildAndSave()
                ).BuildAndSave();

            Assert.That(item.IsOfHighEnoughLevelForRune(rune), Is.False);
        }

        [Test]
        public void can_unembed_runes_on_unowned_items()
        {
            var unownedItem = new ItemBuilder()
                .With(i => i.Id, 600)
                .With(i => i.Level, 1)
                .With(i => i.Owner, null)
                .With(i => i.dbLocationName, "somewhere")
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Pet)
                    .BuildAndSave()
                ).BuildAndSave();

            var rune2 = new ItemBuilder()
                .With(i => i.Id, 1000)
                .With(i => i.IsEquipped, false)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                    .BuildAndSave())
                .BuildAndSave();

            unownedItem.AttachRune(rune);
            unownedItem.AttachRune(rune2);

            unownedItem.RemoveRunes();
            Assert.That(unownedItem.Runes, Is.Empty);

            Assert.That(rune.EmbeddedOnItem, Is.Null);
            Assert.That(rune.IsEquipped, Is.False);
            Assert.That(rune.Owner, Is.Null);
            Assert.That(rune.dbLocationName, Is.EqualTo("somewhere"));
            Assert.That(rune.EquippedThisTurn, Is.True);

            Assert.That(rune2.EmbeddedOnItem, Is.Null);
            Assert.That(rune2.IsEquipped, Is.False);
            Assert.That(rune2.Owner, Is.Null);
            Assert.That(rune2.dbLocationName, Is.EqualTo("somewhere"));
            Assert.That(rune2.EquippedThisTurn, Is.True);
        }

        [Test]
        public void can_unembed_runes_on_owned_items()
        {

            var owner = new PlayerBuilder()
                .With(i => i.Id, 1010)
                .With(i => i.Location, "somewhere")
                .BuildAndSave();

            var ownedItem = new ItemBuilder()
                .With(i => i.Id, 600)
                .With(i => i.Level, 1)
                .With(i => i.Owner, owner)
                .With(i => i.dbLocationName, String.Empty)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Pet)
                    .BuildAndSave()
                ).BuildAndSave();

            var rune2 = new ItemBuilder()
                .With(i => i.Id, 1000)
                .With(i => i.IsEquipped, false)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                    .BuildAndSave())
                .BuildAndSave();

            ownedItem.AttachRune(rune);
            ownedItem.AttachRune(rune2);

            owner.GiveItem(ownedItem);

            ownedItem.RemoveRunes();
            Assert.That(ownedItem.Runes, Is.Empty);

            Assert.That(rune.EmbeddedOnItem, Is.Null);
            Assert.That(rune.IsEquipped, Is.False);
            Assert.That(rune.Owner.Id, Is.EqualTo(owner.Id));
            Assert.That(rune.dbLocationName, Is.Empty);

            Assert.That(rune2.EmbeddedOnItem, Is.Null);
            Assert.That(rune2.IsEquipped, Is.False);
            Assert.That(rune2.Owner.Id, Is.EqualTo(owner.Id));
            Assert.That(rune2.dbLocationName, Is.Empty);
        }

    }
}
