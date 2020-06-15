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
    public class UnembedAllRunesTests : TestBase
    {
        private Player player;
        private Item item1;
        private Item item2;
        private Item rune1;
        private Item rune2;
        private Item rune3;


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

            item2 = new ItemBuilder()
                .With(i => i.Id, 115)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Shirt)
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

            rune2 = new ItemBuilder()
                .With(i => i.Id, 201)
                .With(i => i.ItemSource, new ItemSourceBuilder()
                    .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                    .BuildAndSave()
                )
                .With(i => i.Owner, player)
                .BuildAndSave();

            rune3 = new ItemBuilder()
                .With(i => i.Id, 202)
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
            item1.AttachRune(rune2);
            item2.AttachRune(rune3);

            var cmd = new UnembedAllRunes
            {
                PlayerId = player.Id
            };

            Assert.That(Repository.Execute(cmd), Is.EqualTo("You removed the runes from 2 of your belongings."));

            Assert.That(item1.Runes, Is.Empty);
            Assert.That(item2.Runes, Is.Empty);

            Assert.That(rune1.EmbeddedOnItem, Is.Null);
            Assert.That(rune2.EmbeddedOnItem, Is.Null);
            Assert.That(rune3.EmbeddedOnItem, Is.Null);
        }

        [Test]
        public void should_throw_exception_if_player_id_not_defined()
        {
            var cmd = new UnembedAllRunes();
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("PlayerId is required"));
        }
    }
}
