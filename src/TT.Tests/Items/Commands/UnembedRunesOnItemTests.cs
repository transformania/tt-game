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
    public class UnembedRunesOnItemTests : TestBase
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

        }

        [Test]
        public void can_unembed_a_rune()
        {

            item1.AttachRune(rune1);

            Assert.That(() => Repository.Execute(new UnbembedRunesOnItem {ItemId = item1.Id}), Throws.Nothing);

            Assert.That(rune1.Owner.Id, Is.EqualTo(player.Id));
            Assert.That(rune1.EmbeddedOnItem, Is.Null);
            Assert.That(rune1.IsEquipped, Is.False);
            Assert.That(rune1.dbLocationName, Is.Empty);
        }

        [Test]
        public void should_throw_exception_if_ItemId_not_provided()
        {
            var cmd = new UnbembedRunesOnItem();
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("ItemId is required"));
        }

        [Test]
        public void should_throw_exception_if_rune_not_found()
        {
            var cmd = new UnbembedRunesOnItem { ItemId = 999};
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Cannot find an item with id '999'"));
        }
    }
}
