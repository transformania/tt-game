using System;
using NUnit.Framework;
using FluentAssertions;
using TT.Domain.Commands.Assets;
using TT.Tests.Builders.Item;
using TT.Domain;
using TT.Tests.Builders.AI;

namespace TT.Tests.Domain.Commands.Assets
{
    [TestFixture]
    public class CreateRestockItemTests : TestBase
    {
        [Test]
        public void Should_create_new_RestockItem()
        {
            var item = new ItemBuilder().With(cr => cr.Id, 195).BuildAndSave();
            var npc = new NPCBuilder().With(ri => ri.Id, 4).BuildAndSave();

            var cmd = new CreateRestockItem { AmountBeforeRestock = 0, BaseItemId = item.Id, AmountToRestockTo = 5, NPCId = npc.Id };

            var RestockItem = Repository.Execute(cmd);

            RestockItem.Should().BeGreaterThan(0);
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void Should_throw_error_when_base_item_id_is_invalid(int id)
        {
            var cmd = new CreateRestockItem { AmountBeforeRestock = 0, BaseItemId = 0, AmountToRestockTo = 5, NPCId = id };

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("Base item Id must be greater than 0");
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void Should_throw_error_when_npc_id_is_invalid(int id)
        {
            var cmd = new CreateRestockItem { AmountBeforeRestock = 0, BaseItemId = 7, AmountToRestockTo = 5, NPCId = id };

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage("NPC Id must be greater than 0");
        }

        [Test]
        public void Should_throw_error_when_base_item_is_not_found()
        {
            const int id = 1;
            var cmd = new CreateRestockItem { AmountBeforeRestock = 0, BaseItemId = id, AmountToRestockTo = 5, NPCId = id };

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage(string.Format("Base item with Id {0} could not be found", id));
        }

        [Test]
        public void Should_throw_error_when_npc_is_not_found()
        {
            const int id = 1;
            var item = new ItemBuilder().With(cr => cr.Id, 1).BuildAndSave();
            var cmd = new CreateRestockItem { AmountBeforeRestock = 0, BaseItemId = 1, AmountToRestockTo = 5, NPCId = id };

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage(string.Format("NPC with Id {0} could not be found", id));
        }

        [Test]
        public void Should_throw_error_when_invalid_amount_to_restock()
        {
            var amount = -1;

            var item = new ItemBuilder().With(cr => cr.Id, 1).BuildAndSave();
            var npc = new NPCBuilder().With(n => n.Id, 7).BuildAndSave();

            var cmd = new CreateRestockItem { AmountBeforeRestock = amount, BaseItemId = item.Id, AmountToRestockTo = 5, NPCId = npc.Id };

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage(string.Format("Minimum amount before restock must be 0"));
        }

        [Test]
        public void Should_not_throw_error_when_amount_to_restock_is_zero()
        {
            var amount = 0;

            var item = new ItemBuilder().With(cr => cr.Id, 1).BuildAndSave();
            var npc = new NPCBuilder().With(n => n.Id, 7).BuildAndSave();

            var cmd = new CreateRestockItem { AmountBeforeRestock = amount, BaseItemId = item.Id, AmountToRestockTo = 5, NPCId = npc.Id };

            var RestockItem = Repository.Execute(cmd);

            RestockItem.Should().BeGreaterThan(0);
        }

        [Test]
        public void Should_throw_error_when_invalid_amount_to_restockTo()
        {
            var amount = 0;

            var item = new ItemBuilder().With(cr => cr.Id, 1).BuildAndSave();
            var npc = new NPCBuilder().With(n => n.Id, 7).BuildAndSave();

            var cmd = new CreateRestockItem { AmountBeforeRestock = 1, BaseItemId = item.Id, AmountToRestockTo = amount, NPCId = npc.Id };

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage(string.Format("Minimum amount to restock to must be 1"));
        }


    }
}
