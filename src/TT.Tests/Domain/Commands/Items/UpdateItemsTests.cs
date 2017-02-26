using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Items.Commands;
using TT.Domain.Items.Entities;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Domain.Commands.Items
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

            Repository.Execute(cmdEdit);

            var editedItem = DataContext.AsQueryable<Item>().FirstOrDefault(cr => cr.Id == 7);

            editedItem.Id.Should().Be(7);
            editedItem.Owner.Id.Should().Be(123);
            editedItem.Owner.FirstName.Should().Be("Rupart");
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

            Repository.Execute(cmdEdit);

            var editedItem = DataContext.AsQueryable<Item>().FirstOrDefault(cr => cr.Id == 7);

            editedItem.Id.Should().Be(7);
            editedItem.Owner.Should().BeNull();
            editedItem.dbLocationName.Should().BeEquivalentTo("tampa");
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

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage($"Item with Id {999} could not be found");

        }


    }
}
