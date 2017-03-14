using NUnit.Framework;
using TT.Tests.Builders.Item;
using System.Linq;
using FluentAssertions;
using TT.Domain.Items.Commands;
using TT.Domain.Items.Entities;
using TT.Domain.Players.Entities;
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

            Repository.Execute(cmd);

            var editedItem = DataContext.AsQueryable<Item>().FirstOrDefault(i => i.Id == 87);

            editedItem.Owner.Should().BeNull();
            editedItem.dbLocationName.Should().Be("hometown");

        }

        [Test]
        [Ignore("TODO")]
        public void item_must_be_owned_by_player()
        {

        }

        [Test]
        [Ignore("TODO")]
        public void player_cant_drop_non_owned_item()
        {
            
        }

        [Test]
        [Ignore("TODO")]
        public void player_cant_drop_nonexistent_item()
        {

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
