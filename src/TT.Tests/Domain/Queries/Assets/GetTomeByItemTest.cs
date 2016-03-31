using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Queries.Assets;
using TT.Tests.Builders.Assets;
using TT.Tests.Builders.Item;

namespace TT.Tests.Domain.Queries.Assets
{
    [TestFixture]
    public class GetTomeByItemTests : TestBase
    {
        [Test]
        public void Should_fetch_tome_by_item_id()
        {
            new TomeBuilder().With(cr => cr.Id, 7)
                .With(cr => cr.Text, "First Tome")
                .With(cr => cr.BaseItem, new ItemBuilder().With(cr => cr.Id, 195).BuildAndSave())
                .BuildAndSave();

            var cmd = new GetTomeByItem(195);

            var tome = DomainRegistry.Repository.Find(cmd);

            tome.Id.Should().Equals(7);
            tome.Text.Should().BeEquivalentTo("First Tome");
            tome.BaseItem.Id.Should().Equals(195);

        }
    }
}