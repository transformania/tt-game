using NUnit.Framework;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.AI;
using TT.Tests.Builders.Assets;
using System.Linq;
using TT.Domain.Assets.Commands;
using TT.Domain.Assets.Entities;
using TT.Domain.Exceptions;
using TT.Domain.Statics;

namespace TT.Tests.Assets.Commands
{
    [TestFixture]
    public class UpdateRestockItemTests : TestBase
    {

        RestockItem restockItem;

        [SetUp]
        public void Init()
        {
            restockItem = new RestockItemBuilder().With(ri => ri.Id, 13).BuildAndSave();
        }

        [Test]
        public void Should_update_RestockItem()
        {
            var item = new ItemSourceBuilder().With(cr => cr.Id, 222).BuildAndSave();

            var cmdEdit = new UpdateRestockItem { RestockItemId = 13, AmountBeforeRestock = 25, BaseItemId = item.Id, AmountToRestockTo = 50, BotId = AIStatics.LindellaBotId };

            Assert.That(() => Repository.Execute(cmdEdit), Throws.Nothing);

            var editedRestockItem = DataContext.AsQueryable<RestockItem>().FirstOrDefault(cr => cr.Id == 13);

            Assert.That(editedRestockItem, Is.Not.Null);
            Assert.That(editedRestockItem.Id, Is.EqualTo(13));
            Assert.That(editedRestockItem.AmountBeforeRestock, Is.EqualTo(25));
            Assert.That(editedRestockItem.AmountToRestockTo, Is.EqualTo(50));
            Assert.That(editedRestockItem.BaseItem.Id, Is.EqualTo(222));
            Assert.That(editedRestockItem.BotId, Is.EqualTo(AIStatics.LindellaBotId));
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void Should_throw_error_when_base_item_id_is_invalid(int id)
        {
            var cmd = new UpdateRestockItem { AmountBeforeRestock = 0, BaseItemId = 0, RestockItemId = restockItem.Id, AmountToRestockTo = 5, BotId = AIStatics.LindellaBotId };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Base item id must be greater than 0"));
        }


        [Test]
        public void Should_throw_error_when_base_item_is_not_found()
        {
            const int id = 17;
            var cmd = new UpdateRestockItem { AmountBeforeRestock = 0, BaseItemId = id, RestockItemId = restockItem.Id, AmountToRestockTo = 5, BotId = AIStatics.LindellaBotId };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo($"Base item with Id {id} could not be found"));
        }

        [Test]
        public void Should_throw_error_when_invalid_amount_to_restock()
        {
            var amount = -1;

            var item = new ItemSourceBuilder().With(cr => cr.Id, 1).BuildAndSave();

            var cmd = new UpdateRestockItem { AmountBeforeRestock = amount, BaseItemId = item.Id, RestockItemId = restockItem.Id, AmountToRestockTo = 5, BotId = AIStatics.LindellaBotId };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Minimum amount before restock must be 0"));
        }

        [Test]
        public void Should_not_throw_error_when_amount_to_restock_is_zero()
        {
            var amount = 0;

            var item = new ItemSourceBuilder().With(cr => cr.Id, 215).BuildAndSave();

            var cmd = new UpdateRestockItem { AmountBeforeRestock = amount, BaseItemId = item.Id, RestockItemId = restockItem.Id, AmountToRestockTo = 5, BotId = AIStatics.LindellaBotId };

            Assert.That(() => Repository.Execute(cmd), Throws.Nothing);

            var editedRestockItem = DataContext.AsQueryable<RestockItem>().FirstOrDefault(cr => cr.Id == 13);

            Assert.That(editedRestockItem, Is.Not.Null);
            Assert.That(editedRestockItem.Id, Is.EqualTo(13));
        }

        [Test]
        public void Should_throw_error_when_invalid_amount_to_restockTo()
        {
            var amount = 0;

            var item = new ItemSourceBuilder().With(cr => cr.Id, 1).BuildAndSave();
            var npc = new NPCBuilder().With(n => n.Id, 7).BuildAndSave();

            var cmd = new UpdateRestockItem { AmountBeforeRestock = 1, BaseItemId = item.Id, RestockItemId = restockItem.Id, AmountToRestockTo = amount, BotId = AIStatics.LindellaBotId };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Minimum amount to restock to must be 1"));
        }

        [Test]
        public void Should_throw_error_when_no_restock_item_id()
        {
            var item = new ItemSourceBuilder().With(cr => cr.Id, 1).BuildAndSave();
            var npc = new NPCBuilder().With(n => n.Id, 7).BuildAndSave();

            var cmd = new UpdateRestockItem { AmountBeforeRestock = 1, BaseItemId = item.Id, AmountToRestockTo = 1, BotId = AIStatics.LindellaBotId };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("RestockItemId must be set"));
        }

        [Test]
        public void Should_throw_error_when_invalid_restock_item_id()
        {
            var restockId = 77;
            var item = new ItemSourceBuilder().With(cr => cr.Id, 1).BuildAndSave();
            var npc = new NPCBuilder().With(n => n.Id, 7).BuildAndSave();

            var cmd = new UpdateRestockItem { AmountBeforeRestock = 1, BaseItemId = item.Id, RestockItemId = restockId, AmountToRestockTo = 1, BotId = AIStatics.LindellaBotId };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message
                    .EqualTo($"Restock item with Id {restockId} could not be found"));
        }
    }
}
