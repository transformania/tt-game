using NUnit.Framework;
using TT.Tests.Builders.Item;
using System.Linq;
using TT.Domain.Exceptions;
using TT.Domain.Items.Commands;
using TT.Domain.Items.Entities;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;
using TT.Tests.Builders.Players;

namespace TT.Tests.Items.Commands
{
    [TestFixture]
    public class DropItemsTests : TestBase
    {
        private Player bob;
        private Item item;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            bob = new PlayerBuilder()
                .With(p => p.Id, 59)
                .With(p => p.Location, "hometown")
                .BuildAndSave();

            item = new ItemBuilder()
                .With(i => i.Owner, bob)
                .With(i => i.Id, 87)
                .BuildAndSave();
        }

        [Test]
        public void player_can_drop_item()
        {

            var cmd = new DropItem
            {
                OwnerId = bob.Id,
                ItemId = item.Id
            };

            Assert.That(() => Repository.Execute(cmd), Throws.Nothing);

            var editedItem = DataContext.AsQueryable<Item>().FirstOrDefault(i => i.Id == 87);

            Assert.That(editedItem, Is.Not.Null);
            Assert.That(editedItem.Owner, Is.Null);
            Assert.That(editedItem.dbLocationName, Is.EqualTo("hometown"));
        }

        [Test]
        public void player_can_drop_item_with_location_override()
        {

            var cmd = new DropItem
            {
                OwnerId = bob.Id,
                ItemId = item.Id,
                LocationOverride = LocationsStatics.STREET_70_EAST_9TH_AVE
            };

            Assert.That(() => Repository.Execute(cmd), Throws.Nothing);

            var editedItem = DataContext.AsQueryable<Item>().FirstOrDefault(i => i.Id == 87);

            Assert.That(editedItem, Is.Not.Null);
            Assert.That(editedItem.Owner, Is.Null);
            Assert.That(editedItem.dbLocationName, Is.EqualTo(LocationsStatics.STREET_70_EAST_9TH_AVE));
        }

        [Test]
        public void item_must_be_owned_by_player()
        {
            var sam = new PlayerBuilder()
                .With(p => p.Id, 60)
                .With(p => p.Location, "hometown")
                .BuildAndSave();

            var cmd = new DropItem
            {
                OwnerId = sam.Id,
                ItemId = item.Id
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo($"player {sam.Id} does not own item {item.Id}"));
        }

        [Test]
        public void player_cant_drop_non_owned_item()
        {
            var unownedItem = new ItemBuilder()
                .With(i => i.Id, 88)
                .BuildAndSave();
            
            var cmd = new DropItem
            {
                OwnerId = bob.Id,
                ItemId = unownedItem.Id
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo($"player {bob.Id} does not own item {unownedItem.Id}"));
        }

        [Test]
        public void player_cant_drop_nonexistent_item()
        {
            var cmd = new DropItem
            {
                OwnerId = bob.Id,
                ItemId = 100
            };

            Assert.That(() => Repository.Execute(cmd),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("Item with ID 100 could not be found"));
        }

        [Test]
        [Ignore("TODO")]
        public void player_must_be_animate_to_drop_item()
        {

        }

        [Test]
        [Ignore("TODO")]
        public void player_cannot_be_in_quest_and_drop_item()
        {

        }

        [Test]
        [Ignore("TODO")]
        public void player_cannot_be_in_duel_and_drop_item()
        {

        }

        [Test]
        [Ignore("TODO")]
        public void player_player_is_not_wearing_item_to_drop_unless_pet_type()
        {

        }

        [Test]
        [Ignore("TODO")]
        public void player_must_not_be_mindcontrolled_to_drop_item()
        {

        }

        [Test]
        [Ignore("TODO")]
        public void items_dropped_in_dungeon_appear_in_overworld()
        {

        }

    }
}
