using System.Linq;
using NUnit.Framework;
using TT.Domain.Exceptions;
using TT.Domain.Items.Commands;
using TT.Domain.Items.Entities;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Items.Commands
{
    [TestFixture]
    public class UpdateItemsTests : TestBase
    {

        [Test]
        public void Should_transfer_existing_item_with_new_player()
        {

           var george = new PlayerBuilder().With(p => p.Id, 55)
                .With(p => p.FirstName, "George")
                .BuildAndSave();

            new PlayerBuilder().With(p => p.Id, 123)
                .With(p => p.FirstName, "Rupart")
                .BuildAndSave();

            new ItemBuilder().With(i => i.Id, 7)
                .With(i => i.Owner, george)
                .BuildAndSave();

            var cmdEdit = new UpdateItem { ItemId = 7, OwnerId = 123};

            Assert.That(() => Repository.Execute(cmdEdit), Throws.Nothing);

            var editedItem = DataContext.AsQueryable<Item>().FirstOrDefault(cr => cr.Id == 7);

            Assert.That(editedItem, Is.Not.Null);
            Assert.That(editedItem.Id, Is.EqualTo(7));
            Assert.That(editedItem.Owner.Id, Is.EqualTo(123));
            Assert.That(editedItem.Owner.FirstName, Is.EqualTo("Rupart"));
        }

        [Test]
        public void Should_transfer_existing_item_with_no_player()
        {

            var george = new PlayerBuilder().With(p => p.Id, 55)
                 .With(p => p.FirstName, "George")
                 .BuildAndSave();

            new ItemBuilder().With(i => i.Id, 7)
                .With(i => i.Owner, george)
                .With(i => i.dbLocationName,"")
                .BuildAndSave();

            var cmdEdit = new UpdateItem
            {
                ItemId = 7,
                OwnerId = null,
                dbLocationName = "tampa"
            };

            Assert.That(() => Repository.Execute(cmdEdit), Throws.Nothing);

            var editedItem = DataContext.AsQueryable<Item>().FirstOrDefault(cr => cr.Id == 7);

            Assert.That(editedItem.Id, Is.EqualTo(7));
            Assert.That(editedItem.Owner, Is.Null);
            Assert.That(editedItem.dbLocationName, Is.EqualTo("tampa"));
        }

        [Test]
        public void should_not_allow_update_of_non_existing_item()
        {
            var cmd = new UpdateItem
            {
                ItemId = 999,
                OwnerId = null,
                dbLocationName = "tampa"
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo($"Item with ID {999} could not be found"));
        }
    }
}
