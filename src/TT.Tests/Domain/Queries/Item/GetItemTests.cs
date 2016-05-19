using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Queries.Item;
using TT.Tests.Builders.Item;

namespace TT.Tests.Domain.Queries.Item
{
    public class GetItemTests : TestBase
    {

        [Test]
        public void Should_fetch_item_by_id()
        {
            new ItemBuilder().With(i => i.Id, 77)
                .With(cr => cr.Owner, null)
                .With(cr => cr.ItemSource, new ItemSourceBuilder().With(i => i.Id, 35).BuildAndSave())
                .BuildAndSave();

            var cmd = new GetItem { ItemId = 77 };

            var item = DomainRegistry.Repository.FindSingle(cmd);

            item.Id.Should().Be(77);
            item.Owner.Should().Be(null);
            item.ItemSource.Id.Should().Be(35);
        }

        [Test]
        public void Should_fetch_item_by_id_with_player()
        {

           var player = new PlayerBuilder()
               .With(p => p.Id, 99)
               .With(p => p.FirstName, "Antony")
                .BuildAndSave();

            new ItemBuilder().With(i => i.Id, 77)
                .With(cr => cr.Owner, player)
                .With(cr => cr.ItemSource, new ItemSourceBuilder().With(i => i.Id, 35).BuildAndSave())
                .BuildAndSave();

            var cmd = new GetItem { ItemId = 77 };
            var item = DomainRegistry.Repository.FindSingle(cmd);

            item.Id.Should().Be(77);
            item.Owner.FirstName.Equals("Antony");
        }

    }
}
