using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain.Entities.Items;
using TT.Tests.Builders.Item;

namespace TT.Tests.Domain.Entities
{
    public class PlayerTests : TestBase
    {

        [Test]
        public void player_should_drop_all_items()
        {
            var player = new PlayerBuilder()
                .With(p => p.Id, 50)
                .With(p => p.Location, "beerhall")

                .BuildAndSave();

            var item1 = new ItemBuilder()
                .With(i => i.Id, 1)
                .With(i => i.Owner.Id, 50)
                .With(i => i.IsEquipped, false)
                .BuildAndSave();

            var item2 = new ItemBuilder()
                .With(i => i.Id, 2)
                .With(i => i.Owner.Id, 50)
                .With(i => i.IsEquipped, false)
                .BuildAndSave();

            player.Items.Add(item1);
            player.Items.Add(item2);

            player.DropAllItems();

            item1.Owner.Should().BeNull();
            item1.IsEquipped.Should().BeFalse();
            item1.dbLocationName.Should().Be("beerhall");

            item2.Owner.Should().BeNull();
            item2.IsEquipped.Should().BeFalse();
            item2.dbLocationName.Should().Be("beerhall");

        }

    }
}
