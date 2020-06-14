using NUnit.Framework;
using TT.Domain;
using TT.Domain.Assets.Queries;
using TT.Domain.Exceptions;
using TT.Domain.Statics;
using TT.Tests.Builders.Assets;
using TT.Tests.Builders.Item;

namespace TT.Tests.Assets.Queries
{
    [TestFixture]
    public class GetRestockItemTests : TestBase
    {
        [Test]
        public void Should_fetch_RestockItem_by_id()
        {
            new RestockItemBuilder().With(cr => cr.Id, 7)
                .With(cr => cr.AmountBeforeRestock, 5)
                .With(cr => cr.AmountToRestockTo, 9)
                .With(cr => cr.BaseItem, new ItemSourceBuilder().With(cr => cr.Id, 35).BuildAndSave())
                .With(cr => cr.BotId, AIStatics.LindellaBotId)
                .BuildAndSave();

            var query = new GetRestockItem { RestockItemId = 7 };

            var restockItem = DomainRegistry.Repository.FindSingle(query);

            Assert.That(restockItem.Id, Is.EqualTo(7));
            Assert.That(restockItem.AmountBeforeRestock, Is.EqualTo(5));
            Assert.That(restockItem.AmountToRestockTo, Is.EqualTo(9));
            Assert.That(restockItem.BaseItem.Id, Is.EqualTo(35));
            Assert.That(restockItem.BotId, Is.EqualTo(AIStatics.LindellaBotId));
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void Should_throw_error_when_RestockItem_id_is_invalid(int id)
        {
            var query = new GetRestockItem { RestockItemId = id };

            Assert.That(() => Repository.FindSingle(query),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("RestockItem Id must be greater than 0"));
        }

        [Test]
        public void Should_return_null_if_RestockItem_is_not_found()
        {
            var query = new GetRestockItem { RestockItemId = 1 };

            Assert.That(Repository.FindSingle(query), Is.Null);
        }
    }
}