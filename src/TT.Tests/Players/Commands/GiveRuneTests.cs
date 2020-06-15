using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Items.Entities;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Players.Commands
{

    [TestFixture]
    public class GiveRuneTests : TestBase
    {

        private Player owner;
        private ItemSource runeSource;

        [SetUp]
        public void Init()
        {
            owner = new PlayerBuilder()
                .With(p => p.Id, 100)
                .BuildAndSave();

            runeSource = new ItemSourceBuilder()
                .With(i => i.Id, 500)
                .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                .With(i => i.FriendlyName, "Rune of Power")
                .BuildAndSave();

        }

        [Test]
        public void can_give_rune()
        {
            Assert.That(
                () => DomainRegistry.Repository.Execute(
                    new GiveRune {PlayerId = owner.Id, ItemSourceId = runeSource.Id}), Throws.Nothing);

            Assert.That(owner.Items, Has.Exactly(1).Items);
            var rune = owner.Items.First();
            Assert.That(rune.ItemSource.Id, Is.EqualTo(runeSource.Id));
            Assert.That(rune.ItemSource.FriendlyName, Is.EqualTo(runeSource.FriendlyName));
            Assert.That(rune.IsEquipped, Is.False);
            Assert.That(rune.Owner.Id, Is.EqualTo(owner.Id));
        }

        [Test]
        public void throw_exception_if_playerId_not_provided()
        {
            var cmd = new GiveRune { ItemSourceId = runeSource.Id};
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("PlayerId is required!"));
        }

        [Test]
        public void throw_exception_if_itemSourceId_not_provided()
        {
            var cmd = new GiveRune { PlayerId = owner.Id };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("ItemId is required!"));
        }

        [Test]
        public void throw_exception_if_player_not_found()
        {
            var cmd = new GiveRune { PlayerId = 555, ItemSourceId = runeSource.Id };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Player with ID '555' could not be found"));
        }

        [Test]
        public void throw_exception_if_runeSource_not_found()
        {
            var cmd = new GiveRune { PlayerId = owner.Id, ItemSourceId = 555 };
            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("ItemSource with ID '555' could not be found"));
        }

    }
}
