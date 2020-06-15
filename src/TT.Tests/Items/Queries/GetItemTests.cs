using NUnit.Framework;
using TT.Domain;
using TT.Domain.Items.Queries;
using TT.Tests.Builders.Item;
using TT.Tests.Builders.Players;

namespace TT.Tests.Items.Queries
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

            Assert.That(item.Id, Is.EqualTo(77));
            Assert.That(item.Owner, Is.Null);
            Assert.That(item.ItemSource.Id, Is.EqualTo(35));
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

            Assert.That(item.Id, Is.EqualTo(77));
            Assert.That(item.Owner.FirstName, Is.EqualTo("Antony"));
        }

    }
}
