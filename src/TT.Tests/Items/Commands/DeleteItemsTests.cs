using NUnit.Framework;
using TT.Domain.Exceptions;
using TT.Domain.Items.Commands;
using TT.Domain.Items.Entities;
using TT.Tests.Builders.Item;

namespace TT.Tests.Items.Commands
{
    [TestFixture]
    public class DeleteItemsTests : TestBase
    {
        [Test]
        public void Should_delete_Item()
        {
            new ItemBuilder().With(cr => cr.Id, 7)
                .BuildAndSave();

            var cmd = new DeleteItem {ItemId = 7};

            Assert.That(() => Repository.Execute(cmd), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<Item>(), Is.Empty);
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void Should_throw_error_when_Item_id_is_invalid(int id)
        {

            var cmd = new DeleteItem { ItemId = id };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("ItemId must be greater than 0"));
        }

        [Test]
        public void Should_throw_error_when_Item_id_is_not_found()
        {
            const int id = 1;
            var cmd = new DeleteItem { ItemId = id };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo($"Item with ID {id} was not found"));
        }
    }

}
