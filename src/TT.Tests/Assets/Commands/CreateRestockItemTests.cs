using NUnit.Framework;
using TT.Tests.Builders.Item;
using TT.Domain.Assets.Commands;
using TT.Domain.Exceptions;
using TT.Domain.Statics;
using TT.Tests.Builders.AI;

namespace TT.Tests.Assets.Commands
{
    [TestFixture]
    public class CreateRestockItemTests : TestBase
    {
        [Test]
        public void Should_create_new_RestockItem()
        {
            var item = new ItemSourceBuilder().With(cr => cr.Id, 195).BuildAndSave();
            var npc = new NPCBuilder().With(ri => ri.Id, 4).BuildAndSave();

            var cmd = new CreateRestockItem { AmountBeforeRestock = 0, BaseItemId = item.Id, AmountToRestockTo = 5, BotId = AIStatics.LindellaBotId };

            Assert.That(Repository.Execute(cmd), Is.GreaterThan(0));
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void Should_throw_error_when_base_item_id_is_invalid(int id)
        {
            var cmd = new CreateRestockItem { AmountBeforeRestock = 0, BaseItemId = 0, AmountToRestockTo = 5, BotId = AIStatics.LindellaBotId };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Base item id must be greater than 0"));
        }

        [Test]
        public void Should_throw_error_when_base_item_is_not_found()
        {
            const int id = 1;
            var cmd = new CreateRestockItem { AmountBeforeRestock = 0, BaseItemId = id, AmountToRestockTo = 5, BotId = AIStatics.LindellaBotId };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo($"Base item with Id {id} could not be found"));
        }

        [Test]
        public void Should_throw_error_when_invalid_amount_to_restock()
        {
            var amount = -1;

            var item = new ItemSourceBuilder().With(cr => cr.Id, 1).BuildAndSave();
            var npc = new NPCBuilder().With(n => n.Id, 7).BuildAndSave();

            var cmd = new CreateRestockItem { AmountBeforeRestock = amount, BaseItemId = item.Id, AmountToRestockTo = 5, BotId = AIStatics.LindellaBotId };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Minimum amount before restock must be 0"));
        }

        [Test]
        public void Should_not_throw_error_when_amount_to_restock_is_zero()
        {
            var amount = 0;

            var item = new ItemSourceBuilder().With(cr => cr.Id, 1).BuildAndSave();
            var npc = new NPCBuilder().With(n => n.Id, 7).BuildAndSave();

            var cmd = new CreateRestockItem { AmountBeforeRestock = amount, BaseItemId = item.Id, AmountToRestockTo = 5, BotId = AIStatics.LindellaBotId };

            Assert.That(Repository.Execute(cmd), Is.GreaterThan(0));
        }

        [Test]
        public void Should_throw_error_when_invalid_amount_to_restockTo()
        {
            var amount = 0;

            var item = new ItemSourceBuilder().With(cr => cr.Id, 1).BuildAndSave();
            var npc = new NPCBuilder().With(n => n.Id, 7).BuildAndSave();

            var cmd = new CreateRestockItem { AmountBeforeRestock = 1, BaseItemId = item.Id, AmountToRestockTo = amount, BotId = AIStatics.LindellaBotId };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Minimum amount to restock to must be 1"));
        }


    }
}
