using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Items.Queries;
using TT.Tests.Builders.Item;

namespace TT.Tests.Items.Queries
{
    [TestFixture]
    public class GetAllitemsOfTypeTests : TestBase
    {
        [Test]
        public void get_all_items_of_itemsource_id()
        {

            new ItemBuilder().With(i => i.Id, 21)
                .With(cr => cr.Owner, null)
                .With(cr => cr.dbLocationName, "swamps")
                .With(cr => cr.ItemSource, new ItemSourceBuilder().With(i => i.Id, 35).BuildAndSave())
                .BuildAndSave();

            new ItemBuilder().With(i => i.Id, 99)
                .With(cr => cr.Owner, null)
                .With(cr => cr.dbLocationName, "bog")
                .With(cr => cr.ItemSource, new ItemSourceBuilder().With(i => i.Id, 86).BuildAndSave())
                .BuildAndSave();

            new ItemBuilder().With(i => i.Id, 100)
                .With(cr => cr.Owner, null)
                .With(cr => cr.dbLocationName, "swamps")
                .With(cr => cr.ItemSource, new ItemSourceBuilder().With(i => i.Id, 35).BuildAndSave())
                .BuildAndSave();

            var cmd = new GetAllItemsOfType { ItemSourceId = 35 };

            var items = DomainRegistry.Repository.Find(cmd);

            items.Should().HaveCount(2);
            items.First().Id.Should().Be(21);
            items.Last().Id.Should().Be(100);
        }

    }
}
