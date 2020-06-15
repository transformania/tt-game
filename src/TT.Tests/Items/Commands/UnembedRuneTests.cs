using System.Collections.Generic;
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
    public class UnembedRuneTests : TestBase
    {
        private Player player;
        private Item item1;
        private Item rune1;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            player = new PlayerBuilder()
                .With(p => p.Id, 100)
                .BuildAndSave();

            item1 = new ItemBuilder()
                .With(i => i.Id, 100)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Pet)
                    .BuildAndSave())
                .With(i => i.Owner, player)
                .With(i => i.Runes, new List<Item>())
                .BuildAndSave();

            rune1 = new ItemBuilder()
                .With(i => i.Id, 200)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                    .BuildAndSave()
                )
                .With(i => i.Owner, player)
                .BuildAndSave();

            item1.AttachRune(rune1);
            rune1.SetEquippedThisTurn(false);

        }

        [Test]
        public void can_unembed_a_rune()
        {

            var cmd = new UnembedRune
            {
                PlayerId = player.Id,
                ItemId = rune1.Id
            };

            Assert.That(Repository.Execute(cmd), Is.EqualTo("You unembedded your <b>Test Item Source</b>."));
            Assert.That(item1.Runes, Is.Empty);
            Assert.That(rune1.EmbeddedOnItem, Is.Null);
        }

        [Test]
        public void should_throw_exception_if_player_id_not_defined()
        {
            var cmd = new UnembedRune
            {
                ItemId = rune1.Id
            };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("PlayerId is required"));
        }

        [Test]
        public void should_throw_exception_if_item_id_not_defined()
        {
            var cmd = new UnembedRune
            {
                PlayerId = player.Id
            };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("ItemId is required"));
        }

        [Test]
        public void should_throw_exception_if_item_not_found()
        {
            var cmd = new UnembedRune
            {
                PlayerId = player.Id,
                ItemId = 999
            };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Rune with id '999' could not be found!"));
        }

        [Test]
        public void should_throw_exception_if_player_not_found()
        {
            var cmd = new UnembedRune
            {
                PlayerId = 999,
                ItemId = rune1.Id
            };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("You don't own this rune!"));
        }

        [Test]
        public void should_throw_exception_if_rune_not_on_item()
        {

            var rune2 = new ItemBuilder()
                .With(i => i.Id, 201)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                    .BuildAndSave()
                )
                .With(i => i.Owner, player)
                .BuildAndSave();

            var cmd = new UnembedRune
            {
                PlayerId = player.Id,
                ItemId = rune2.Id
            };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("This rune is not currently embdded on an item."));
        }

        [Test]
        public void should_throw_exception_if_rune_already_equipped_this_turn()
        {

            var rune2 = new ItemBuilder()
                .With(i => i.Id, 201)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                    .BuildAndSave()
                )
                .With(i => i.Owner, player)
                .With(i => i.EquippedThisTurn, true)
                .With(i => i.EmbeddedOnItem, item1)
                .BuildAndSave();

            var cmd = new UnembedRune
            {
                PlayerId = player.Id,
                ItemId = rune2.Id
            };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo("This rune was equipped this turn.  Wait until next turn to remove it."));
        }

    }
}
