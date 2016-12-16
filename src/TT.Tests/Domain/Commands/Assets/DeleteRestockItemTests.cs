using System;
using NUnit.Framework;
using FluentAssertions;
using System.Linq;
using TT.Domain;
using TT.Domain.Entities.Assets;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Assets;
using TT.Domain.Commands.Assets;

namespace TT.Tests.Domain.Commands.Assets
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

            Repository.Execute(cmd);

            DataContext.AsQueryable<RestockItem>().Count().Should().Be(0);
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void Should_throw_error_when_RestockItem_id_is_invalid(int id)
        {
            var cmd = new DeleteRestockItem { RestockItemId = id };

            Action action = () => Repository.Execute(cmd);
            action.ShouldThrowExactly<DomainException>().WithMessage("RestockItem Id must be greater than 0");
        }

        [Test]
        public void Should_throw_error_when_RestockItem_is_not_found()
        {
            const int id = 1;
            var cmd = new DeleteRestockItem { RestockItemId = id };

            Action action = () => Repository.Execute(cmd);
            action.ShouldThrowExactly<DomainException>().WithMessage($"RestockItem with ID {id} was not found");
        }
    }
}
