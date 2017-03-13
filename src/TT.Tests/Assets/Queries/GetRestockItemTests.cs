using System;
using FluentAssertions;
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

            var RestockItem = DomainRegistry.Repository.FindSingle(query);

            RestockItem.Id.Should().Be(7);
            RestockItem.AmountBeforeRestock.Should().Be(5);
            RestockItem.AmountToRestockTo.Should().Be(9);
            RestockItem.BaseItem.Id.Should().Be(35);
            RestockItem.BotId.Should().Be(AIStatics.LindellaBotId);
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void Should_throw_error_when_RestockItem_id_is_invalid(int id)
        {
            var query = new GetRestockItem { RestockItemId = id };

            var action = new Action(() => { Repository.FindSingle(query); });

            action.ShouldThrowExactly<DomainException>().WithMessage("RestockItem Id must be greater than 0");
        }

        [Test]
        public void Should_return_null_if_RestockItem_is_not_found()
        {
            var query = new GetRestockItem { RestockItemId = 1 };

            var RestockItem = Repository.FindSingle(query);

            RestockItem.Should().BeNull();
        }
    }
}