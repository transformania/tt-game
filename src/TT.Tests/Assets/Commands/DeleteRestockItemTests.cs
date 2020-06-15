using NUnit.Framework;
using TT.Domain.Assets.Commands;
using TT.Domain.Assets.Entities;
using TT.Domain.Exceptions;
using TT.Tests.Builders.Assets;

namespace TT.Tests.Assets.Commands
{
    [TestFixture]
    public class DeleteRestockItemTests : TestBase
    {
        [Test]
        public void Should_delete_RestockItem()
        {
            new RestockItemBuilder().With(cr => cr.Id, 7)
                .BuildAndSave();

            var cmd = new DeleteRestockItem { RestockItemId = 7 };

            Assert.That(() => Repository.Execute(cmd), Throws.Nothing);
            Assert.That(DataContext.AsQueryable<RestockItem>(), Is.Empty);
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void Should_throw_error_when_RestockItem_id_is_invalid(int id)
        {
            var cmd = new DeleteRestockItem { RestockItemId = id };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("RestockItem Id must be greater than 0"));
        }

        [Test]
        public void Should_throw_error_when_RestockItem_is_not_found()
        {
            const int id = 1;
            var cmd = new DeleteRestockItem { RestockItemId = id };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo($"RestockItem with ID {id} was not found"));
        }
    }
}
