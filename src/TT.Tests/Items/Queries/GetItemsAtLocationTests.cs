using System.Linq;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Items.Queries;
using TT.Tests.Builders.Item;

namespace TT.Tests.Items.Queries
{
    [TestFixture]
    public class GetItemsAtLocationTests : TestBase
    {
        [Test]
        public void get_all_items_at_location()
        {

            new ItemBuilder().With(i => i.Id, 77)
                .With(cr => cr.Owner, null)
                .With(cr => cr.dbLocationName, "swamps")
                .With(cr => cr.ItemSource, new ItemSourceBuilder().With(i => i.Id, 35).BuildAndSave())
                .BuildAndSave();

            new ItemBuilder().With(i => i.Id, 99)
                .With(cr => cr.Owner, null)
                .With(cr => cr.dbLocationName, "bog")
                .With(cr => cr.ItemSource, new ItemSourceBuilder().With(i => i.Id, 35).BuildAndSave())
                .BuildAndSave();

            new ItemBuilder().With(i => i.Id, 91)
                .With(cr => cr.Owner, null)
                .With(cr => cr.dbLocationName, "swamps")
                .With(cr => cr.ItemSource, new ItemSourceBuilder().With(i => i.Id, 35).BuildAndSave())
                .BuildAndSave();

            var cmd = new GetItemsAtLocation {dbLocationName = "swamps"};

            var items = DomainRegistry.Repository.Find(cmd);

            Assert.That(items, Has.Exactly(2).Items);
            Assert.That(items.First().Id, Is.EqualTo(77));
            Assert.That(items.Last().Id, Is.EqualTo(91));
        }

    }
}
