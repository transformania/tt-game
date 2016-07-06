using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Commands.Items;
using TT.Domain.Entities.Items;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Domain.Commands.Items
{
    public class CreateItemsTests : TestBase
    {

        [Test]
        public void Should_create_new_item_with_player()
        {

            new PlayerBuilder()
                .With(p => p.Id, 49).BuildAndSave();

             new ItemSourceBuilder()
                .With(i => i.Id, 195).BuildAndSave();

            var cmd = new CreateItem();
            cmd.OwnerId = 49;
            cmd.ItemSourceId = 195;

            Repository.Execute(cmd);

            DataContext.AsQueryable<Item>().Count(i =>
               i.Owner.Id == 49)
            .Should().Be(1);
        }

        [Test]
        public void Should_create_new_item_without_player()
        {

            new ItemSourceBuilder()
               .With(i => i.Id, 195).BuildAndSave();

            var cmd = new CreateItem
            {
                ItemSourceId = 195,
                OwnerId = null
            };

            Repository.Execute(cmd);

            DataContext.AsQueryable<Item>().Count(i =>
               i.Owner == null)
            .Should().Be(1);
        }

        [Test]
        public void Should_throw_exception_if_owner_not_null_and_player_not_found()
        {

            new ItemSourceBuilder()
              .With(i => i.Id, 7).BuildAndSave();

            var cmd = new CreateItem
            {
                ItemSourceId = 7,
                OwnerId = 999
            };

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage(string.Format("Player with Id {0} could not be found", 999));
        }

        [Test]
        public void Should_not_allow_item_with_no_itemSource()
        {
            var cmd = new CreateItem
            {
                ItemSourceId = 999,
                OwnerId = null
            };

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage(string.Format("Item Source with Id {0} could not be found", 999));
        }

    }
}
