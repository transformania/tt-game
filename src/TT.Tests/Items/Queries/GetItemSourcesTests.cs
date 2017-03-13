using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Items.Queries;
using TT.Tests.Builders.Item;

namespace TT.Tests.Items.Queries
{
    [TestFixture]
    public class GetItemSourcesTests : TestBase
    {
        [Test]
        public void Should_fetch_all_available_items()
        {
            new ItemSourceBuilder().With(cr => cr.Id, 200)
                .With(cr => cr.DbName, "dbName")
                .With(cr => cr.FriendlyName, "Friendly Name")
                .With(cr => cr.IsUnique, true)
                .BuildAndSave();

            new ItemSourceBuilder().With(cr => cr.Id, 200)
                .With(cr => cr.DbName, "dbName2")
                .With(cr => cr.FriendlyName, "Unfriendly Name!")
                .With(cr => cr.IsUnique, true)
                .BuildAndSave();

            var cmd = new GetItemSources();

            var items = DomainRegistry.Repository.Find(cmd);

            items.Should().HaveCount(2);
        }

        [Test]
        public void Should_return_empty_list_if_no_items_found()
        {
            var cmd = new GetItemSources();

            var items = DomainRegistry.Repository.Find(cmd);

            items.Should().BeEmpty();
        }

    }
}