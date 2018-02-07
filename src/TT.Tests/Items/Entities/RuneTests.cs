using System.Linq;
using FluentAssertions;
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

            item.CanAttachRunesToThisItemType().Should().Be(true);
        }

        [Test]
        [TestCase(PvPStatics.ItemType_DungeonArtifact)]
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

            item.CanAttachRunesToThisItemType().Should().Be(false);
        }

        [Test]
        public void can_only_attach_one_rune_for_most_items()
        {

            var item = new ItemBuilder()
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Shoes)
                    .BuildAndSave()
                ).BuildAndSave();

            item.HasRoomForRunes().Should().Be(true);
            item.AttachRune(rune);
            item.HasRoomForRunes().Should().Be(false);

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

            item.HasRoomForRunes().Should().Be(true);
            item.AttachRune(rune);
            item.HasRoomForRunes().Should().Be(true);
            item.AttachRune(rune2);
            item.HasRoomForRunes().Should().Be(false);
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

            item.Runes.Count.Should().Be(1);
            item.Runes.First().ItemSource.FriendlyName.Should().Be("Rune of Mana");

            rune.IsEquipped.Should().Be(true);
            rune.EmbeddedOnItem.Id.Should().Be(item.Id);
            rune.Owner.Id.Should().Be(owner.Id);
            rune.Owner.FirstName.Should().Be(owner.FirstName);
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

            item.IsOfHighEnoughLevelForRune(rune).Should().Be(true);
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

            item.IsOfHighEnoughLevelForRune(rune).Should().Be(false);
        }

        [Test]
        public void CanUnembedAllRunes()
        {
            var item1 = new ItemBuilder()
                .With(i => i.Id, 600)
                .With(i => i.Level, 1)
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

            item1.AttachRune(rune);
            item1.AttachRune(rune2);

            item1.RemoveRunes();
            item1.Runes.Count.Should().Be(0);
            rune.EmbeddedOnItem.Should().Be(null);
            rune.IsEquipped.Should().Be(false);
            rune2.EmbeddedOnItem.Should().Be(null);
            rune2.IsEquipped.Should().Be(false);
        }

    }
}
