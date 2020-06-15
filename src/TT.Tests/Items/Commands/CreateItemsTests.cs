using System.Linq;
using NUnit.Framework;
using TT.Domain.Exceptions;
using TT.Domain.Items.Commands;
using TT.Domain.Items.Entities;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Items.Commands
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

             var cmd = new CreateItem
             {
                 OwnerId = 49, ItemSourceId = 195
             };

             Assert.That(() => Repository.Execute(cmd), Throws.Nothing);

             Assert.That(DataContext.AsQueryable<Item>().Where(i => i.Owner.Id == 49), Has.Exactly(1).Items);
        }

        [Test]
        public void Should_create_new_item_with_former_player()
        {

            new PlayerBuilder()
                .With(p => p.Id, 49).BuildAndSave();

             new PlayerBuilder()
                .With(p => p.Id, 57)
                .With(p => p.FirstName, "Gerald")
                .BuildAndSave();

            new ItemSourceBuilder()
               .With(i => i.Id, 195).BuildAndSave();

            var cmd = new CreateItem
            {
                OwnerId = 49,
                ItemSourceId = 195,
                FormerPlayerId = 57
            };

            Assert.That(() => Repository.Execute(cmd), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<Item>().Where(i => i.FormerPlayer.Id == 57), Has.Exactly(1).Items);
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

            Assert.That(() => Repository.Execute(cmd), Throws.Nothing);

            Assert.That(DataContext.AsQueryable<Item>().Where(i => i.Owner == null), Has.Exactly(1).Items);
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

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Player with Id 999 could not be found"));
        }

        [Test]
        public void Should_throw_exception_if_former_player_not_null_and_player_not_found()
        {

            new ItemSourceBuilder()
              .With(i => i.Id, 7).BuildAndSave();

            var cmd = new CreateItem
            {
                ItemSourceId = 7,
                FormerPlayerId = 999
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Former player with Id 999 could not be found"));
        }

        [Test]
        public void Should_not_allow_item_with_no_itemSource()
        {
            var cmd = new CreateItem
            {
                ItemSourceId = 999,
                OwnerId = null
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo($"Item Source with Id {999} could not be found"));
        }

    }
}
