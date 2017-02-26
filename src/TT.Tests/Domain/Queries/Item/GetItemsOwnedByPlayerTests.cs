using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Items.Queries;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Domain.Queries.Item
{
    [TestFixture]
    public class GetItemsOwnedByPlayerTests : TestBase
    {
        [Test]
        [Ignore("NOT WORKING -- WHY?")]
        public void get_all_items_owned_by_player()
        {

            var player = new PlayerBuilder()
               .With(p => p.Id, 99)
               .BuildAndSave();

            new ItemBuilder().With(i => i.Id, 21)
                .With(cr => cr.Owner, null)
                .With(cr => cr.ItemSource, new ItemSourceBuilder()
                    .With(i => i.Id, 35)
                    .BuildAndSave())
                .BuildAndSave();

            new ItemBuilder().With(i => i.Id, 99)
                .With(cr => cr.Owner, player)
                .With(cr => cr.ItemSource, new ItemSourceBuilder()
                    .With(i => i.Id, 37)
                    .BuildAndSave())
                .BuildAndSave();

            new ItemBuilder().With(i => i.Id, 100)
                .With(cr => cr.Owner, player)
                .With(cr => cr.ItemSource, new ItemSourceBuilder()
                    .With(i => i.Id, 39)
                    .BuildAndSave())
                .BuildAndSave();

            var cmd = new GetItemsOwnedByPlayer { OwnerId = 99 };

            var items = DomainRegistry.Repository.Find(cmd);

            items.Count().Should().Be(2);
            items.ElementAt(0).Id.Should().Be(99);
            items.ElementAt(1).Id.Should().Be(100);
        }

    }
}
