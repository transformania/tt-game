
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain.Commands.Items;
using TT.Domain.Entities.Items;
using TT.Tests.Builders.Item;

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

    }
}
