using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Commands.Assets;
using TT.Domain.Commands.Items;
using TT.Domain.Entities.Items;
using TT.Tests.Builders.Item;

namespace TT.Tests.Domain.Commands.Items
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

            Repository.Execute(cmd);

            DataContext.AsQueryable<Item>().Count().Should().Be(0);
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void Should_throw_error_when_Item_id_is_invalid(int id)
        {

            var cmd = new DeleteItem { ItemId = id };

            Action action = () => Repository.Execute(cmd);
            action.ShouldThrowExactly<DomainException>().WithMessage("ItemId must be greater than 0");
        }

        [Test]
        public void Should_throw_error_when_Item_id_is_not_found()
        {
            const int id = 1;
            var cmd = new DeleteItem { ItemId = id };

            Action action = () => Repository.Execute(cmd);
            action.ShouldThrowExactly<DomainException>().WithMessage(string.Format("Item with ID {0} was not found", id));
        }
    }

}
