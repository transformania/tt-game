using NUnit.Framework;
using TT.Domain.Commands.Items;
using TT.Domain.Entities.Items;
using TT.Tests.Builders.Item;
using System.Linq;
using FluentAssertions;

namespace TT.Tests.Domain.Commands.Items
{
    [TestFixture]
    public class DropItemsTests : TestBase
    {

        [Test]
        public void player_can_drop_item()
        {
            var bob = new PlayerBuilder()
                .With(p => p.Id, 59)
                .With(p => p.Location, "hometown")
                .BuildAndSave();

            var item = new ItemBuilder()
                .With(i => i.Owner, bob)
                .With(i => i.Id, 87)
                .BuildAndSave();

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
    }
}
