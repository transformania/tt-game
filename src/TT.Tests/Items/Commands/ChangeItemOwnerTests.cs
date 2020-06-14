using NUnit.Framework;
using TT.Tests.Builders.Item;
using System.Linq;
using TT.Domain.Exceptions;
using TT.Domain.Items.Commands;
using TT.Domain.Items.Entities;
using TT.Domain.Players.Entities;
using TT.Tests.Builders.Players;

namespace TT.Tests.Items.Commands
{
    [TestFixture]
    public class ChangeItemOwnerTests : TestBase
    {
        private Player bob;
        private Player sam;
        private Item item;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            bob = new PlayerBuilder()
                .With(p => p.Id, 59)
                .With(p => p.FirstName, "Bob")
                .BuildAndSave();

            sam = new PlayerBuilder()
                .With(p => p.Id, 68)
                .With(p => p.FirstName, "Sam")
                .BuildAndSave();

            item = new ItemBuilder()
                .With(i => i.Owner, bob)
                .With(i => i.Id, 87)
                .BuildAndSave();
        }

        [Test]
        public void can_change_item_owner()
        {

            var cmd = new ChangeItemOwner
            {
                ItemId = item.Id,
                OwnerId = sam.Id
            };

            Assert.That(() => Repository.Execute(cmd), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<Item>().First(i => i.Id == item.Id).Owner.FirstName,
                Is.EqualTo("Sam"));
        }

        [Test]
        public void throw_exception_if_item_not_found()
        {
            var cmd = new ChangeItemOwner
            {
                ItemId = 100,
                OwnerId = sam.Id
            };

            Assert.That(() => Repository.Execute(cmd), Throws.TypeOf<DomainException>().With.Message.EqualTo("Item with ID 100 could not be found"));
        }

        [Test]
        public void throw_exception_if_player_not_found()
        {
            var cmd = new ChangeItemOwner
            {
                ItemId = item.Id,
                OwnerId = 100
            };

            Assert.That(() => Repository.Execute(cmd), Throws.TypeOf<DomainException>().With.Message.EqualTo("player with ID 100 could not be found"));
        }

    }
}
