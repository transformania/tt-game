using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TT.Domain.Exceptions;
using TT.Domain.Items.Commands;
using TT.Domain.Items.Entities;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Items.Commands
{
    [TestFixture]
    public class EmbedRuneTests : TestBase
    {
        private Player player;
        private Item item;
        private Item rune;


        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            player = new PlayerBuilder()
                .With(p => p.Id, 100)
                .BuildAndSave();

            item = new ItemBuilder()
                .With(i => i.Id, 100)
                .With(i => i.Level, 15)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Shirt)
                    .With(i => i.FriendlyName, "Red T-Shirt")
                    .BuildAndSave())
                .With(i => i.Owner, player)
                .With(i => i.Runes, new List<Item>())
                .BuildAndSave();

            rune = new ItemBuilder()
                .With(i => i.Id, 200)
                .With(i => i.Level, 3)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                    .With(i => i.FriendlyName, "Rune of Leetness")
                    .With(i => i.RuneLevel, 10)
                    .BuildAndSave()
                )
                .With(i => i.Owner, player)
                .BuildAndSave();

        }

        [Test]
        public void can_embed_a_rune()
        {

            var cmd = new EmbedRune
            {
                ItemId = item.Id,
                RuneId = rune.Id,
                PlayerId = player.Id
            };

            Assert.That(Repository.Execute(cmd), Is.EqualTo("You attached your Rune of Leetness onto your Red T-Shirt."));
            Assert.That(item.Runes, Has.Exactly(1).Items);
            Assert.That(item.Runes.First().EmbeddedOnItem.Id, Is.EqualTo(item.Id));
        }

        [Test]
        public void should_throw_exception_if_player_id_not_defined()
        {
            var cmd = new EmbedRune {  RuneId = 100, ItemId = 250};
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("PlayerId is required"));
        }

        [Test]
        public void should_throw_exception_if_item_id_not_defined()
        {
            var cmd = new EmbedRune { PlayerId = 12, RuneId = 100 };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("ItemId is required"));
        }

        [Test]
        public void should_throw_exception_if_rune_id_not_defined()
        {
            var cmd = new EmbedRune { PlayerId = 12, ItemId = 250 };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("RuneId is required"));
        }

        [Test]
        public void should_throw_exception_if_player_not_found()
        {

            var cmd = new EmbedRune { ItemId = item.Id, RuneId = rune.Id, PlayerId = 999 };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("You do not own the item you are attempting to embed runes on."));
        }

        [Test]
        public void should_throw_exception_if_item_not_found()
        {

            var cmd = new EmbedRune
            {
                ItemId = 999,
                RuneId = rune.Id,
                PlayerId = player.Id
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Item with ID '999' could not be found"));
        }

        [Test]
        public void should_throw_exception_if_rune_not_found()
        {

            var cmd = new EmbedRune
            {
                ItemId = item.Id,
                RuneId = 999,
                PlayerId = player.Id
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Rune with ID '100' could not be found"));
        }

        [Test]
        public void should_throw_exception_if_rune_not_rune_item_type()
        {

            rune = new ItemBuilder()
                .With(i => i.Id, 500)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Pants)
                    .BuildAndSave()
                )
                .With(i => i.Owner, player)
                .BuildAndSave();

            var cmd = new EmbedRune { ItemId = item.Id, RuneId = rune.Id, PlayerId = player.Id };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Only runes can be embedded on items."));
        }

        [Test]
        public void should_throw_exception_if_item_not_attachable()
        {

            item = new ItemBuilder()
                .With(i => i.Id, 500)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Consumable)
                    .BuildAndSave()
                )
                .With(i => i.Owner, player)
                .BuildAndSave();

            var cmd = new EmbedRune{ItemId = item.Id, RuneId = rune.Id, PlayerId = player.Id };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("You cannot embed a rune on this item type."));
        }

        [Test]
        public void should_throw_exception_if_item_insufficent_level()
        {

            item = new ItemBuilder()
                .With(i => i.Id, 500)
                .With(i => i.Level, 7)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Pants)
                    .BuildAndSave()
                )
                .With(i => i.Owner, player)
                .BuildAndSave();


            var cmd = new EmbedRune { ItemId = item.Id, RuneId = rune.Id, PlayerId = player.Id };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo(
                    "This item is of too low level to attach this rune.  It is level 7 and needs to be at least level 10."));
        }

        [Test]
        public void should_throw_exception_if_no_more_room_on_item()
        {

            item = new ItemBuilder()
                .With(i => i.Id, 500)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Pants)
                    .BuildAndSave()
                )
                .With(i => i.Owner, player)
                .BuildAndSave();

            var rune2 = new ItemBuilder()
                .With(i => i.Id, 550)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                    .BuildAndSave()
                )
                .With(i => i.Owner, player)
                .BuildAndSave();

            item.AttachRune(rune2);

            var cmd = new EmbedRune { ItemId = item.Id, RuneId = rune.Id, PlayerId = player.Id };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("This item has no more room for additional runes."));
        }

        [Test]
        public void should_throw_exception_if_rune_already_equipped_this_turn()
        {

            var rune2 = new ItemBuilder()
                .With(i => i.Id, 550)
                .With(i => i.EquippedThisTurn, true)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                    .With(i => i.RuneLevel, 1)
                    .BuildAndSave()
                )
                .With(i => i.Owner, player)
                .BuildAndSave();

            var cmd = new EmbedRune { ItemId = item.Id, RuneId = rune2.Id, PlayerId = player.Id };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("This rune has already been equipped once this turn."));
        }


    }
}
